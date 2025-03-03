using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Models.docs;
using PM.Models.documents;

namespace PM.Controllers
{
    [Route("/")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ILogger<DocumentController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7203") };
        }

        /// <summary>
        /// Fetches all documents.
        /// </summary>
        [HttpGet("doc/get-docs")]
        public async Task<IActionResult> GetDocs()
        {
            try
            {
                var response = await _httpClient.GetAsync("/doc/get-docs");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                return BadRequest("Invalid login attempt.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Fetches a specific document by ID.
        /// </summary>
        [HttpGet("doc/get-doc")]
        public async Task<IActionResult> DetailDoc([FromQuery] string docId)
        {
            if (string.IsNullOrEmpty(docId)) return BadRequest("Document ID cannot be null or empty.");
            try
            {
                var response = await _httpClient.GetAsync($"/doc/get-doc?docId={docId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                return NotFound("Document not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching document details.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Fetches documents within a specific project.
        /// </summary>
        [HttpGet("project/plan/doc/get-docs-in-project")]
        public async Task<IActionResult> GetDocsInProject([FromQuery] string projectId)
        {
            if (string.IsNullOrEmpty(projectId)) return BadRequest();
            try
            {
                var response = await _httpClient.GetAsync($"/project/plan/doc/get-docs-in-project?projectId={projectId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                return NotFound("Document not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching document details.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Fetches documents within a specific plan.
        /// </summary>
        [HttpGet("project/plan/doc/get-docs-in-plan")]
        public async Task<IActionResult> GetDocsInPlan([FromQuery] string planId)
        {
            if (string.IsNullOrEmpty(planId)) return BadRequest();
            try
            {
                var response = await _httpClient.GetAsync($"/project/plan/doc/get-docs-in-plan?planId={planId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                return NotFound("Document not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching document details.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Fetches documents within a specific mission.
        /// </summary>
        [HttpGet("project/plan/mission/doc/get-docs-in-mission")]
        public async Task<IActionResult> GetDocsInMission([FromQuery] string missionId)
        {
            if (string.IsNullOrEmpty(missionId)) return BadRequest();
            try
            {
                var response = await _httpClient.GetAsync($"/project/plan/mission/doc/get-docs-in-mission?missionId={missionId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                return NotFound("Document not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching document details.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Adds a document to a project.
        /// </summary>
        [HttpPost("project/doc/add-doc")]
        public async Task<IActionResult> AddDocToProject([FromBody] AddDocToProjectModel model)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/project/doc/add-doc", model);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                return NotFound("Document not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding document to project.");
                return StatusCode(500, "Internal server error.");
            }
        }
        /// <summary>
        /// Updates an existing document.
        /// </summary>
        [HttpPut("doc/update-doc")]
        public async Task<IActionResult> UpdateDoc([FromBody] UpdateDocModel model)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                var response = await _httpClient.PutAsJsonAsync("/doc/update-doc", model);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                return NotFound("Document not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Deletes an existing document.
        /// </summary>
        [HttpDelete("doc/delete-doc")]
        public async Task<IActionResult> DeleteDoc([FromBody] DeleteDocModel model)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/doc/delete-doc", model);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                return NotFound("Document not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
