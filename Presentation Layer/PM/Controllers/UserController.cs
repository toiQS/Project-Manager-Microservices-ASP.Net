using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using PM.Models.users;

namespace PM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserController> _logger;
        private string _baseUrl = "https://localhost:7203";
        public UserController(ILogger<UserController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }
        [HttpGet("/user/user-detail-token")]
        public async Task<IActionResult> UserTokem(string token)
        {
            return await ForwardRequestAsync(HttpMethod.Get, $"/user/user-detail-token?token={token}");
        }
        [HttpGet("/user/another-user-detail-identity")]
        public async Task<IActionResult> AnotherUserDetailIdentity(string identity)
        {
            return await ForwardRequestAsync(HttpMethod.Get, $"/user/another-user-detail-identity?identity={identity}");
        }
        [HttpPut("/user/update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {
            return await ForwardRequestAsync(HttpMethod.Put, "/user/update-user", model);
        }
        [HttpPut("/user/update-avatar")]
        public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarModel model)
        {
            return await ForwardRequestAsync(HttpMethod.Put, "/user/update-avatar", model);
        }
        [HttpPut("/user/change-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordModel model)
        {
            return await ForwardRequestAsync(HttpMethod.Put, "/user/change-password", model);
        }
        private async Task<IActionResult> ForwardRequestAsync(HttpMethod method, string path, object? data = null)
        {
            try
            {
                using var request = new HttpRequestMessage(method, _baseUrl + path);
                if (data != null)
                {
                    request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                }
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode ? Ok(await response.Content.ReadAsStringAsync()) : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request for {Path}", path);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
