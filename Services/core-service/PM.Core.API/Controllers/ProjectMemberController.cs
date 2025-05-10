using Microsoft.AspNetCore.Mvc;

namespace PM.Core.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectMemberController : ControllerBase
    {
        //private readonly IProjectMemberHandle _projectMemberHandle;
        //private readonly ILogger<ProjectMemberController> _logger;
        //private readonly HttpClient _httpClient;
        //private readonly string _baseUrl = "https://localhost:5000";

        //public ProjectMemberController(IProjectMemberHandle projectMemberHandle, ILogger<ProjectMemberController> logger)
        //{
        //    _projectMemberHandle = projectMemberHandle;
        //    _logger = logger;
        //    _httpClient = new HttpClient();
        //}

        ///// <summary>
        ///// Lấy các thành viên trong dự án
        ///// </summary>
        //[HttpGet("members-in-project")]
        //public async Task<IActionResult> GetMembersInProject(string projectId)
        //{
        //    var result = await _projectMemberHandle.GetMembersInProject(projectId);

        //    if (result.Status == ResultStatus.Success)
        //    {
        //        _logger.LogInformation("Members for project {ProjectId} retrieved successfully.", projectId);
        //        await LogTrackingAsync(projectId, $"Retrieved members for project {projectId}");
        //        return Ok(result);
        //    }

        //    _logger.LogError("Failed to get members for project {ProjectId}: {Message}", projectId, result.Message);
        //    return BadRequest(result);
        //}

        ///// <summary>
        ///// Thêm thành viên vào dự án
        ///// </summary>
        //[HttpPost("add-member")]
        //public async Task<IActionResult> AddMember(string userId, [FromBody] AddProjectMemberModel model)
        //{
        //    var result = await _projectMemberHandle.AddAsync(userId, model);

        //    if (result.Status == ResultStatus.Success)
        //    {
        //        _logger.LogInformation("Member added to project for user {UserId} successfully.", userId);
        //        await LogTrackingAsync(userId, $"User {userId} added a member to project.");
        //        return Ok(result);
        //    }

        //    _logger.LogError("Failed to add member for user {UserId}: {Message}", userId, result.Message);
        //    return BadRequest(result);
        //}

        ///// <summary>
        ///// Cập nhật thông tin thành viên trong dự án
        ///// </summary>
        //[HttpPatch("update-member")]
        //public async Task<IActionResult> UpdateMember(string userId, string memberId, [FromBody] PacthProjectMemberModel model)
        //{
        //    var result = await _projectMemberHandle.PatchAsync(userId, memberId, model);

        //    if (result.Status == ResultStatus.Success)
        //    {
        //        _logger.LogInformation("Member {MemberId} for user {UserId} updated successfully.", memberId, userId);
        //        await LogTrackingAsync(userId, $"User {userId} updated member {memberId} in project.");
        //        return Ok(result);
        //    }

        //    _logger.LogError("Failed to update member {MemberId} for user {UserId}: {Message}", memberId, userId, result.Message);
        //    return BadRequest(result);
        //}

        ///// <summary>
        ///// Xóa thành viên khỏi dự án
        ///// </summary>
        //[HttpDelete("delete-member")]
        //public async Task<IActionResult> DeleteMember(string userId, string memberId)
        //{
        //    var result = await _projectMemberHandle.DeleteAsync(userId, memberId);

        //    if (result.Status == ResultStatus.Success)
        //    {
        //        _logger.LogInformation("Member {MemberId} for user {UserId} deleted successfully.", memberId, userId);
        //        await LogTrackingAsync(userId, $"User {userId} deleted member {memberId} from project.");
        //        return Ok(result);
        //    }

        //    _logger.LogError("Failed to delete member {MemberId} for user {UserId}: {Message}", memberId, userId, result.Message);
        //    return BadRequest(result);
        //}

        ///// <summary>
        ///// Xóa nhiều thành viên khỏi dự án
        ///// </summary>
        //[HttpDelete("delete-many-members")]
        //public async Task<IActionResult> DeleteManyMembers(string projectId)
        //{
        //    var result = await _projectMemberHandle.DeteleManyAsync(projectId);

        //    if (result.Status == ResultStatus.Success)
        //    {
        //        _logger.LogInformation("Members for project {ProjectId} deleted successfully.", projectId);
        //        await LogTrackingAsync(projectId, $"Deleted members for project {projectId}");
        //        return Ok(result);
        //    }

        //    _logger.LogError("Failed to delete members for project {ProjectId}: {Message}", projectId, result.Message);
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
