using Microsoft.AspNetCore.Mvc;
using PM.Core.Application.Interfaces;

namespace PM.Core.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionMemberController : ControllerBase
    {
        //private readonly IMissionMemberHandle _missionMemberHandle;
        //private readonly ILogger<MissionMemberController> _logger;
        //private readonly HttpClient _httpClient;
        //private readonly string _baseUrl = "https://localhost:5000";

        //public MissionMemberController(IMissionMemberHandle missionMemberHandle, ILogger<MissionMemberController> logger)
        //{
        //    _missionMemberHandle = missionMemberHandle;
        //    _logger = logger;
        //    _httpClient = new HttpClient();
        //}

        ///// <summary>
        ///// Lấy các thành viên của nhiệm vụ
        ///// </summary>
        //[HttpGet("members-in-mission")]
        //public async Task<IActionResult> GetMembersInMission(string missionId)
        //{
        //    var result = await _missionMemberHandle.GetAsync(missionId);

        //    if (result.Status == ResultStatus.Success)
        //    {
        //        _logger.LogInformation("Members for mission {MissionId} retrieved successfully.", missionId);
        //        await LogTrackingAsync(missionId, $"Retrieved members for mission {missionId}");
        //        return Ok(result);
        //    }

        //    _logger.LogError("Failed to get members for mission {MissionId}: {Message}", missionId, result.Message);
        //    return BadRequest(result);
        //}

        ///// <summary>
        ///// Thêm thành viên vào nhiệm vụ
        ///// </summary>
        //[HttpPost("add-member")]
        //public async Task<IActionResult> AddMember(string userId, [FromBody] AddMemberMissionModel model)
        //{
        //    var result = await _missionMemberHandle.AddAsync(userId, model);

        //    if (result.Status == ResultStatus.Success)
        //    {
        //        _logger.LogInformation("Member added to mission for user {UserId} successfully.", userId);
        //        await LogTrackingAsync(userId, $"User {userId} added a member to mission.");
        //        return Ok(result);
        //    }

        //    _logger.LogError("Failed to add member for user {UserId}: {Message}", userId, result.Message);
        //    return BadRequest(result);
        //}

        ///// <summary>
        ///// Xóa thành viên khỏi nhiệm vụ
        ///// </summary>
        //[HttpDelete("delete-member")]
        //public async Task<IActionResult> DeleteMember(string userId, [FromBody] DeleteMemberMissionModel model)
        //{
        //    var result = await _missionMemberHandle.DeleteAsync(userId, model);

        //    if (result.Status == ResultStatus.Success)
        //    {
        //        _logger.LogInformation("Member for mission deleted successfully for user {UserId}.", userId);
        //        await LogTrackingAsync(userId, $"User {userId} deleted a member from mission.");
        //        return Ok(result);
        //    }

        //    _logger.LogError("Failed to delete member for user {UserId}: {Message}", userId, result.Message);
        //    return BadRequest(result);
        //}

        ///// <summary>
        ///// Xóa nhiều thành viên khỏi nhiệm vụ
        ///// </summary>
        //[HttpDelete("delete-many-members")]
        //public async Task<IActionResult> DeleteManyMembers(string missionId)
        //{
        //    var result = await _missionMemberHandle.DeleteManyAsync(missionId);

        //    if (result.Status == ResultStatus.Success)
        //    {
        //        _logger.LogInformation("Members for mission {MissionId} deleted successfully.", missionId);
        //        await LogTrackingAsync(missionId, $"Deleted members for mission {missionId}");
        //        return Ok(result);
        //    }

        //    _logger.LogError("Failed to delete members for mission {MissionId}: {Message}", missionId, result.Message);
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
