using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Models;
using Test.Models.DTO;


namespace Test.Controllers
{
    [Route("/api/comment/")]
    [ApiController]
    public class CommentController : ControllerBase
    {

        private readonly TestContext _context;

        public CommentController(TestContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/tree")]
        [ProducesResponseType(typeof(List<CommentDto>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public ActionResult<List<CommentDto>> GetNestedComments(Guid id)
        {
            try
            {
                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    string authorizationHeader = Request.Headers["Authorization"];
                    string bearerToken = authorizationHeader.Substring("Bearer ".Length);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
                    userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);

                    var logoutToken = _context.LogoutTokens.FirstOrDefault(t => t.token == bearerToken);
                    if (logoutToken != null)
                    {
                        return StatusCode(401, new { status = "error", message = "Недействительный токен" });
                    }
                }

                var postComment = _context.PostComment.FirstOrDefault(pc => pc.commentId == id);

                var post = _context.Posts.FirstOrDefault(p => p.id == postComment.postId);

                var community = _context.Communities.FirstOrDefault(c => c.id == post.communityId);

                if (community != null)
                {
                    if (community.isClosed)
                    {
                        var communityUser = _context.CommunityUsers.FirstOrDefault(cu => cu.communityId == community.id && cu.userId == userId);
                        if (communityUser == null)
                        {
                            return StatusCode(403, new { status = "error", message = "User is not subscribed to the community" });
                        }
                    }
                }


                var parentComment = _context.Comments.FirstOrDefault(c => c.id == id);
                if (parentComment == null)
                {
                    return StatusCode(404, new { status = "error", message = $"Parent comment with id='{id}' not found" });
                }

                var childComments = _context.CommentComment
                    .Where(cc => cc.commentId2 == id).Join(_context.Comments, cc => cc.commentId1, c => c.id, (cc, c) => c).ToList();

                return Ok(childComments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { message = ex.Message });
            }
        }

        [HttpPost("{id}/comment")]
        [Authorize]
        [ProducesResponseType(typeof(CommentDto), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<ActionResult> CreateComment([Required] Guid id, [FromBody] CreateCommentDto commentDto)
        {
            try
            {
                string authorizationHeader = Request.Headers["Authorization"];
                string bearerToken = authorizationHeader.Substring("Bearer ".Length);
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
                Guid userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
                var user = _context.Users.FirstOrDefault(u => u.id == userId);

                var logoutToken = _context.LogoutTokens.FirstOrDefault(t => t.token == bearerToken);
                if (logoutToken != null)
                {
                    return StatusCode(401, new { status = "error", message = "Недействительный токен" });
                }

                if (user == null)
                {
                    return StatusCode(404, new { status = "error", message = "User not found" });
                }

                var post = _context.Posts.SingleOrDefault(p => p.id == Guid.Parse(id.ToString()));
                if (post == null)
                {
                    return StatusCode(404, new { status = "error", message = $"Post with id='{id}' not found in  database" });
                }

                // Создание комментария
                var comment = new CommentDto
                {
                    id = Guid.NewGuid(),
                    createTime = DateTime.Now.ToString(),
                    content = commentDto.content,
                    authorId = user.id,
                    author = user.fullName,
                    subComments = 0,
                };

                var postComment = new PostComment
                {
                    postId = post.id,
                    commentId = comment.id,
                };

                post.commentsCount++;
                _context.Comments.Add(comment);
                _context.PostComment.Add(postComment);

                if (commentDto.parentId != null)
                {
                    var parentComment = _context.Comments.FirstOrDefault(c => c.id == commentDto.parentId);
                    if (parentComment != null)
                    {
                        var commentComment = new CommentComment
                        {
                            commentId1 = comment.id,
                            commentId2 = parentComment.id,
                        };
                        parentComment.subComments++;
                        _context.CommentComment.Add(commentComment);
                        await _context.SaveChangesAsync();
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(comment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(CommentDto), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<ActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateCommentDto)
        {
            try
            {
                string authorizationHeader = Request.Headers["Authorization"];
                string bearerToken = authorizationHeader.Substring("Bearer ".Length);
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
                Guid userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
                var user = _context.Users.FirstOrDefault(u => u.id == userId);

                var logoutToken = _context.LogoutTokens.FirstOrDefault(t => t.token == bearerToken);
                if (logoutToken != null)
                {
                    return StatusCode(401, new { status = "error", message = "Недействительный токен" });
                }

                if (user == null)
                {
                    return StatusCode(404, new { status = "error", message = "User not found" });
                }

                var comment = await _context.Comments.FindAsync(id);
                if (comment == null)
                {
                    return StatusCode(404, new { status = "error", message = $"Comment with id='{id}' not found in database" });
                }

                if (user.id != comment.authorId)
                {
                    return StatusCode(404, new { status = "error", message = "This is not your comment" });
                }

                comment.content = updateCommentDto.content;
                comment.modifiedDate = DateTime.Now.ToString();

                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();

                return Ok(comment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(CommentDto), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<ActionResult> DeleteComment(Guid id)
        {
            try
            {
                string authorizationHeader = Request.Headers["Authorization"];
                string bearerToken = authorizationHeader.Substring("Bearer ".Length);
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
                Guid userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
                var user = _context.Users.FirstOrDefault(u => u.id == userId);

                var logoutToken = _context.LogoutTokens.FirstOrDefault(t => t.token == bearerToken);
                if (logoutToken != null)
                {
                    return StatusCode(401, new { status = "error", message = "Недействительный токен" });
                }

                if (user == null)
                {
                    return StatusCode(404, new { status = "error", message = "User not found" });
                }

                var comment = await _context.Comments.FindAsync(id);
                if (comment == null)
                {
                    return StatusCode(404, new { status = "error", message = $"Comment with id='{id}' not found in database" });
                }

                if (user.id != comment.authorId)
                {
                    return StatusCode(404, new { status = "error", message = "This is not your comment" });
                }

                comment.content = " ";
                comment.deleteDate = DateTime.Now.ToString();

                Guid postId = _context.PostComment.FirstOrDefault(pt => pt.commentId == id).postId;
                var post = await _context.Posts.FindAsync(postId);
                post.commentsCount--;

                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();

                return Ok(comment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { message = ex.Message });
            }
        }
    }
}
