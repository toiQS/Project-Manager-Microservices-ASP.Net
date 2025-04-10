using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Interfaces;
using PM.Identity.Domain.Entities;
using PM.Shared.Dtos.refresh_token;

namespace PM.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<RefreshTokenController> _logger;
        public RefreshTokenController(IRefreshTokenService refreshTokenService, ILogger<RefreshTokenController> logger)
        {
            _refreshTokenService = refreshTokenService;
            _logger = logger;
        }
        [HttpGet("revoke-all-active-tokens")]
        public async Task<IActionResult> RevokeAllActiveTokens(string userId)
        {
            _logger.LogInformation("RevokeAllActiveTokens request received for userId: {UserId}", userId);
            var result = await _refreshTokenService.RevokeAllActiveTokensByUserIdAsync(userId);
            if (result.Status)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPost("create-refresh-token")]
        public async Task<IActionResult> CreateRefreshToken([FromBody] RefreshTokenModel refreshTokenModel)
        {
            var token = new RefreshToken
            {
                Token = refreshTokenModel.Token,
                UserId = refreshTokenModel.UserId,
                Id = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddMinutes(30),
                IsRevoke = false,
                CreateAt = DateTime.UtcNow
            };
            _logger.LogInformation("CreateRefreshToken request received for token: {Token}", refreshTokenModel.Token);
            var result = await _refreshTokenService.CreateRefreshTokenAsync(token);
            if (result.Status)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPatch("update-refresh-token")]
        public async Task<IActionResult> UpdateRefreshToken(UpdateTokenRefresh model)
        {
            var refreshToken = await _refreshTokenService.GetRefreshTokenByUserId(model.UserId);
            if (refreshToken.Status == false)
            {
                return BadRequest(refreshToken);
            }
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>()
            {
                {"Token", model.Token},
                {"Expires", DateTime.UtcNow.AddMinutes(30)},
                {"IsRevoke", false}
            };
            var result = await _refreshTokenService.UpdateTokenFieldsAsync(refreshToken.Data!, keyValuePairs);
            if (result.Status)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
