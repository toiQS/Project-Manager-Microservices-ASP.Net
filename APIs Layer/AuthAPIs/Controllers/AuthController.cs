using Microsoft.AspNetCore.Mvc;
using PM.Application.Interfaces;
using PM.Domain.Models.auths;

namespace AuthAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthLogic authLogic, IHttpContextAccessor httpContextAccessor, ILogger<AuthController> logger)
        {
            _authLogic = authLogic;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            try
            {
                var response = await _authLogic.Login(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in Login");
            }
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel request)
        {
            try
            {
                var response = await _authLogic.Register(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Register");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in Register");
            }
        }
    }
}
 