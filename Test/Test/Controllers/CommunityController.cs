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
        [ProducesResponseType(typeof(AuthorDto), 200)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult GetCommunity()
        {
            var community = _context.Communities.ToList();
            var communities = community.Select(community => new CommunityDto
            {
                id = community.id,
                createDate = community.createDate,
                name = community.name,
                description = community.description,
                isClosed = community.isClosed,
                subscribersCount = community.subscribersCount,

            }).ToList();

            return Ok(communities);
        }
    }
}
