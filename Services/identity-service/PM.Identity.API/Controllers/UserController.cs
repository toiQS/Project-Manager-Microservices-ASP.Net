using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Interfaces;
using PM.Shared.Dtos.Users;

namespace PM.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        [HttpGet("get-user-by-email")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            _logger.LogInformation("GetUserByEmail request received for email: {Email}", email);
            var result = await _userService.GetUserByEmail(email);
            if (result.Status)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet("get-user-by-id")]
        public async Task<IActionResult> GetUserById(string id)
        {
            _logger.LogInformation("GetUserById request received for id: {Id}", id);
            var result = await _userService.GetUserById(id);
            if (result.Status)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPatch("update-user")]
        public async Task<IActionResult> UpdateUser(UpdateUserModel model)
        {
            _logger.LogInformation("UpdateUser request received for id: {Id}", model.Id);
            var result = await _userService.UpdateUser(model.Id, model.FirstName, model.LastName, model.UserName, model.AvatarPath);
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
}
