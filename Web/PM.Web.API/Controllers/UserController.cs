using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PM.Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:5000/api/user";
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        [HttpGet("detail-user")]
        public async Task<IActionResult> GetUserAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var endPoint = $"get-user?userId={userId}";
            return await ForwardToIdentityAsync(endPoint, null, HttpMethod.Get);
        }

        /// <summary>
        /// Forward các request tới Identity API
        /// </summary>
        /// <param name="endpoint">Tên endpoint của Identity API</param>
        /// <param name="payload">Dữ liệu cần gửi</param>
        private async Task<IActionResult> ForwardToIdentityAsync(string endpoint, object payload, HttpMethod method)
        {
            try
            {
                var request = new HttpRequestMessage(method, $"{_baseUrl}/{endpoint}")
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
