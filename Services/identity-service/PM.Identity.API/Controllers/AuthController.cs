using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PM.Identity.Application.Interfaces;
using PM.Identity.Domain.Entities;
using PM.Shared.Dtos;
using PM.Shared.Dtos.Auths;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authServices;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AuthController(IAuthService authServices, ILogger<AuthController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _authServices = authServices;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }
    [HttpPost("login")]
    public async Task<IActionResult> HandleLogin([FromBody] LoginModel request)
    {
        _logger.LogInformation("Login request received for email: {Email}", request.Email);
        var result = await _authServices.SignInAsync(request.Email, request.Password);
        if (result.Status)
        {
            var user = new User()
            {
                UserName = "akai",
                Email = request.Email,
                Id = Guid.NewGuid().ToString()
            };
            var token = GenerateToken(user);
            return Ok(token);
        }
        else
        {
            return BadRequest(result);
        }
    }
   
    [HttpPost("register")]
    public async Task<IActionResult> HandleRegister(RegisterModel register)
    {
        _logger.LogInformation("Register request received for email: {Email}", register.Email);
        var result = await _authServices.RegisterUserAsync(register.Email, register.UserName, register.Password);
        if (result.Status)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }
    [HttpPost("logout")]
    public async Task<IActionResult> HandleLogout()
    {
        _logger.LogInformation("Logout request received");
        var result = await _authServices.SignOutAsync();
        if (result.Status)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }
    [HttpPost("change-password")]
    public async Task<IActionResult> HandleChangePassword([FromBody] ChangePasswordModel changePassword)
    {
        _logger.LogInformation($"Change password Email:{changePassword.Email}");
        var result = await _authServices.ChangeUserPasswordAsync(changePassword.Email, changePassword.OldPassword, changePassword.NewPassword);
        if (result.Status)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }
    [HttpGet("get-user")]       
    public IActionResult GetUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user.Identity.IsAuthenticated)
        {
            return Ok(new
            {
                Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Email = user.FindFirst(ClaimTypes.Email)?.Value,
                UserName = user.FindFirst(ClaimTypes.Name)?.Value
            });
        }
        return Unauthorized("User not authenticated");
    }

    private string GenerateToken(User user)
    {
        try
        {
            if (user is null)
            {
                return "User not found";
            }

            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.Name, user.UserName!),
                };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
            signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }
        catch (Exception ex)
        {
            return "Token generation failed";
        }
    }
}
