using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Namotion.Reflection;
using PM.Shared.Dtos;
using System.Security.Claims;

namespace PM.Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpContextAccessor _content;
        private readonly HttpClient _httpClient;
        private string _baseUrl = "https://localhost:5000";
        public AuthController()
        {
            _httpClient = new HttpClient();
            _content = new HttpContextAccessor();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/auth/login")
            {
                Content = JsonContent.Create(model),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "Accept", "application/json" }
                }
            };
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
               return Ok(token);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
        }
        [Authorize]  // Đảm bảo người dùng đã được xác thực
        [HttpGet("userinfo")]
        public IActionResult GetUserInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("User information is missing.");
            }

            // Bạn có thể lấy thông tin người dùng từ database hoặc trả về dữ liệu theo yêu cầu
            return Ok(new { UserId = userId, UserEmail = userEmail });
        }

    }
}
