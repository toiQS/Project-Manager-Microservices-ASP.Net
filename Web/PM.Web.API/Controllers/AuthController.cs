using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PM.Shared.Dtos.auths;
using PM.Shared.Handle.Interfaces;
using System.Security.Claims;

namespace PM.Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAPIService<string> _aPIService;

        public AuthController(ILogger<AuthController> logger, IAPIService<string> aPIService)
        {
            _logger = logger;
            _aPIService = aPIService;
        }

        [HttpPost("login")]
        public Task<IActionResult> Login([FromBody] LoginModel model) => PostToIdentityAsync("login", model);

        [HttpPost("register")]
        public Task<IActionResult> Register([FromBody] RegisterModel model) => PostToIdentityAsync("register", model);

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(string newPassword, string oldPassword)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null) return BadRequest("User is not authorize");
            var model = new ChangePasswordModel()
            {
                Email = userEmail,
                OldPassword = oldPassword,  
                NewPassword = newPassword
            };
            return await PostToIdentityAsync("change-password", model);
        }

        [Authorize]
        [HttpPost("forgot-password")]
        public Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model) => PostToIdentityAsync("forgot-password", model);

        /// <summary>
        /// Hàm xử lý chung cho các post auth
        /// </summary>
        private async Task<IActionResult> PostToIdentityAsync(string endpoint, object model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for {Endpoint}", endpoint);
                return BadRequest(ModelState);
            }

            var response = await _aPIService.APIsPostAsync($"api/auth/{endpoint}", model);
            if (response.Status != ResultStatus.Success)
            {
                _logger.LogWarning("API call failed for {Endpoint}", endpoint);
                return BadRequest(response);
            }

            _logger.LogInformation("API call succeeded for {Endpoint}", endpoint);
            return Ok(response);
        }
    }
}
