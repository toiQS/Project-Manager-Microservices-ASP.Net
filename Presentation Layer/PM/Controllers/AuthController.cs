using Microsoft.AspNetCore.Mvc;
using PM.Models.auths;

namespace PM.Controllers
{
    //[Route("api/[controller]")]
    [Route("")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthController> _logger;
        private string _baseUrl = "https://localhost:7203";
        public AuthController(ILogger<AuthController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        /// <summary>
        /// Handles user login by forwarding the request to an external authentication service.
        /// </summary>
        /// <param name="model">The login request model containing user credentials.</param>
        /// <returns>Returns an HTTP response with the authentication result.</returns>
        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login attempt.");
            }

            try
            {
                string loginEndpoint = _baseUrl + "/login";

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
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid registration data.");
            }

            try
            {
                string registerEndpoint = _baseUrl + "/register";

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


        /// <summary>
        /// Handles user logout by forwarding the request to an external authentication service.
        /// </summary>
        /// <param name="model">The logout request containing the authentication token.</param>
        /// <returns>Returns an HTTP response indicating the logout status.</returns>
        [HttpPost("/log-out")]
        public async Task<IActionResult> LogOut([FromBody] string token)
        {
            if ( string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid logout attempt: Token is required.");
            }

            try
            {
                string logoutEndpoint = _baseUrl + "/log-out";

                using var request = new HttpRequestMessage(HttpMethod.Post, logoutEndpoint)
                {
                    Content = JsonContent.Create(new {token})
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(result) ? Ok(result) : BadRequest("Empty response from logout service.");
                }

                return BadRequest("Logout failed. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout attempt for token: {TokenPrefix}", token);
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

    }
}