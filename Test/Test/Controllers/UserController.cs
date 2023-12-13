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
    public class UserController : ControllerBase
    {

        private readonly TestContext _context;

        public UserController(TestContext context, TestContext regContext)
        {
            _context = context;
        }


        [HttpPost("register")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<ActionResult> register(UserRegisterModel model)
        {
            try
            {
                // Создаем новый объект пользователя на основе данных из модели UserRegisterModel
                User user = new User
                {
                    id = Guid.NewGuid(), // Пример значения для id
                    createTime = DateTime.Now.ToString(), // Пример значения для createTime
                    fullName = model.fullName,
                    birthDate = model.birthDate,
                    gender = model.gender,
                    email = model.email,
                    phoneNumber = model.phoneNumber,
                    password = model.password,
                };

                // Добавляем пользователя в контекст базы данных
                _context.Users.Add(user);

                // Сохраняем изменения в базе данных
                await _context.SaveChangesAsync();


                // Генерация токена
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes("1234567890123456789012345678901234567890");
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    NotBefore = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = "HITS",
                    Audience = "HITS",
                    Subject = new ClaimsIdentity(new Claim[]
                    {
            new Claim(ClaimTypes.Name, user.id.ToString()) // Используем email пользователя в качестве имени в токене
                    })
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // Возвращаем токен в случае успеха
                return Ok(new { token = tokenString });
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если возникла ошибка при добавлении пользователя
                return StatusCode(500, new Response { message = ex.Message });
            }
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<ActionResult> login(LoginCredentials model)
        {
            try
            {
            // Проверяем наличие пользователя с указанным email в базе данных
            var user = _context.Users.FirstOrDefault(u => u.email == model.email);
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

            //var userDto = _context.Users.FirstOrDefault(u => u.email == user.email);

            // Генерируем токен для пользователя
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("1234567890123456789012345678901234567890");
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "HITS",
                Audience = "HITS",
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.id.ToString()) // Используем email пользователя в качестве имени в токене
                })
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Возвращаем токен в случае успеха
            return Ok(new { token = tokenString });
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если возникла ошибка при входе пользователя
                return StatusCode(500, new Response { message = ex.Message });
            }
        }

        [HttpGet("profile")]
        [Authorize] // Требуется аутентификация
        [ProducesResponseType(typeof(UserDto),200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(Response), 500)]
        public ActionResult<UserDto> GetProfile()
        {
            try
            {
            // Получаем значение заголовка "Authorization"
            string authorizationHeader = Request.Headers["Authorization"];

            // Извлекаем токен Bearer из значения заголовка
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);


            var tokenHandler = new JwtSecurityTokenHandler();

            // Расшифровываем и проверяем токен
            var jwtToken = tokenHandler.ReadJwtToken(bearerToken);

            // Извлекаем идентификатор пользователя из полезной нагрузки токена
            string userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;

            // Ищем пользователя в базе данных по идентификатору
            var user = _context.Users.FirstOrDefault(u => u.id.ToString() == userId);


            if (user == null)
            {
                // Если пользователь не найден, возвращаем ошибку
                return NotFound();
            }

            // Создаем объект UserDto с данными профиля пользователя
            var userProfile = new UserDto
            {
                fullName = user.fullName,
                birthDate = user.birthDate,
                gender = user.gender,
                email = user.email,
                phoneNumber = user.phoneNumber,
                id = user.id,
                createTime = user.createTime
            };

            // Возвращаем данные профиля пользователя
            return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { message = ex.Message });
            }
        }

        [HttpPut("profile")]
        [Authorize] // Требуется аутентификация
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<ActionResult> UpdateProfile(UserEditModel updatedUserDto)
        {
            try
            {
            // Получаем идентификатор пользователя из токена
            // Получаем значение заголовка "Authorization"
            string authorizationHeader = Request.Headers["Authorization"];

            // Извлекаем токен Bearer из значения заголовка
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);


            var tokenHandler = new JwtSecurityTokenHandler();

            // Расшифровываем и проверяем токен
            var jwtToken = tokenHandler.ReadJwtToken(bearerToken);

            // Извлекаем идентификатор пользователя из полезной нагрузки токена
            string userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;

            // Ищем пользователя в базе данных по идентификатору
            var user = _context.Users.FirstOrDefault(u => u.id.ToString() == userId);

            if (user == null)
            {
                // Если пользователь не найден, возвращаем ошибку
                return NotFound();
            }

            // Обновляем данные пользователя на основе полученного объекта UserDto
            user.email = updatedUserDto.email;
            user.fullName = updatedUserDto.fullName;
            user.birthDate = updatedUserDto.birthDate;
            user.gender = updatedUserDto.gender;
            user.phoneNumber = updatedUserDto.phoneNumber;

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            // Возвращаем обновленные данные профиля пользователя
            return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize] // Требуется аутентификация
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<ActionResult> Logout()
        {
            try
            {
            string authorizationHeader = Request.Headers["Authorization"];
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(bearerToken);
            string userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var user = _context.Users.FirstOrDefault(u => u.id.ToString() == userId);

            //Логика удаления токена

            return StatusCode(200, new Response { status = null, message = "Logged Out" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { message = ex.Message });
            }
        }

    }
}
