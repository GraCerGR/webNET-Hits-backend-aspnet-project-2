using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Controllers
{
    [Route("/api/account/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]

        public ActionResult<UserDto> Get()
        {
            UserDto user = new UserDto();

            return null;
        }

        [HttpGet("{id}")]

        public string Get(int id)
        {
            return $"{id}";
        }

        [HttpPost("{name}")]
        public string Post(string name)
        {
            return $"{name}";
        }

        private readonly TestContext _context;

        public TestController(TestContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(UserRegisterModel model)
        {
            try
            {
                // Создаем новый объект пользователя на основе данных из модели UserRegisterModel
                UserDto user = new UserDto
                {
                    id = Guid.NewGuid().ToString(), // Пример значения для id
                    createTime = DateTime.Now.ToString(), // Пример значения для createTime
                    fullname = model.fullname,
                    birthDate = model.birthDate,
                    gender = model.gender,
                    email = model.email,
                    phoneNumber = model.phoneNumber
                };

                // Добавляем пользователя в контекст базы данных
                _context.Users.Add(user);

                // Сохраняем изменения в базе данных
                await _context.SaveChangesAsync();

                return Ok();

            }
            catch (Exception ex)
            {
                // Обработка ошибок, если возникла ошибка при добавлении пользователя
                return StatusCode(500, "Произошла ошибка при добавлении пользователя.");
            }
        }
    }
}
