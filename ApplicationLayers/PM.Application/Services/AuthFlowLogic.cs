using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PM.Application.DTOs.Users;
using PM.Application.Features.Auth.Commands;
using PM.Application.Interfaces;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PM.Application.Services
{
    public class AuthFlowLogic : ControllerBase, IAuthFlowLogic
    {
        private readonly IAuthServices _authServices;
        private readonly ILogger<AuthFlowLogic> _logger;
        private readonly IUserServices _userServices;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenServices _refreshTokenServices;

        public AuthFlowLogic(IAuthServices authServices, ILogger<AuthFlowLogic> logger, IUserServices userServices, IConfiguration configuration, IRefreshTokenServices refreshTokenServices)
        {
            _authServices = authServices;
            _logger = logger;
            _userServices = userServices;
            _configuration = configuration;
            _refreshTokenServices = refreshTokenServices;
        }

        public async Task<IActionResult> Login(LoginCommand loginCommand)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[Login] Invalid model state.");
                return BadRequest("Invalid request data.");
            }

            var login = await _authServices.Login(loginCommand.Email, loginCommand.Password);
            if (!login.Status)
            {
                _logger.LogWarning("[Login] Authentication failed for Email={Email}", loginCommand.Email);
                return BadRequest(login.Message);
            }

            var user = await _authServices.GetUserByEmail(loginCommand.Email);
            if (!user.Status)
            {
                _logger.LogWarning("[Login] User not found for Email={Email}", loginCommand.Email);
                return BadRequest(user.Message);
            }

            var userDetailDTO = new UserDetailDTO
            {
                UserId = user.Data!.Id,
                Avata = user.Data.AvatarPath,
                FullName = user.Data.FullName,
                UserName = user.Data.UserName ?? "Empty",
                Email = loginCommand.Email,
                Phone = user.Data.PhoneNumber ?? "Empty"
            };

            var token = GenerateToken(userDetailDTO);
            if (!token.Status)
            {
                _logger.LogError("[Login] Token generation failed for UserId={UserId}", user.Data.Id);
                return BadRequest(token.Message);
            }

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddMinutes(30),
                IsRevoke = false,
                UserId = user.Data.Id,
                Token = token.Data!
            };

            var addToken = await _refreshTokenServices.AddAsync(refreshToken);
            if (!addToken.Status)
            {
                _logger.LogError("[Login] Failed to store refresh token for UserId={UserId}", user.Data.Id);
                return BadRequest("Failed to store refresh token.");
            }

            _logger.LogInformation("[Login] User logged in successfully: UserId={UserId}", user.Data.Id);
            return Ok(new { Token = token.Data, RefreshToken = refreshToken.Token });
        }

        public async Task<IActionResult> Register(RegisterCommand command)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[Register] Invalid model state.");
                return BadRequest("Invalid request data.");
            }

            var register = await _authServices.Register(command.Email, command.Username, command.Password);
            if (!register.Status)
            {
                _logger.LogError("[Register] Registration failed for Email={Email}", command.Email);
                return BadRequest(register.Message);
            }

            var user = await _authServices.GetUserByEmail(command.Email);
            if (!user.Status)
            {
                _logger.LogWarning("[Register] User retrieval failed for Email={Email}", command.Email);
                return BadRequest(user.Message);
            }

            var addToRoleCustom = await _authServices.AddRoleCustomer(user.Data!);
            if (!addToRoleCustom.Status)
            {
                _logger.LogError("[Register] Failed to assign role for UserId={UserId}", user.Data!.Id);
                return BadRequest("Failed to assign user role.");
            }

            _logger.LogInformation("[Register] User registered successfully: UserId={UserId}", user.Data!.Id);
            return Ok("Registration successful.");
        }

        public ServicesResult<string> GenerateToken(UserDetailDTO user)
        {
            try
            {
                var claims = new[]
                {
                    new Claim("Id", user.UserId),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("FullName", user.FullName),
                    new Claim("Phone", user.Phone),
                    new Claim("Avata", user.Avata)
                };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokenOptions = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: signinCredentials
                );

                return ServicesResult<string>.Success(new JwtSecurityTokenHandler().WriteToken(tokenOptions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GenerateToken] Token generation failed.");
                return ServicesResult<string>.Failure("Token generation error.");
            }
        }
    }
}