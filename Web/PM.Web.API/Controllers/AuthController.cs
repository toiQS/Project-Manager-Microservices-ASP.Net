using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Shared.Dtos;

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
        [HttpGet("test")]
        public IActionResult demo()
        {
            var user = _content.HttpContext.User;
            if(user.Identity.IsAuthenticated)
            {
                return Ok(new { message = "Hello from Identity API" });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
