using Microsoft.AspNetCore.Authorization;
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
    }
}
