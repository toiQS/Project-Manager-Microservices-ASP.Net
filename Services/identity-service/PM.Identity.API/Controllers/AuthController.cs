using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Dtos.auths;
using PM.Identity.Application.Interfaces;
using PM.Shared.Dtos.Models;

namespace PM.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthHandle _authHandle;
        private readonly ILogger<AuthController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:5000";

        public AuthController(IAuthHandle authHandle, ILogger<AuthController> logger)
        {
            _authHandle = authHandle;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// API đăng ký tài khoản người dùng mới
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Register.");
                return BadRequest("Dữ liệu đầu vào không hợp lệ.");
            }

            var result = await _authHandle.RegisterHandleAsync(model);

            if (result.Status == ResultStatus.Success)
            {
                return Ok(result);
            }

            _logger.LogError("Register failed: {Message}", result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// API đăng nhập
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Login.");
                return BadRequest("Dữ liệu đầu vào không hợp lệ.");
            }

            var result = await _authHandle.LoginHandleAsync(model);

            if (result.Status == ResultStatus.Success)
            {
                return Ok(result);
            }

            _logger.LogError("Login failed: {Message}", result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// API thay đổi mật khẩu
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in ChangePassword.");
                return BadRequest("Dữ liệu đầu vào không hợp lệ.");
            }

            var result = await _authHandle.ChangePasswordHandle(model);

            if (result.Status == ResultStatus.Success)
            {
                return Ok(result);
            }

            _logger.LogError("Change password failed: {Message}", result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// API quên mật khẩu
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in ForgotPassword.");
                return BadRequest("Dữ liệu đầu vào không hợp lệ.");
            }

            var result = await _authHandle.ForgotPasswordHandle(model);


            _logger.LogError("Forgot password failed: {Message}", result.Message);
            return BadRequest(result);
        }

       
    }
}
