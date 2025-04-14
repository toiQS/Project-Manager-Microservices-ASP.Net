using Microsoft.AspNetCore.Mvc;
using PM.Shared.Dtos.Auths;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using YamlDotNet.Serialization.NodeTypeResolvers;

namespace PM.EndPoint.API.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private string _baseUrl = "https://localhost:8000/api/auth/";
        public AuthController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = new HttpClient();
        }
        [HttpPost("login")]
        public async Task<IActionResult> HandleLogin([FromBody] LoginModel model)
        {
            var client = new HttpClient();
           var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "login")
           {
               Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
           };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                return Ok(token);
            }
            else
            {
                return BadRequest("Login failed");
            }
        }
        [HttpGet("demo")]
        public IActionResult demo()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null)
            {
                return Ok(new
                {
                    Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Email = user.FindFirst(ClaimTypes.Email)?.Value,
                    UserName = user.FindFirst(ClaimTypes.Name)?.Value
                })
                ;
            }
            return Unauthorized("User not authenticated");
        }
    }
}
