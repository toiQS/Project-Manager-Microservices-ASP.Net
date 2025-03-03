using Microsoft.AspNetCore.Mvc;
using PM.Models.auths;
using System.Net.Http.Json;

namespace PM.Controllers
{
    //[Route("api/[controller]")]
    [Route("/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7203") };
        }

        /// <summary>
        /// Handles user login.
        /// </summary>
        /// <param name="model">Login credentials.</param>
        /// <returns>JWT token or error response.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/login", model);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                return BadRequest("Invalid login attempt.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Handles user registration.
        /// </summary>
        /// <param name="model">User registration data.</param>
        /// <returns>Success or error response.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/register", model);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                return BadRequest("Registration failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration attempt.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Logs out the user.
        /// </summary>
        /// <param name="token">JWT token.</param>
        /// <returns>Success or error response.</returns>
        [HttpPost("log-out")]
        public async Task<IActionResult> LogOut([FromBody] string token)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/logout", token );
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                return BadRequest("Logout failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout attempt.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}