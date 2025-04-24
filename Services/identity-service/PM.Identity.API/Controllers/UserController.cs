using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Interfaces;
using PM.Shared.Dtos.auths;
using PM.Shared.Dtos.users;

namespace PM.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserHandle _userHandle;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserHandle userHandle, ILogger<UserController> logger)
        {
            _userHandle = userHandle;
            _logger = logger;
        }
        [HttpGet("get-user")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var result = await _userHandle.GetUser(userId);
            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Get user successfully");
                return Ok(result);
            }
            _logger.LogError(result.Message);
            return BadRequest(result);
        }
        [HttpPatch("pacth-user")]
        public async Task<IActionResult> PacthUser(string userId, [FromBody] UserPacthModel model)
        {
            var result = await _userHandle.PacthUserHandle(userId, model);
            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Pacth user successfully");
                return Ok(result);
            }
            _logger.LogError(result.Message);
            return BadRequest(result);
        }
    }
}
