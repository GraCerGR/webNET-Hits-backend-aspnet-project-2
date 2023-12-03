﻿using Microsoft.AspNetCore.Html;
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

namespace Test.Controllers
{
    [Route("/api/account/")]
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
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult CreatePost(CreatePostDto postDto)
        {
            // Получаем идентификатор авторизованного пользователя из токена
            string authorizationHeader = Request.Headers["Authorization"];
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
            string userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var user = _context.Users.FirstOrDefault(u => u.id == userId);
            var tagIds = postDto.tags; // Получение массива id тегов из postDto
            var tags = new List<TagDto>(); // Создание списка для хранения объектов тегов
            foreach (var tagId in tagIds)
            {
                var existingTag = _context.Tags.FirstOrDefault(t => t.id == tagId); // Поиск существующего тега по id
                if (existingTag != null)
                {
                    tags.Add(existingTag); // Использование существующего тега
                }
                else
                {
                    ModelState.AddModelError("$.tags[0]", $"The tag with id '{tagId}' was not found.");
                }
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
                id = Guid.NewGuid().ToString(),
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
                tags = tags,
            };

            // Добавляем пост в контекст базы данных
            _context.Posts.Add(post);
            // Сохраняем изменения в базе данных
            _context.SaveChanges();

            // Возвращаем созданный пост
            return Ok(post.id);
        }
    }
}