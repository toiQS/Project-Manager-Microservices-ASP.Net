using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Interfaces;
using PM.Shared.Dtos.auths;
using PM.Shared.Dtos.tracking;

namespace PM.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthHandle _authHandle;
        private readonly ILogger<AuthController> _logger;
        private string _baseUrl = "https://localhost:5000";
        private readonly HttpClient _httpClient;
        public AuthController(IAuthHandle authHandle, ILogger<AuthController> logger)
        {
            _authHandle = authHandle;
            _logger = logger;
            _httpClient = new HttpClient();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authHandle.RegisterHandleAsync(model);
            if (result.Status == ResultStatus.Success)
            {
                var context = new AddTrackingModel()
                {
                    ProjectId = string.Empty,
                    UserId = string.Empty,
                    ActionName = $"A user register an account with email: {model.Email}"
                };
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(_baseUrl + "/api/tracking/add-tracking-log"),
                    Method = HttpMethod.Post,
                    Content = JsonContent.Create(context)
                };
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var responseResult = await response.Content.ReadAsStringAsync();
                    var responseSplit = responseResult.Split(',');
                    var responseContext = responseSplit[3];
                    return BadRequest(responseContext);
                    
                }
                return Ok(result);
            }
            else
            {
                _logger.LogError(result.Message);
                return BadRequest(result);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authHandle.LoginHandleAsync(model);
            if (result.Status == ResultStatus.Success)
            {
                return Ok(result);
            }
            else
            {
                _logger.LogError(result.Message);
                return BadRequest(result);
            }
        }
    }
}
