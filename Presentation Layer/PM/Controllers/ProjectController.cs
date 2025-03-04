using Microsoft.AspNetCore.Mvc;
using PM.Models.projects;
using System.Text;
using System.Text.Json;

namespace PM.Controllers
{
    [Route("")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProjectController> _logger;
        private readonly string _baseUrl = "https://localhost:7203";

        public ProjectController(ILogger<ProjectController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        }

        [HttpGet("/project/joined")]
        public async Task<IActionResult> GetJoinedProjects()
        {
            return await ForwardRequestAsync(HttpMethod.Get, "/project/joined");
        }

        [HttpGet("/project/owned")]
        public async Task<IActionResult> GetOwnedProjects()
        {
            return await ForwardRequestAsync(HttpMethod.Get, "/project/owned");
        }

        [HttpGet("/project/get-detail-project")]
        public async Task<IActionResult> GetProjectDetail(string projectId)
        {
            return await ForwardRequestAsync(HttpMethod.Get, $"/project/get-detail-project?projectId={projectId}");
        }

        [HttpPost("/project/add-project")]
        public async Task<IActionResult> AddProject([FromBody] AddProjectModel model)
        {
            return await ForwardRequestAsync(HttpMethod.Post, "/project/add-project", model);
        }

        [HttpPut("/project/update-project")]
        public async Task<IActionResult> UpdateProject([FromBody] UpdateProjectModel model)
        {
            return await ForwardRequestAsync(HttpMethod.Put, "/project/update-project", model);
        }

        [HttpDelete("/project/delete-project")]
        public async Task<IActionResult> DeleteProject([FromBody] DeleteProjectModel model)
        {
            return await ForwardRequestAsync(HttpMethod.Delete, "/project/delete-project", model);
        }

        [HttpPut("/project/update-complete")]
        public async Task<IActionResult> UpdateComplete([FromBody] string projectId)
        {
            return await ForwardRequestAsync(HttpMethod.Put, "/project/update-complete", projectId);
        }

        [HttpPut("/project/update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] string projectId)
        {
            return await ForwardRequestAsync(HttpMethod.Put, "/project/update-status", projectId);
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
