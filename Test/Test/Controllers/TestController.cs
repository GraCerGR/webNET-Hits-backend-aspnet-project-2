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
        private readonly TestContext _regContext;

        public TestController(TestContext context, TestContext regContext)
        {
            _context = context;
            _regContext = regContext;
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

/*                UserRegisterModel userReg = new UserRegisterModel
                {
                    fullname = model.fullname,
                    password = model.password,
                    email = model.email,
                    birthDate = model.birthDate,
                    gender = model.gender,
                    phoneNumber = model.phoneNumber
                };*/


                // Добавляем пользователя в контекст базы данных UserRegisterContext
                _regContext.UserRegisterModels.Add(model);

                // Сохраняем изменения в базе данных UserRegisterContext
                await _regContext.SaveChangesAsync();



                // Генерация токена
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes("1234567890123456789012345678901234567890");
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    NotBefore = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = "HITS",
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                new Claim(ClaimTypes.Name, user.id) // Используем id пользователя в качестве имени в токене
                    })
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // Возвращение токена в качестве ответа
                return Ok(tokenString);
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если возникла ошибка при добавлении пользователя
                return StatusCode(500, "Произошла ошибка при добавлении пользователя.");
            }
        }


        [HttpPost("login")]
        public IActionResult Login(LoginCredentials model)
        {
            // Проверяем наличие пользователя с указанным email в базе данных
            var user = _regContext.UserRegisterModels.FirstOrDefault(u => u.email == model.email);
            if (user == null)
            {
                // Если пользователя с указанным email не существует, возвращаем ошибку
                return StatusCode(400, new { status = "error", message = "Неверный email или пароль." });
            }

            // Проверяем правильность введенного пароля
            if (user.password != model.password)
            {
                // Если пароль неверный, возвращаем ошибку
                return StatusCode(400, new { status = "error", message = "Неверный email или пароль." });
            }

            var userDto = _context.Users.FirstOrDefault(u => u.email == user.email);

            // Генерируем токен для пользователя
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("1234567890123456789012345678901234567890");
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "HITS",
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, userDto.id) // Используем email пользователя в качестве имени в токене
                })
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Возвращаем токен в случае успеха
            return Ok(new { token = tokenString });
        }


        /*        [HttpGet("name")]
                public IActionResult Login(string name)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes("1234567890123456789012345678901234567890");

                    var tokenDescriptor = new SecurityTokenDescriptor()
                    {
                        NotBefore = DateTime.UtcNow,
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials =
                        new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                        Issuer = "HITS",
                        Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, name)
                    })
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    return Ok(tokenHandler.WriteToken(token));
                }*/


    }
}
