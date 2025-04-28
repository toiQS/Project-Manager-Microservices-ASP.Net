using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PM.Shared.Dtos.auths;
using PM.Shared.Handle.Interfaces;
using System.Net.Http.Json;
using System.Security.Claims;

namespace PM.Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthController> _logger;
        private readonly IAPIService<string> _aPIService;
        private readonly string _baseUrl = "https://localhost:5000/api/auth";

        public AuthController(ILogger<AuthController> logger, IAPIService<string> aPIService)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _aPIService = aPIService;
        }

        /// <summary>
        /// Đăng nhập người dùng qua API Identity
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //return await ForwardToIdentityAsync("login", model);
            var response = await _aPIService.APIsPostAsync("api/auth/login", model);
            if (response.Status != ResultStatus.Success)
                return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Register model invalid");
                return BadRequest(ModelState);
            }

            return await ForwardToIdentityAsync("register", model);
        }

        /// <summary>
        /// Thay đổi mật khẩu (yêu cầu đăng nhập)
        /// </summary>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Change password model invalid");
                return BadRequest(ModelState);
            }

            return await ForwardToIdentityAsync("change-password", model);
        }

        /// <summary>
        /// Yêu cầu đặt lại mật khẩu (yêu cầu đăng nhập)
        /// </summary>
        [Authorize]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Forgot password model invalid");
                return BadRequest(ModelState);
            }

            return await ForwardToIdentityAsync("forgot-password", model);
        }

        /// <summary>
        /// Forward các request tới Identity API
        /// </summary>
        /// <param name="endpoint">Tên endpoint của Identity API</param>
        /// <param name="payload">Dữ liệu cần gửi</param>
        private async Task<IActionResult> ForwardToIdentityAsync(string endpoint, object payload)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/{endpoint}")
                {
                    Content = JsonContent.Create(payload)
                };
                request.Headers.Add("Accept", "application/json");

               

                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Forwarded to Identity API - {Endpoint} - SUCCESS", endpoint);
                    return Ok(content);
                }

                _logger.LogWarning("Forwarded to Identity API - {Endpoint} - FAILED: {Content}", endpoint, content);
                return BadRequest(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception when forwarding to Identity API - {Endpoint}", endpoint);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
