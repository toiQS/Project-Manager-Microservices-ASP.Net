using Microsoft.AspNetCore.Mvc;
using PM.Application.Contracts.Interfaces;
using PM.Application.Features.Users.Commands;

namespace PM.ProjectManagementAPIs.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserFlowLogic _flowLogic;
        public UserController(IUserFlowLogic flowLogic)
        {
            _flowLogic = flowLogic;
        }
        [HttpPatch("update-info")]
        public async Task<IActionResult> PatchUserAsync(PacthUserCommand command)
        {
            return await _flowLogic.PatchUserAsync(command);
        }
        [HttpGet("detail-user-info")]
        public async Task<IActionResult> DetailUser(string userId)
        {
            return await _flowLogic.DetailUser(userId);
        }
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUserAsync(PacthUserCommand command)
        {
            return await _flowLogic.UpdateUserAsync(command);
        }
    }
}
