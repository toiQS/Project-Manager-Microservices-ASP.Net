using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PM.Shared.Dtos.auths;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PM.Shared.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ServiceResult<string> GenerateToken(string userId, string email, string role)
        {
            if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
            {
                return ServiceResult<string>.Error("Invalid user data");
            }
            try
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
                };
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
                var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: credentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return ServiceResult<string>.Success(tokenString);
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.FromException(ex);
            }
        }

    }
}
