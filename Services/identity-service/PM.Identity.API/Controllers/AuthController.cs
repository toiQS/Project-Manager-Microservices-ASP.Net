using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Interfaces;
using PM.Shared.Dtos.auths;
using PM.Shared.Dtos.tracking;
using System.Net.Http.Json;
using System.Threading.Tasks;

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
                await LogTrackingAsync($"A user registered an account with email: {model.Email}");
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
                await LogTrackingAsync($"A user logged in with email: {model.Email}");
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
                await LogTrackingAsync($"User changed password for email: {model.Email}");
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

            if (result.Status == ResultStatus.Success)
            {
                await LogTrackingAsync($"User requested password reset for email: {model.Email}");
                return Ok(result);
            }

            _logger.LogError("Forgot password failed: {Message}", result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Gửi log tracking đến API tracking
        /// </summary>
        private async Task LogTrackingAsync(string actionName)
        {
            var context = new AddTrackingModel
            {
                ProjectId = string.Empty,
                UserId = string.Empty,
                ActionName = actionName
            };

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_baseUrl}/api/tracking/add-tracking-log"),
                    Method = HttpMethod.Post,
                    Content = JsonContent.Create(context)
                };

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Tracking failed: {Response}", content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during tracking call.");
            }
        }
    }
}
