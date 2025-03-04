using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using PM.Models.reports;

namespace PM.Controllers
{
    [Route("")]
    [ApiController]
    public class ProgressReportController : ControllerBase
    {
       private string _baseUrl = "https://localhost:7203";
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProgressReportController> _logger;
        public ProgressReportController(ILogger<ProgressReportController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }
        [HttpGet("/project/plan/progress-report/get-reports-in-plan")]
        public async Task<IActionResult> GetReportsInPlan(string planId)
        {
            return await ForwardRequestAsync(HttpMethod.Get, $"/project/plan/progress-report/get-reports-in-plan?planId={planId}");
        }
        [HttpPost("/project/plan/progress-report/add-report")]
        public async Task<IActionResult> AddReport([FromBody] AddReportModel model)
        {
            return await ForwardRequestAsync(HttpMethod.Post, "/project/plan/progress-report/add-report", model);
        }
        [HttpPut("/project/plan/progress-report/update-report")]
        public async Task<IActionResult> UpdateReport([FromBody] UpdateReportModel model)
        {
            return await ForwardRequestAsync(HttpMethod.Put, "/project/plan/progress-report/update-report", model);
        }
        [HttpDelete("/project/plan/progress-report/delete-report")]
        public async Task<IActionResult> DeleteReport([FromBody] DeleteReportModel model)
        {
            return await ForwardRequestAsync(HttpMethod.Delete, "/project/plan/progress-report/delete-report", model);
        }

        private async Task<IActionResult> ForwardRequestAsync(HttpMethod method, string path, object? data = null)
        {
            try
            {
                using var request = new HttpRequestMessage(method, _baseUrl + path);
                if (data != null)
                {
                    request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                }
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode ? Ok(await response.Content.ReadAsStringAsync()) : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request for {Path}", path);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
