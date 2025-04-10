using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PM.EndPoint.API.Flows.Interfaces;
using PM.Shared.Dtos.Auths;

namespace PM.EndPoint.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthFlow _authFlow;
        public AuthController(IAuthFlow authFlow)
        {
            _authFlow = authFlow;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                var response = await _authFlow.HandleLogin(loginModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("demo"), Authorize]
        public async Task<IActionResult> Demo()
        {
            try
            {
                var response = await _authFlow.HandleDemo();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
