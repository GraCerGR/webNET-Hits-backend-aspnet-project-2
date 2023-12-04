using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Models;
using Test.Models.DTO;

namespace Test.Controllers
{
    [Route("/api/author/")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly TestContext _context;

        public AuthorController(TestContext context)
        {
            _context = context;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(AuthorDto), 200)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult GetAuthors()
        {
            var users = _context.Users.ToList();
            var authors = users.Select(user => new AuthorDto
            {
                fullName = user.fullName,
                birthDate = user.birthDate,
                gender = user.gender,
                posts = user.posts,
                likes = user.likes,
                created = user.createTime
            }).ToList();

            return Ok(authors);
        }
    }
}
