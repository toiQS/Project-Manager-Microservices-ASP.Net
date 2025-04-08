using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Shared.Dtos.Auths;
using System.Security.Claims;

namespace PM.EndPoint.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthController> _logger;
        private string _baseUrl = "https://localhost:8000";
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthController(ILogger<AuthController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/auth/login")
            {
                Content = JsonContent.Create(model)
            };
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(new { error });
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{_baseUrl}/api/auth/register"),
                Method = HttpMethod.Post,
                Content = JsonContent.Create(model),
                Headers =
                {
                    { "Accept", "application/json" }
                }
            };
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(new { error });
            }
        }
        [HttpPost("logout"), Authorize]
        public Task<IActionResult> LogOut()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Task.FromResult<IActionResult>(Ok(userId));
        }
    }
}
