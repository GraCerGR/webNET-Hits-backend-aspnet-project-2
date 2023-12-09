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


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommunityFullDto), 200)]
        [ProducesResponseType(typeof(Response), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult GetPostId(string id)
        {
            var community = _context.Communities.SingleOrDefault(p => p.id == Guid.Parse(id));
            if (community == null)
            {
                return StatusCode(404, new { status = "error", message = $"Community with id='{id}' not found in  database" });
            }
            //post.tags = _context.Tags.Where(t => t.PostDtoid == id).ToList();

            var tagIds = _context.CommunityUsers.Where(pt => pt.communityId == community.id && pt.role == CommunityRole.Administrator).Select(pt => pt.userId).ToList();
            var users = _context.Users.Where(t => tagIds.Contains(t.id)).ToList();
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
    }
}
