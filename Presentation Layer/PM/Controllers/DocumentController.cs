using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
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
        private string _baseUrl = "https://localhost:7203";

       public DocumentController(ILogger<DocumentController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of documents from the external document service.
        /// </summary>
        /// <returns>Returns a list of documents in JSON format.</returns>
        [HttpGet("doc/get-docs")]
        public async Task<IActionResult> GetDocs()
        {
            try
            {
                string docsEndpoint = _baseUrl + "/doc/get-docs";

                using var request = new HttpRequestMessage(HttpMethod.Get, docsEndpoint);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(content) ? Ok(content) : NotFound("No documents found.");
                }

                _logger.LogWarning("Failed to fetch documents. Status Code: {StatusCode}", response.StatusCode);
                return NotFound("Documents not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving documents from {DocsEndpoint}", _baseUrl + "/doc/get-docs");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Retrieves details of a specific document based on its ID.
        /// </summary>
        /// <param name="docId">The unique identifier of the document.</param>
        /// <returns>Returns the document details if found; otherwise, an appropriate error message.</returns>
        [HttpGet("doc/get-doc")]
        public async Task<IActionResult> DetailDoc([FromQuery] string docId)
        {
            if (string.IsNullOrWhiteSpace(docId))
            {
                return BadRequest("Document ID cannot be null or empty.");
            }

            try
            {
                var docEndpoint = new UriBuilder(_baseUrl + "/doc/get-doc")
                {
                    Query = $"docId={Uri.EscapeDataString(docId)}"
                }.Uri;

                using var request = new HttpRequestMessage(HttpMethod.Get, docEndpoint);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(content) ? Ok(content) : NotFound("Document not found.");
                }

                _logger.LogWarning("Failed to retrieve document {DocId}. Status Code: {StatusCode}", docId.Substring(0, Math.Min(5, docId.Length)) + "...", response.StatusCode);
                return NotFound("Document not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching document details for DocId: {DocId}", docId.Substring(0, Math.Min(5, docId.Length)) + "...");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Retrieves all documents associated with a specific project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project.</param>
        /// <returns>Returns a list of documents in JSON format if found; otherwise, an appropriate error message.</returns>
        [HttpGet("project/plan/doc/get-docs-in-project")]
        public async Task<IActionResult> GetDocsInProject([FromQuery] string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return BadRequest("Project ID cannot be null or empty.");
            }

            try
            {
                var docsEndpoint = new UriBuilder(_baseUrl + "/project/plan/doc/get-docs-in-project")
                {
                    Query = $"projectId={Uri.EscapeDataString(projectId)}"
                }.Uri;

                using var request = new HttpRequestMessage(HttpMethod.Get, docsEndpoint);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(content) ? Ok(content) : NotFound("No documents found in this project.");
                }

                _logger.LogWarning("No documents found for project {ProjectId}. Status Code: {StatusCode}",
                    projectId.Substring(0, Math.Min(5, projectId.Length)) + "...", response.StatusCode);

                return NotFound("Documents not found for the specified project.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents for project: {ProjectId}",
                    projectId.Substring(0, Math.Min(5, projectId.Length)) + "...");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Retrieves all documents associated with a specific plan.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan.</param>
        /// <returns>Returns a list of documents in JSON format if found; otherwise, an appropriate error message.</returns>
        [HttpGet("project/plan/doc/get-docs-in-plan")]
        public async Task<IActionResult> GetDocsInPlan([FromQuery] string planId)
        {
            if (string.IsNullOrWhiteSpace(planId))
            {
                return BadRequest("Plan ID cannot be null or empty.");
            }

            try
            {
                var docsEndpoint = new UriBuilder(_baseUrl + "/project/plan/doc/get-docs-in-plan")
                {
                    Query = $"planId={Uri.EscapeDataString(planId)}"
                }.Uri;

                using var request = new HttpRequestMessage(HttpMethod.Get, docsEndpoint);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(content) ? Ok(content) : NotFound("No documents found in this plan.");
                }

                _logger.LogWarning("No documents found for plan {PlanId}. Status Code: {StatusCode}",
                    planId.Substring(0, Math.Min(5, planId.Length)) + "...", response.StatusCode);

                return NotFound("Documents not found for the specified plan.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents for plan: {PlanId}",
                    planId.Substring(0, Math.Min(5, planId.Length)) + "...");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Retrieves all documents associated with a specific mission.
        /// </summary>
        /// <param name="missionId">The unique identifier of the mission.</param>
        /// <returns>Returns a list of documents in JSON format if found; otherwise, an appropriate error message.</returns>
        [HttpGet("project/plan/mission/doc/get-docs-in-mission")]
        public async Task<IActionResult> GetDocsInMission([FromQuery] string missionId)
        {
            if (string.IsNullOrWhiteSpace(missionId))
            {
                return BadRequest("Mission ID cannot be null or empty.");
            }

            try
            {
                var docsEndpoint = new UriBuilder(_baseUrl + "/project/plan/mission/doc/get-docs-in-mission")
                {
                    Query = $"missionId={Uri.EscapeDataString(missionId)}"
                }.Uri;

                using var request = new HttpRequestMessage(HttpMethod.Get, docsEndpoint);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(content) ? Ok(content) : NotFound("No documents found for this mission.");
                }

                _logger.LogWarning("No documents found for mission {MissionId}. Status Code: {StatusCode}",
                    missionId.Substring(0, Math.Min(5, missionId.Length)) + "...", response.StatusCode);

                return response.StatusCode == System.Net.HttpStatusCode.NotFound
                    ? NotFound("Documents not found for the specified mission.")
                    : StatusCode((int)response.StatusCode, "Error retrieving mission documents.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents for mission: {MissionId}",
                    missionId.Substring(0, Math.Min(5, missionId.Length)) + "...");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Adds a document to a specific project.
        /// </summary>
        /// <param name="model">The model containing document details.</param>
        /// <returns>Returns success response if the document is added successfully, otherwise an error message.</returns>
        [HttpPost("project/doc/add-doc")]
        public async Task<IActionResult> AddDocToProject([FromBody] AddDocToProjectModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request. Please check the provided data.");
            }

            try
            {
                var requestUri = new Uri($"{_baseUrl}/project/doc/add-doc");

                using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = JsonContent.Create(model)
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(content) ? Ok(content) : StatusCode(204, "Document added but no content returned.");
                }

                _logger.LogWarning("Failed to add document to project. Status Code: {StatusCode}. Reason: {Reason}",
                    response.StatusCode, response.ReasonPhrase ?? "Unknown error");

                return StatusCode((int)response.StatusCode, response.ReasonPhrase ?? "An error occurred while adding the document.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding document to project.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        /// <summary>
        /// Updates an existing document with new data.
        /// </summary>
        /// <param name="model">The model containing updated document details.</param>
        /// <returns>Returns success response if the update is successful, otherwise an appropriate error message.</returns>
        [HttpPut("doc/update-doc")]
        public async Task<IActionResult> UpdateDoc([FromBody] UpdateDocModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request. Please check the provided data.");
            }

            try
            {
                var requestUri = new Uri($"{_baseUrl}/doc/update-doc");

                using var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
                {
                    Content = JsonContent.Create(model)
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(content) ? Ok(content) : StatusCode(204, "Document updated but no content returned.");
                }

                _logger.LogWarning("Failed to update document. Status Code: {StatusCode}. Reason: {Reason}",
                    response.StatusCode, response.ReasonPhrase ?? "Unknown error");

                return StatusCode((int)response.StatusCode, response.ReasonPhrase ?? "An error occurred while updating the document.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating document.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Deletes a document from the system.
        /// </summary>
        /// <param name="model">The model containing the document details to delete.</param>
        /// <returns>Returns success response if the document is deleted successfully, otherwise an error message.</returns>
        [HttpDelete("doc/delete-doc")]
        public async Task<IActionResult> DeleteDoc([FromBody] DeleteDocModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request. Please check the provided data.");
            }

            try
            {
                var requestUri = new Uri($"{_baseUrl}/doc/delete-doc");

                using var request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
                {
                    Content = JsonContent.Create(model)
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(content) ? Ok(content) : StatusCode(204, "Document deleted but no content returned.");
                }

                _logger.LogWarning("Failed to delete document. Status Code: {StatusCode}. Reason: {Reason}",
                    response.StatusCode, response.ReasonPhrase ?? "Unknown error");

                return StatusCode((int)response.StatusCode, response.ReasonPhrase ?? "An error occurred while deleting the document.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting document.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

    }
}
