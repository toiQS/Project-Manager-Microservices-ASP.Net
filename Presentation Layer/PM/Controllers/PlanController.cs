using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace PM.Controllers
{
    [Route("")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PlanController> _logger;
        private readonly string _baseUrl = "https://localhost:7203";

        public PlanController(ILogger<PlanController> logger)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
            _logger = logger;
        }

        [HttpGet("/plan/get-plans")]
        public async Task<IActionResult> GetPlans()
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "plan/get-plans");
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode ? Ok(await response.Content.ReadAsStringAsync()) : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching plans");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("/project/plan/get-plan-in-project")]
        public async Task<IActionResult> GetPlanInProject(string projectId)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"project/plan/get-plan-in-project?projectId={projectId}");
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode ? Ok(await response.Content.ReadAsStringAsync()) : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching plans in project");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("/project/plan/get-detail-plan")]
        public async Task<IActionResult> GetDetailPlan(string planId)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"project/plan/get-detail-plan?planId={planId}");
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode ? Ok(await response.Content.ReadAsStringAsync()) : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching plan details");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("/project/plan/add-plan")]
        public async Task<IActionResult> AddPlan([FromBody] object model)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "project/plan/add-plan")
                {
                    Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
                };
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode ? Ok(await response.Content.ReadAsStringAsync()) : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding plan");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("/project/plan/update-plan")]
        public async Task<IActionResult> UpdatePlan([FromBody] object model)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, "project/plan/update-plan")
                {
                    Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
                };
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode ? Ok(await response.Content.ReadAsStringAsync()) : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plan");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("/project/plan/delete-plan")]
        public async Task<IActionResult> DeletePlan([FromBody] object model)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, "project/plan/delete-plan")
                {
                    Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
                };
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode ? Ok(await response.Content.ReadAsStringAsync()) : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting plan");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
