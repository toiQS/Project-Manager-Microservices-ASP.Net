using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Interfaces.Flows;
using PM.Shared.Dtos.Auths;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthFlow _authFlow;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthFlow authFlow, ILogger<AuthController> logger)
    {
        _authFlow = authFlow;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var result = await _authFlow.HandleSignInAsync(model);
        return result.Status
            ? Ok(new { token = result.Data })
            : BadRequest(new { error = result.Message });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var result = await _authFlow.HandleRegisterUserAsync(model);
        return result.Status
            ? Ok(new { message = result.Data })
            : BadRequest(new { error = result.Message });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest(new { error = "Token is required" });

        token = token.Replace("Bearer ", "");
        var result = await _authFlow.HandleSignOutAsync(token);
        return result.Status
            ? Ok(new { message = result.Data })
            : BadRequest(new { error = result.Message });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
    {
        var result = await _authFlow.HandleChangePasswordAsync(model);
        return result.Status
            ? Ok(new { message = result.Data })
            : BadRequest(new { error = result.Message });
    }
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { message = "Test successful" });
    }
}
