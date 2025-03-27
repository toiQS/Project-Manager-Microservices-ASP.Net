using Microsoft.AspNetCore.Mvc;
using PM.EndPointAPIs.Model.Auth.Commands;

namespace PM.EndPointAPIs.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthController> _logger;
        private string _baseUri = "https://localhost:7200";
        public AuthController(IHttpContextAccessor httpContextAccessor, ILogger<AuthController> logger)
        {
            _httpClient = new HttpClient();
        }
        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginCommand model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login attempt.");
            }

            try
            {
                string loginEndpoint = _baseUri + "/login";

                using var request = new HttpRequestMessage(HttpMethod.Post, loginEndpoint)
                {
                    Content = JsonContent.Create(model)
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(result) ? Ok(result) : BadRequest("Empty response from authentication service.");
                }

                return BadRequest("Invalid login attempt.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt for user: {Email}", model.Email);
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Handles user registration by forwarding the request to an external authentication service.
        /// </summary>
        /// <param name="model">The registration request model containing user details.</param>
        /// <returns>Returns an HTTP response with the registration result.</returns>
        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid registration data.");
            }

            try
            {
                string registerEndpoint = _baseUri + "/register";

                using var request = new HttpRequestMessage(HttpMethod.Post, registerEndpoint)
                {
                    Content = JsonContent.Create(model)
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(result) ? Ok(result) : BadRequest("Empty response from registration service.");
                }

                return BadRequest("Registration failed. Please check your input and try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration attempt for user: {Email}", model.Email);
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
    }
}
