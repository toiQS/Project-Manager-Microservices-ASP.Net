using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Models.auths;

namespace PM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthController> _logger;
        public AuthController(ILogger<AuthController> loggers)
        {
            _logger = loggers;
            var url = new Uri("https://localhost:7203");
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = url;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var request = await _httpClient.PostAsJsonAsync("/login", model);
            if (request.IsSuccessStatusCode)
            {
                var response = await request.Content.ReadAsStringAsync();
                return Ok(response);
            }
            return BadRequest();
        }
    }
}
