using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Test.Models;
using Test.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Test.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.ComponentModel;

namespace Test.Controllers
{
    [Route("/api/post/")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly TestContext _context;

        //private readonly IGetTokenService _tokenService;

        public PostController(TestContext context/*, IGetTokenService tokenService*/)
        {
            _context = context;
            // _tokenService = tokenService;
        }

        [HttpPost]
        [Authorize] // Требуется аутентификация
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(Response), 401)]
        [ProducesResponseType(typeof(Response), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult CreatePost(CreatePostDto postDto)
        {
            // Получаем идентификатор авторизованного пользователя из токена
            string authorizationHeader = Request.Headers["Authorization"];
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
            Guid userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.id == userId);

            var tagIds = postDto.tags; // Получение массива id тегов из postDto 
            var tags = new List<TagDto>(); // Создание списка для хранения объектов тегов 
            foreach (var tagId in tagIds)
            {
                var existingTag = _context.Tags.FirstOrDefault(t => t.id == Guid.Parse(tagId)); // Поиск существующего тега по id 
                if (existingTag != null)
                {
                    tags.Add(existingTag); // Использование существующего тега 
                }
                else
                {
                    ModelState.AddModelError("$.tags[0]", $"The tag with id '{tagId}' was not found.");
                }
            }

            List<Guid> tagIds1 = new List<Guid>();
            foreach (TagDto tag in tags)
            {
                tagIds1.Add(tag.id);
            }

            if (!ModelState.IsValid)
            {
                // Возвращаем ответ с ошибкой валидации модели
                var validationErrors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                var response = new
                {
                    type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    title = "One or more validation errors occurred.",
                    status = 400,
                    traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    errors = validationErrors
                };

                return BadRequest(response);
            }

            // Создаем новый объект поста на основе данных из запроса
            var post = new PostDto
            {
                id = Guid.NewGuid(),
                createTime = DateTime.Now.ToString(),
                title = postDto.title,
                description = postDto.description,
                readingTime = postDto.readingTime,
                image = postDto.image,
                authorId = userId,
                author = user.fullName,
                addressId = postDto.addressId,
                likes = 0,
                hasLike = false,
                commentsCount = 0,
                //tags = tags,
            };

            // Добавляем пост в контекст базы данных
            _context.Posts.Add(post);

            foreach (Guid tagId in tagIds1)
            {
                var tags_database = new PostTag
                {
                    postId = post.id,
                    tagId = tagId,
                };
                _context.PostTags.Add(tags_database);
                _context.SaveChanges();
            }


            // Возвращаем созданный пост
            return Ok(post.id);
        }

        [HttpGet("{id}")]
        //[Authorize] // Требуется аутентификация
        [ProducesResponseType(typeof(PostFullDto), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(Response), 401)]
        [ProducesResponseType(typeof(Response), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult GetPostId(Guid id)
        {
            var post = _context.Posts.SingleOrDefault(p => p.id == Guid.Parse(id.ToString()));
            if (post == null)
            {
                return StatusCode(404, new { status = "error", message = $"Post with id='{id}' not found in  database" });
            }
            //post.tags = _context.Tags.Where(t => t.PostDtoid == id).ToList();

            var tagIds = _context.PostTags.Where(pt => pt.postId == post.id).Select(pt => pt.tagId).ToList();
            var tags = _context.Tags.Where(t => tagIds.Contains(t.id)).ToList();
            post.tags = tags;

            var commentIds = _context.PostComment.Where(pt => pt.postId == post.id).Select(pt => pt.commentId).ToList();
            var nestedCommentIds = _context.CommentComment.Select(cc => cc.commentId1).ToList();
            var comments = _context.Comments
                .Where(c => commentIds.Contains(c.id))
                .Except(_context.Comments.Where(c => nestedCommentIds.Contains(c.id)))
                .ToList();

            var postFull = new PostFullDto
            {
                id = post.id,
                createTime = post.createTime,
                title = post.title,
                description = post.description,
                readingTime = post.readingTime,
                image = post.image,
                authorId = post.authorId,
                author = post.author,
                addressId = post.addressId,
                likes = post.likes,
                hasLike = post.hasLike,
                commentsCount = post.commentsCount,
                tags = post.tags,
                comments = comments,
            };


            return Ok(postFull);
        }


        [HttpGet]
        //[Authorize] // Требуется аутентификация
        [ProducesResponseType(typeof(PostPagedListDto), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult GetPosts([FromQuery] string?[] tags, [FromQuery] string? author, int? min, int? max, PostSorting? sorting, [Range(1, int.MaxValue), DefaultValue(1)] int? page, [Range(1, int.MaxValue), DefaultValue(5)] int? size)
        {
            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                string authorizationHeader = Request.Headers["Authorization"];
                string bearerToken = authorizationHeader.Substring("Bearer ".Length);
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
                userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
            }
            var posts = _context.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(author))
            {
                posts = posts.Where(p => p.author.Contains(author));
            }

            if (tags != null && tags.Length > 0)
            {
                var tagGuids = tags.Select(Guid.Parse).ToArray();
                var postIdsWithTags = _context.PostTags.Where(pt => tagGuids.Contains(pt.tagId)).Select(pt => pt.postId).Distinct();
                posts = posts.Where(p => postIdsWithTags.Contains(p.id));
            }

            if (min.HasValue)
            {
                posts = posts.Where(p => p.readingTime >= min.Value);
            }

            if (max.HasValue)
            {
                posts = posts.Where(p => p.readingTime <= max.Value);
            }

            var postList = posts.ToList();

            for (int i = postList.Count - 1; i >= 0; i--)
            {
                var post = postList[i];
                var tagIds = _context.PostTags.Where(pt => pt.postId == post.id).Select(pt => pt.tagId).ToList();
                var tag = _context.Tags.Where(t => tagIds.Contains(t.id)).ToList();
                post.tags = tag;

                var community = _context.Communities.FirstOrDefault(c => c.id == post.communityId && c.isClosed);
                if (community != null)
                {
                    var userSubscribed = _context.CommunityUsers.Any(cu => cu.userId == userId && cu.communityId == post.communityId);
                    if (!userSubscribed)
                    {
                        postList.RemoveAt(i);
                    }
                }
            }

            if (sorting.HasValue)
            {
                switch (sorting.Value)
                {
                    case PostSorting.CreateDesc:
                        postList = postList.OrderByDescending(p => p.createTime).ToList();
                        break;
                    case PostSorting.CreateAsc:
                        postList = postList.OrderBy(p => p.createTime).ToList();
                        break;
                    case PostSorting.LikeAsc:
                        postList = postList.OrderBy(p => p.likes).ToList();
                        break;
                    case PostSorting.LikeDesc:
                        postList = postList.OrderByDescending(p => p.likes).ToList();
                        break;
                }
            }

            var totalCount = postList.Count();

            if (size.HasValue && page.HasValue)
            {
                postList = postList.Skip((page.Value - 1) * size.Value).Take(size.Value).ToList();
            }

            var result = new PostPagedListDto
            {
                posts = postList,
                pagination = new PageInfoModel
                {
                    size = size ?? 0,
                    count = (int)Math.Ceiling((double)totalCount/size.Value),
                    current = page ?? 0
                }
            };

            return Ok(result);
        }


        [HttpPost("{postId}/like")]
        [Authorize]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult AddLikeToPost(Guid postId)
        {
            // Получаем текущего пользователя

            string authorizationHeader = Request.Headers["Authorization"];
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
            Guid userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);

            // Проверяем, существует ли пост с указанным идентификатором
            var post = _context.Posts.FirstOrDefault(p => p.id == postId);
            if (post == null)
            {
                return StatusCode(404, new { status = "error", message = $"Post with id='{postId}' not found in  database" });
            }

            // Проверяем, был ли уже лайк от этого пользователя к этому посту
            var existingLike = _context.PostLikes.FirstOrDefault(pl => pl.postId == postId && pl.userId == userId);
            if (existingLike != null)
            {
                return StatusCode(400, new { status = "error", message = "You have already liked this post." });
            }


            var community = _context.Communities.FirstOrDefault(c => c.id == post.communityId);
            if (community != null && community.isClosed)
            {
                var userSubscribed = _context.CommunityUsers.Any(c => c.userId == userId && c.communityId == post.communityId);
                if (!userSubscribed)
                {
                    return StatusCode(403, new { status = "error", message = "Community is closed" });
                }
            }


            // Добавляем лайк к посту
            post.likes++;

            var like = new PostLiked
            {
                postId = post.id,
                userId = userId,
            };
            _context.PostLikes.Add(like);

            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{postId}/like")]
        [Authorize]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult DeleteLikeToPost(Guid postId)
        {

            string authorizationHeader = Request.Headers["Authorization"];
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
            Guid userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);

            // Проверяем, существует ли пост с указанным идентификатором
            var post = _context.Posts.FirstOrDefault(p => p.id == postId);
            if (post == null)
            {
                return StatusCode(404, new { status = "error", message = $"Post with id='{postId}' not found in  database" });
            }

            // Проверяем, был ли уже лайк от этого пользователя к этому посту
            var existingLike = _context.PostLikes.FirstOrDefault(pl => pl.postId == postId && pl.userId == userId);
            if (existingLike == null)
            {
                return StatusCode(400, new { status = "error", message = "You havn't liked this post yet." });
            }

            post.likes--;

            _context.PostLikes.Remove(existingLike);

            _context.SaveChanges();

            return Ok();
        }

    }
}
