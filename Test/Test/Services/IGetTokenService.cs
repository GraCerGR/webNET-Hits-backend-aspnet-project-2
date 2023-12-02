using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Test.Services
{
    public interface IGetTokenService
    {
        string GetUserIdFromToken(string token);
    }

    public class GetTokenService : IGetTokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetTokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            string userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            return userId;
        }
    }
}
