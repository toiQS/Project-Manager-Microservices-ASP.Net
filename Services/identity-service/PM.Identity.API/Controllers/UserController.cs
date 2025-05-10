using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Interfaces;

namespace PM.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //private readonly IUserHandle _userHandle;
        //private readonly ILogger<UserController> _logger;
        //private readonly HttpClient _httpClient;
        //private readonly string _baseUrl = "https://localhost:5000";

        //public UserController(IUserHandle userHandle, ILogger<UserController> logger)
        //{
        //    _userHandle = userHandle;
        //    _logger = logger;
        //    _httpClient = new HttpClient();
        //}

        ///// <summary>
        ///// Lấy thông tin người dùng theo userId
        ///// </summary>
        //[HttpGet("get-user")]
        //public async Task<IActionResult> GetUser(string userId)
        //{
        //    var result = await _userHandle.GetUser(userId);

        //    if (result.Status == ResultStatus.Success)
        //    {
        //        _logger.LogInformation("User {UserId} retrieved successfully.", userId);
        //        await LogTrackingAsync(userId, $"User {userId} data retrieved.");
        //        return Ok(result);
        //    }

        //    _logger.LogError("Failed to get user {UserId}: {Message}", userId, result.Message);
        //    return BadRequest(result);
        //}

        ///// <summary>
        ///// Cập nhật thông tin người dùng
        ///// </summary>
        //[HttpPatch("patch-user")]
        //public async Task<IActionResult> PatchUser(string userId, [FromBody] UserPatchModel model)
        //{
        //    var result = await _userHandle.PatchUserHandle(userId, model);

        //    if (result.Status == ResultStatus.Success)
        //    {
        //        _logger.LogInformation("User {UserId} updated successfully.", userId);
        //        await LogTrackingAsync(userId, $"User {userId} information updated.");
        //        return Ok(result);
        //    }

        //    _logger.LogError("Failed to patch user {UserId}: {Message}", userId, result.Message);
        //    return BadRequest(result);
        //}

        ///// <summary>
        ///// Gửi log hành vi người dùng đến hệ thống tracking
        ///// </summary>
        //private async Task LogTrackingAsync(string userId, string actionName)
        //{
        //    var trackingData = new AddTrackingModel
        //    {
        //        ProjectId = string.Empty,
        //        UserId = userId,
        //        ActionName = actionName
        //    };

        //    try
        //    {
        //        var request = new HttpRequestMessage
        //        {
        //            RequestUri = new Uri($"{_baseUrl}/api/tracking/add-tracking-log"),
        //            Method = HttpMethod.Post,
        //            Content = JsonContent.Create(trackingData)
        //        };

        //        var response = await _httpClient.SendAsync(request);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();
        //            _logger.LogWarning("Tracking failed for user {UserId}. Response: {Response}", userId, content);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Exception occurred while sending tracking log for user {UserId}.", userId);
        //    }
        //}
    }
}
