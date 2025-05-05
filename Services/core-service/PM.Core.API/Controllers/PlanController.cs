using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Core.Application.Interfaces;
using PM.Shared.Dtos;
using PM.Shared.Dtos.cores.plans;
using PM.Shared.Dtos.tracking;
using System.Net.Http.Json;

namespace PM.Core.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly IPlanHandle _planHandle;
        private readonly ILogger<PlanController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:5000";

        public PlanController(IPlanHandle planHandle, ILogger<PlanController> logger)
        {
            _planHandle = planHandle;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Lấy các kế hoạch của dự án
        /// </summary>
        [HttpGet("plans-project")]
        public async Task<IActionResult> GetPlansProject(string projectId)
        {
            var result = await _planHandle.GetPlansProject(projectId);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Plans for project {ProjectId} retrieved successfully.", projectId);
                await LogTrackingAsync(projectId, $"Retrieved plans for project {projectId}");
                return Ok(result);
            }

            _logger.LogError("Failed to get plans for project {ProjectId}: {Message}", projectId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Lấy chi tiết kế hoạch
        /// </summary>
        [HttpGet("plan-detail")]
        public async Task<IActionResult> GetDetailPlan(string planId)
        {
            var result = await _planHandle.GetDetailPlan(planId);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Plan {PlanId} details retrieved successfully.", planId);
                await LogTrackingAsync(planId, $"Retrieved details for plan {planId}");
                return Ok(result);
            }

            _logger.LogError("Failed to get plan details {PlanId}: {Message}", planId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Thêm mới kế hoạch
        /// </summary>
        [HttpPost("add-plan")]
        public async Task<IActionResult> AddPlan(string userId, [FromBody] AddPlanModel model)
        {
            var result = await _planHandle.AddAsync(userId, model);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Plan for user {UserId} added successfully.", userId);
                await LogTrackingAsync(userId, $"User {userId} added a new plan.");
                return Ok(result);
            }

            _logger.LogError("Failed to add plan for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Cập nhật kế hoạch
        /// </summary>
        [HttpPatch("update-plan")]
        public async Task<IActionResult> UpdatePlan(string userId, string planId, [FromBody] PacthPlanModel model)
        {
            var result = await _planHandle.PatchAsync(userId, planId, model);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Plan {PlanId} for user {UserId} updated successfully.", planId, userId);
                await LogTrackingAsync(userId, $"User {userId} updated plan {planId}");
                return Ok(result);
            }

            _logger.LogError("Failed to update plan {PlanId} for user {UserId}: {Message}", planId, userId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Xóa kế hoạch
        /// </summary>
        [HttpDelete("delete-plan")]
        public async Task<IActionResult> DeletePlan(string userId, string planId)
        {
            var result = await _planHandle.DeleteAsync(userId, planId);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Plan {PlanId} for user {UserId} deleted successfully.", planId, userId);
                await LogTrackingAsync(userId, $"User {userId} deleted plan {planId}");
                return Ok(result);
            }

            _logger.LogError("Failed to delete plan {PlanId} for user {UserId}: {Message}", planId, userId, result.Message);
            return BadRequest(result);
        }

        /// <summary>
        /// Xóa nhiều kế hoạch
        /// </summary>
        [HttpDelete("delete-many-plans")]
        public async Task<IActionResult> DeleteManyPlans(string projectId)
        {
            var result = await _planHandle.DeleteManyAsync(projectId);

            if (result.Status == ResultStatus.Success)
            {
                _logger.LogInformation("Plans for project {ProjectId} deleted successfully.", projectId);
                await LogTrackingAsync(projectId, $"Deleted plans for project {projectId}");
                return Ok(result);
            }

            _logger.LogError("Failed to delete plans for project {ProjectId}: {Message}", projectId, result.Message);
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
