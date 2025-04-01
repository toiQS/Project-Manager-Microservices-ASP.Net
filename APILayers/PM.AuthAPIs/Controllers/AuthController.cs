using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Application.Contracts.Interfaces;
using PM.Application.Features.Auth.Commands;

namespace PM.AuthAPIs.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthFlowLogic _flowLogic;
        public AuthController(IAuthFlowLogic flowLogic)
        {
            _flowLogic = flowLogic;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand loginCommand)
        {
            return await _flowLogic.Login(loginCommand);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand loginCommand)
        {
            return await _flowLogic.Register(loginCommand);
        }
    }
}
