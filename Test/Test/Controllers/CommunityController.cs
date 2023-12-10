using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Models;
using Test.Models.DTO;

namespace Test.Controllers
{
    [Route("/api/community/")]
    [ApiController]
    public class CommunityController : Controller
    {
        private readonly TestContext _context;

        public CommunityController(TestContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CommunityDto), 200)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult GetCommunity()
        {
            var community = _context.Communities.ToList();
            var communities = community.Select(community => new CommunityDto
            {
                id = community.id,
                createTime = community.createTime,
                name = community.name,
                description = community.description,
                isClosed = community.isClosed,
                subscribersCount = community.subscribersCount,

            }).ToList();

            return Ok(communities);
        }

        [HttpGet("my")]
        [Authorize] // Требуется аутентификация]
        [ProducesResponseType(typeof(CommunityUserDto), 200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(Response), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult GetMyCommunity()
        {
            // Получаем идентификатор авторизованного пользователя из токена
            string authorizationHeader = Request.Headers["Authorization"];
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
            Guid userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.id == userId);

            if (user == null)
            {
                return StatusCode(404, new { status = "error", message = "User not found" });
            }

            var communityUsers = _context.CommunityUsers.Where(cu => cu.userId == user.id).ToList();

            return Ok(communityUsers);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommunityFullDto), 200)]
        [ProducesResponseType(typeof(Response), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult GetPostId(Guid id)
        {
            var community = _context.Communities.SingleOrDefault(p => p.id == Guid.Parse(id.ToString()));
            if (community == null)
            {
                return StatusCode(404, new { status = "error", message = $"Community with id='{id}' not found in  database" });
            }

            var userIds = _context.CommunityUsers.Where(pt => (pt.communityId == community.id) && (pt.role == CommunityRole.Administrator)).Select(pt => pt.userId).ToList();
            var users = _context.Users.Where(t => userIds.Contains(t.id)).ToList();
            var userDto = users.Select(user => new UserDto
            {
                id = user.id,
                createTime = user.createTime,
                fullName = user.fullName,
                birthDate = user.birthDate,
                gender = user.gender,
                email = user.email,
                phoneNumber = user.phoneNumber
            }).ToList();

            community.administrators = userDto;

            return Ok(community);
        }

        [HttpPost("{id}/post")]
        [Authorize] // Требуется аутентификация]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(Response), 401)]
        [ProducesResponseType(typeof(Response), 403)]
        [ProducesResponseType(typeof(Response), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult CreatePostInCommunity([Required] Guid id, CreatePostDto postDto)
        {
            // Получаем идентификатор авторизованного пользователя из токена
            string authorizationHeader = Request.Headers["Authorization"];
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
            Guid userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.id == userId);

            if (user == null)
            {
                return StatusCode(404, new { status = "error", message = "User not found" });
            }

            var community = _context.Communities.SingleOrDefault(p => p.id == Guid.Parse(id.ToString()));
            if (community == null)
            {
                return StatusCode(404, new { status = "error", message = $"Community with id='{id}' not found in  database" });
            }

            var userIds = _context.CommunityUsers.Where(pt => (pt.communityId == community.id) && (pt.role == CommunityRole.Administrator)).Select(pt => pt.userId).ToList();
            var userAdmin = _context.Users.FirstOrDefault(t => userIds.Contains(t.id));
            if (userAdmin == null || userAdmin?.id != user.id)
            {
                return StatusCode(403, new { status = "error", message = "User is not able to post in the community" });
            }


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
                    return StatusCode(404, new { status = "error", message = $"Tag with id='{tagId}' not found in  database" });
                }
            }

            List<Guid> tagIds1 = new List<Guid>();
            foreach (TagDto tag in tags)
            {
                tagIds1.Add(tag.id);
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
                communityId = community.id,
                communityName = community.name,
                addressId = postDto.addressId,
                likes = 0,
                hasLike = false,
                commentsCount = 0,
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

        [HttpGet("{id}/role")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(Response), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult GetRolePostId(Guid id)
        {
            string authorizationHeader = Request.Headers["Authorization"];
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
            Guid userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.id == userId);

            if (user == null)
            {
                return StatusCode(404, new { status = "error", message = "User not found" });
            }



            var community = _context.Communities.SingleOrDefault(p => p.id == Guid.Parse(id.ToString()));
            if (community == null)
            {
                return StatusCode(404, new { status = "error", message = $"Community with id='{id}' not found in  database" });
            }

            var role = _context.CommunityUsers.FirstOrDefault(r => r.userId == user.id && r.communityId == id)?.role;

            return role != null ? Ok(role) : Ok(Enumerable.Empty<string>());
        }


        [HttpPost("{id}/subscribe")]
        [Authorize]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult AddSubscribeToPost(Guid id)
        {
            // Получаем текущего пользователя

            string authorizationHeader = Request.Headers["Authorization"];
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
            Guid userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);

            var community = _context.Communities.FirstOrDefault(p => p.id == id);
            if (community == null)
            {
                return StatusCode(404, new { status = "error", message = $"Community with id='{id}' not found in  database" });
            }

            var existingSub = _context.CommunityUsers.FirstOrDefault(s => s.communityId == id && s.userId == userId);
            if (existingSub != null)
            {
                return StatusCode(400, new { status = "error", message = "You have already subscribe or you are administrator this community." });
            }

            var communityUser = new CommunityUserDto
            {
                communityId = community.id,
                userId = userId,
                role = CommunityRole.Subscriber
            };

            community.subscribersCount++;

            _context.CommunityUsers.Add(communityUser);

            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}/unsubscribe")]
        [Authorize]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult DeleteLikeToPost(Guid id)
        {

            string authorizationHeader = Request.Headers["Authorization"];
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
            Guid userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);

            var community = _context.Communities.FirstOrDefault(p => p.id == id);
            if (community == null)
            {
                return StatusCode(404, new { status = "error", message = $"Community with id='{id}' not found in  database" });
            }

            // Проверяем, был ли уже лайк от этого пользователя к этому посту
            var existingSub = _context.CommunityUsers.FirstOrDefault(s => s.communityId == id && s.userId == userId && s.role == CommunityRole.Subscriber);
            if (existingSub == null)
            {
                return StatusCode(400, new { status = "error", message = "You havn't subscribe on this community yet." });
            }

            community.subscribersCount--;

            _context.CommunityUsers.Remove(existingSub);

            _context.SaveChanges();

            return Ok();
        }

        [HttpGet("{id}/post")]
        //[Authorize] // Требуется аутентификация
        [ProducesResponseType(typeof(PostPagedListDto), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult GetPosts([Required] Guid id, [FromQuery] string?[] tags, PostSorting? sorting, [Range(1, int.MaxValue), DefaultValue(1)] int? page, [Range(1, int.MaxValue), DefaultValue(5)] int? size)
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

            var community = _context.Communities.FirstOrDefault(p => p.id == id);
            if (community == null)
            {
                return StatusCode(404, new { status = "error", message = $"Community with id='{id}' not found in  database" });
            }

            if (community.isClosed)
            {
                var communityUser = _context.CommunityUsers.FirstOrDefault(cu => cu.userId == userId && cu.communityId == id);
                if (communityUser == null)
                {
                    return StatusCode(403, new { status = "error", message = $"Access to closed community with id='{id}' is forbidden" });
                }
            }

            var posts = _context.Posts.Where(p => p.communityId == id).AsQueryable();

            if (tags != null && tags.Length > 0)
            {
                var tagGuids = tags.Select(Guid.Parse).ToArray();
                var postIdsWithTags = _context.PostTags.Where(pt => tagGuids.Contains(pt.tagId)).Select(pt => pt.postId).Distinct();
                posts = posts.Where(p => postIdsWithTags.Contains(p.id));
            }

            var postList = posts.ToList();

            foreach (PostDto post in postList)
            {
                var tagIds = _context.PostTags.Where(pt => pt.postId == post.id).Select(pt => pt.tagId).ToList();
                var tag = _context.Tags.Where(t => tagIds.Contains(t.id)).ToList();
                post.tags = tag;
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
                    count = (int)Math.Ceiling((double)totalCount / size.Value),
                    current = page ?? 0
                }
            };

            return Ok(result);
        }
    }
}
