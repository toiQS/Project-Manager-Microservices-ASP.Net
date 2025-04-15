using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Interfaces;

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
    
}
