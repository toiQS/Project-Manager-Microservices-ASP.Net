using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Interfaces;
using PM.Shared.Dtos;
using PM.Shared.Dtos.auths;

namespace PM.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthHandle _authHandle;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthHandle authHandle, ILogger<AuthController> logger)
        {
            _authHandle = authHandle;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authHandle.RegisterHandleAsync(model);
            if (result.Status == ResultStatus.Success)
            {
                return Ok(result);
            }
            else
            {
                _logger.LogError(result.Message);
                return BadRequest(result);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authHandle.LoginHandleAsync(model);
            if (result.Status == ResultStatus.Success)
            {
                return Ok(result);
            }
            else
            {
                _logger.LogError(result.Message);
                return BadRequest(result);
            }
        }
    }
}
