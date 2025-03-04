using Microsoft.AspNetCore.Mvc;
using PM.Models.missions;
using System.Text;
using System.Text.Json;

namespace PM.Controllers
{
    [Route("")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly ILogger<MissionController> _logger;
        private readonly HttpClient _httpClient;
        private string _baseUrl = "https://localhost:7203";
        public MissionController(ILogger<MissionController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }
        /// <summary>
        /// Lấy danh sách các mission từ API.
        /// </summary>
        /// <returns>Danh sách mission hoặc lỗi</returns>
        [HttpGet("mission/get-missions")]
        public async Task<IActionResult> GetMissions()
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "mission/get-missions");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var missions = await response.Content.ReadAsStringAsync();
                    return Ok(missions);
                }

                _logger.LogWarning("Failed to get missions. Status: {StatusCode}, Reason: {Reason}",
                                   response.StatusCode, response.ReasonPhrase);
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching missions.");
                return StatusCode(500, "Internal server error.");
            }
        }
        /// <summary>
        /// Lấy danh sách mission trong một plan.
        /// </summary>
        /// <param name="planId">ID của kế hoạch</param>
        /// <returns>Danh sách mission hoặc lỗi</returns>
        [HttpGet("project/plan/mission/get-missions-in-plan")]
        public async Task<IActionResult> GetMissionInPlan([FromQuery] string planId)
        {
            if (string.IsNullOrEmpty(planId))
            {
                _logger.LogWarning("GetMissionInPlan: Missing or empty planId.");
                return BadRequest("Plan ID cannot be empty.");
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"project/plan/mission/get-missions-in-plan?planId={planId}");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }

                _logger.LogWarning("GetMissionInPlan failed. Status: {StatusCode}, Reason: {Reason}",
                                   response.StatusCode, response.ReasonPhrase);
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching missions in plan.");
                return StatusCode(500, "Internal server error.");
            }
        }
        /// <summary>
        /// Lấy chi tiết một mission theo ID.
        /// </summary>
        /// <param name="missionId">ID của mission</param>
        /// <returns>Chi tiết mission hoặc lỗi</returns>
        [HttpGet("project/plan/mission/get-detail-mission")]
        public async Task<IActionResult> GetDetailMission([FromQuery] string missionId)
        {
            if (string.IsNullOrWhiteSpace(missionId))
            {
                _logger.LogWarning("GetDetailMission: Missing or empty missionId.");
                return BadRequest("Mission ID cannot be empty.");
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"project/plan/mission/get-detail-mission?missionId={missionId}");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var mission = await response.Content.ReadAsStringAsync();
                    return Ok(mission);
                }

                _logger.LogWarning("GetDetailMission failed. Status: {StatusCode}, Reason: {Reason}",
                                   response.StatusCode, response.ReasonPhrase);
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching mission details.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Thêm một mission mới vào plan.
        /// </summary>
        /// <param name="model">Thông tin mission</param>
        /// <returns>Mission được thêm hoặc lỗi</returns>
        [HttpPost("project/plan/mission/add-mission")]
        public async Task<IActionResult> AddMission([FromBody] AddMissonModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("AddMission: Invalid model state.");
                return BadRequest("Invalid input data.");
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "project/plan/mission/add-mission")
                {
                    Content = JsonContent.Create(model)
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var mission = await response.Content.ReadAsStringAsync();
                    return Ok(mission);
                }

                _logger.LogWarning("AddMission failed. Status: {StatusCode}, Reason: {Reason}",
                                   response.StatusCode, response.ReasonPhrase);
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while adding mission.");
                return StatusCode(500, "Internal server error.");
            }
        }
        /// <summary>
        /// Cập nhật thông tin mission trong plan.
        /// </summary>
        /// <param name="model">Thông tin mission cần cập nhật</param>
        /// <returns>Mission đã cập nhật hoặc lỗi</returns>
        [HttpPut("project/plan/mission/update-mission")]
        public async Task<IActionResult> UpdateMission([FromBody] UpdateMissionModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateMission: Invalid model state.");
                return BadRequest("Invalid input data.");
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, "project/plan/mission/update-mission")
                {
                    Content = JsonContent.Create(model)
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var mission = await response.Content.ReadAsStringAsync();
                    return Ok(mission);
                }

                _logger.LogWarning("UpdateMission failed. Status: {StatusCode}, Reason: {Reason}",
                                   response.StatusCode, response.ReasonPhrase);
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating mission.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Xóa mission trong plan.
        /// </summary>
        /// <param name="model">Thông tin mission cần xóa</param>
        /// <returns>Mission đã xóa hoặc lỗi</returns>
        [HttpDelete("project/plan/mission/delete-mission")]
        public async Task<IActionResult> DeleteMission([FromBody] DeleteMissionModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("DeleteMission: Invalid model state.");
                return BadRequest("Invalid input data.");
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, "project/plan/mission/delete-mission")
                {
                    Content = JsonContent.Create(model)
                };

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var mission = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return Ok(mission);
                }

                _logger.LogWarning("DeleteMission failed. Status: {StatusCode}, Reason: {Reason}",
                                   response.StatusCode, response.ReasonPhrase);
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting mission.");
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}
