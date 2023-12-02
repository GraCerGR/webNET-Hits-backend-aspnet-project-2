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

namespace Test.Controllers
{
    [Route("/api/account/")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly TestContext _context;

        private readonly IGetTokenService _tokenService;

        public PostController(TestContext context, IGetTokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Authorize] // Требуется аутентификация
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult CreatePost(CreatePostDto postDto)
        {
            // Получаем идентификатор авторизованного пользователя из токена
            string authorizationHeader = Request.Headers["Authorization"];
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);
            string userId = _tokenService.GetUserIdFromToken(bearerToken);
            var user = _context.Users.FirstOrDefault(u => u.id == userId);

            // Создаем новый объект поста на основе данных из запроса
            var post = new PostDto
            {
                id = Guid.NewGuid().ToString(),
                createTime = DateTime.UtcNow.ToString(),
                title = postDto.title,
                description = postDto.description,
                readingTime = postDto.readingTime,
                image = postDto.image,
                authorId = userId,
                author = user.fullName, // Подставьте имя автора, если оно доступно
                addressId = postDto.addressId,
                likes = 0,
                hasLike = false,
                commentsCount = 0,
                tags = postDto.tags
            };

            // Добавляем пост в контекст базы данных
            _context.Posts.Add(post);

            // Сохраняем изменения в базе данных
            _context.SaveChanges();

            // Возвращаем созданный пост
            return Ok(post);
        }
    }
}
