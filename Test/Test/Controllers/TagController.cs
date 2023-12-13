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
    [Route("/api/")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly TestContext _context; // Замените YourDbContext на ваш контекст базы данных

        public TagController(TestContext context)
        {
            _context = context;
        }

        [HttpGet("tag")]
        [ProducesResponseType(typeof(TagDto), 200)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<ActionResult> GetTags()
        {
            try
            {
                object tags = _context.Tags.ToList(); // Получение всех элементов таблицы Tags

                return Ok(tags);
            }
            catch (Exception ex)
            {
                // Обработка ошибки
                return StatusCode(500,"InternalServerError");
            }
        }
    }
}
