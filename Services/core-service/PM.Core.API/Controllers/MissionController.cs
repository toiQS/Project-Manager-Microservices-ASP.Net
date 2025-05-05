using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Core.Application.Interfaces;
using PM.Shared.Dtos;
using PM.Shared.Dtos.cores.missions;
using PM.Shared.Dtos.tracking;
using System.Net.Http.Json;

namespace PM.Core.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly IMissionHandle _missionHandle;
        private readonly ILogger<MissionController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:5000";

        public MissionController(IMissionHandle missionHandle, ILogger<MissionController> logger)
        {
            _missionHandle = missionHandle;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Lấy các nhiệm vụ của kế hoạch
        /// </summary>
        [HttpGet("missions-plan")]
        public async Task<IActionResult> GetMissions(string planId)
        {
            var result = await _missionHandle.GetAsync(planId);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Missions for plan {PlanId} retrieved successfully.", planId);
                await LogTrackingAsync(planId, $"Retrieved missions for plan {planId}");
                return Ok(result);
            }

            _logger.LogError("Failed to get missions for plan {PlanId}: {Message}", planId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Lấy chi tiết nhiệm vụ
        /// </summary>
        [HttpGet("mission-detail")]
        public async Task<IActionResult> GetMissionDetail(string missionId)
        {
            var result = await _missionHandle.GetDetailAsync(missionId);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Mission {MissionId} details retrieved successfully.", missionId);
                await LogTrackingAsync(missionId, $"Retrieved details for mission {missionId}");
                return Ok(result);
            }

            _logger.LogError("Failed to get mission details for {MissionId}: {Message}", missionId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Thêm nhiệm vụ mới
        /// </summary>
        [HttpPost("add-mission")]
        public async Task<IActionResult> AddMission(string userId, [FromBody] AddMissonModel model)
        {
            var result = await _missionHandle.AddAsync(userId, model);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Mission for user {UserId} added successfully.", userId);
                await LogTrackingAsync(userId, $"User {userId} added a new mission.");
                return Ok(result);
            }

            _logger.LogError("Failed to add mission for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Cập nhật thông tin nhiệm vụ
        /// </summary>
        [HttpPatch("update-mission")]
        public async Task<IActionResult> UpdateMission(string userId, string missionId, [FromBody] PatchMissionModel model)
        {
            var result = await _missionHandle.PatchMissionAsync(userId, missionId, model);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Mission {MissionId} for user {UserId} updated successfully.", missionId, userId);
                await LogTrackingAsync(userId, $"User {userId} updated mission {missionId}");
                return Ok(result);
            }

            _logger.LogError("Failed to update mission {MissionId} for user {UserId}: {Message}", missionId, userId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Xóa nhiệm vụ
        /// </summary>
        [HttpDelete("delete-mission")]
        public async Task<IActionResult> DeleteMission(string userId, string missionId)
        {
            var result = await _missionHandle.DeleteAsync(userId, missionId);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Mission {MissionId} for user {UserId} deleted successfully.", missionId, userId);
                await LogTrackingAsync(userId, $"User {userId} deleted mission {missionId}");
                return Ok(result);
            }

            _logger.LogError("Failed to delete mission {MissionId} for user {UserId}: {Message}", missionId, userId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Xóa nhiều nhiệm vụ
        /// </summary>
        [HttpDelete("delete-many-missions")]
        public async Task<IActionResult> DeleteManyMissions(string planId)
        {
            var result = await _missionHandle.DeleteManyAsync(planId);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Missions for plan {PlanId} deleted successfully.", planId);
                await LogTrackingAsync(planId, $"Deleted missions for plan {planId}");
                return Ok(result);
            }

            _logger.LogError("Failed to delete missions for plan {PlanId}: {Message}", planId, result.Message);
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
