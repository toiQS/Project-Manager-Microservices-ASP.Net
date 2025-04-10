using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Interfaces;
using PM.Shared.Dtos.Auths;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authServices;
    private readonly ILogger<AuthController> _logger;
    public AuthController(IAuthService authServices, ILogger<AuthController> logger)
    {
        _authServices = authServices;
        _logger = logger;
    }
    [HttpPost("login")]
    public async Task<IActionResult> HandleLogin([FromBody] LoginModel request)
    {
        _logger.LogInformation("Login request received for email: {Email}", request.Email);
        var result = await _authServices.SignInAsync(request.Email, request.Password);
       if(result.Status)
        {
            return Ok(result);
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
}
