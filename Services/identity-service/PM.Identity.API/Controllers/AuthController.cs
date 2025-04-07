using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Implements.Flows;
using PM.Shared.Dtos.Auths;

namespace PM.Identity.API.Controllers
{
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
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var result = await _authFlow.Login(loginModel);
            if (result.Status)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            var result = await _authFlow.Register(registerModel);
            if (result.Status)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }
    }
}
