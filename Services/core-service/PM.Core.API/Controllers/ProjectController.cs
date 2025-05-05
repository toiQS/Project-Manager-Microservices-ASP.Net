using Microsoft.AspNetCore.Mvc;
using PM.Core.Application.Interfaces;
using PM.Shared.Dtos;
using PM.Shared.Dtos.cores.projects;
using PM.Shared.Dtos.tracking;
using System.Net.Http.Json;

namespace PM.Core.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectHandle _projectHandle;
        private readonly ILogger<ProjectController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:5000";

        public ProjectController(IProjectHandle projectHandle, ILogger<ProjectController> logger)
        {
            _projectHandle = projectHandle;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Lấy các dự án mà người dùng tham gia
        /// </summary>
        [HttpGet("projects-user-joined")]
        public async Task<IActionResult> GetProjectsUserHasJoined(string userId)
        {
            var result = await _projectHandle.ProjectsUserHasJoined(userId);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Projects for user {UserId} retrieved successfully.", userId);
                await LogTrackingAsync(userId, $"User {userId} retrieved joined projects.");
                return Ok(result);
            }

            _logger.LogError("Failed to get projects for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Lấy các dự án mà người dùng là chủ sở hữu
        /// </summary>
        [HttpGet("projects-user-owner")]
        public async Task<IActionResult> GetProjectsUserIsOwner(string userId)
        {
            var result = await _projectHandle.ProjectUserIsOwner(userId);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Projects for user {UserId} as owner retrieved successfully.", userId);
                await LogTrackingAsync(userId, $"User {userId} retrieved owned projects.");
                return Ok(result);
            }

            _logger.LogError("Failed to get projects for user {UserId} as owner: {Message}", userId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Tìm kiếm dự án theo từ khóa
        /// </summary>
        [HttpGet("search-projects")]
        public async Task<IActionResult> SearchProjects(string text)
        {
            var result = await _projectHandle.GetProjectsByText(text);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Projects search for '{Text}' executed successfully.", text);
                return Ok(result);
            }

            _logger.LogError("Failed to search projects by text '{Text}': {Message}", text, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Thêm mới dự án
        /// </summary>
        [HttpPost("add-project")]
        public async Task<IActionResult> AddProject(string userId, [FromBody] AddProjectModel model)
        {
            var result = await _projectHandle.AddAsync(userId, model);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Project for user {UserId} added successfully.", userId);
                await LogTrackingAsync(userId, $"User {userId} added a new project.");
                return Ok(result);
            }

            _logger.LogError("Failed to add project for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Cập nhật thông tin dự án
        /// </summary>
        [HttpPatch("update-project")]
        public async Task<IActionResult> UpdateProject(string userId, string projectId, [FromBody] PatchProjectModel model)
        {
            var result = await _projectHandle.PatchAsync(userId, projectId, model);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Project {ProjectId} for user {UserId} updated successfully.", projectId, userId);
                await LogTrackingAsync(userId, $"User {userId} updated project {projectId}.");
                return Ok(result);
            }

            _logger.LogError("Failed to update project {ProjectId} for user {UserId}: {Message}", projectId, userId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Xóa dự án
        /// </summary>
        [HttpDelete("delete-project")]
        public async Task<IActionResult> DeleteProject(string userId, string projectId)
        {
            var result = await _projectHandle.DeleteAsync(userId, projectId);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Project {ProjectId} for user {UserId} deleted successfully.", projectId, userId);
                await LogTrackingAsync(userId, $"User {userId} deleted project {projectId}.");
                return Ok(result);
            }

            _logger.LogError("Failed to delete project {ProjectId} for user {UserId}: {Message}", projectId, userId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Gửi log hành vi người dùng đến hệ thống tracking
        /// </summary>
        private async Task LogTrackingAsync(string userId, string actionName)
        {
            var trackingData = new AddTrackingModel
            {
                ProjectId = string.Empty,
                UserId = userId,
                ActionName = actionName
            };

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_baseUrl}/api/tracking/add-tracking-log"),
                    Method = HttpMethod.Post,
                    Content = JsonContent.Create(trackingData)
                };

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Tracking failed for user {UserId}. Response: {Response}", userId, content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while sending tracking log for user {UserId}.", userId);
            }
        }
    }
}
