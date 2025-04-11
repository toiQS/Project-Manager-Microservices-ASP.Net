using PM.EndPoint.API.Flows.Interfaces;
using PM.Shared.Dtos.Auths;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace PM.EndPoint.API.Flows.Implements
{
    public class AuthFlow : IAuthFlow
    {
        private string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthFlow> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthFlow(IConfiguration configuration, ILogger<AuthFlow> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _baseUrl = "https://localhost:8000";
        }
        public async Task<string> HandleLogin(LoginModel loginModel)
        {
            var requestSigninHandle = new HttpRequestMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json"),
                RequestUri = new Uri($"{_baseUrl}/api/auth/login"),
                Method = HttpMethod.Post,
                Headers = {
                    { "Accept", "application/json" },
                    { "Accept-Encoding", "gzip, deflate, br" },
                    { "Accept-Language", "en-US,en;q=0.9" },
                    { "Connection", "keep-alive" },
                    { "Host", "localhost:8000" },
                    { "Origin", _baseUrl },
                    { "Referer", $"{_baseUrl}/auth/login" }
                }
            };
            var responseSigninHandle = await _httpClient.SendAsync(requestSigninHandle);
            if (responseSigninHandle.IsSuccessStatusCode)
            {
                var responseContent = await responseSigninHandle.Content.ReadAsStringAsync();
                return responseContent;
            }
            else
            {
                var errorContent = await responseSigninHandle.Content.ReadAsStringAsync();
                _logger.LogError($"Error during login: {errorContent}");
                throw new Exception($"Login failed: {errorContent}");
            }
        }
        public async Task<string> HandleDemo()
        {

            var user = _httpContextAccessor.HttpContext?.User;
            if (user.Identity.IsAuthenticated)
            {
                return "auth";
            }
            return "not auth";
        }
    }
}
