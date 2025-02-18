using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Models.users;
using PM.Infrastructers.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PM.Infrastructers.Implements
{
    public class JwtServices : IJwtServices
    {
        private readonly IConfiguration _configuration;
        public JwtServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ServicesResult<string> GenerateToken(DetailAppUser user)
        {
            try
            {
                if (user is null)
                {
                    return ServicesResult<string>.Failure("User not found");
                }

                var claims = new[]
                {
                    new Claim("Id", user.UserId),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("FullName", user.FullName),
                    new Claim("Phone", user.Phone),
                    new Claim("Avata", user.Avata)
                };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);
                var tokenOptions = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return ServicesResult<string>.Success(tokenString);
            }
            catch (Exception ex)
            {
                return ServicesResult<string>.Failure(ex.Message);
            }
        }


        public ServicesResult<DetailAppUser> ParseToken(string token)
        {
            if(token is null)
            {
                return ServicesResult<DetailAppUser>.Failure("Token not found");
            }
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
                {
                    return ServicesResult<DetailAppUser>.Failure("Invalid token");
                }
                var user = new DetailAppUser
                {
                    UserId = principal.FindFirst(ClaimTypes.NameIdentifier).Value ?? "Empty",
                    Role = principal.FindFirst(ClaimTypes.Role).Value ?? "Empty",
                    UserName = principal.FindFirst(ClaimTypes.Name).Value ?? "Empty",
                    Email = principal.FindFirst(ClaimTypes.Email).Value ?? "Empty",
                    FullName = principal.FindFirst("FullName").Value ?? "Empty",
                    Phone = principal.FindFirst("Phone").Value ?? "Empty",
                    Avata = principal.FindFirst("Avata").Value ?? "Empty"
                };
                return ServicesResult<DetailAppUser>.Success(user);
            }
            catch(Exception ex)
            {
                return ServicesResult<DetailAppUser>.Failure(ex.Message);
            }
        }
    }
}
