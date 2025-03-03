using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Models.documents;

namespace PM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthController> _logger;

        public DocumentController(ILogger<AuthController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7203") };
        }
        [HttpGet("get-doc")]
        public async Task<IActionResult> GetDocs()
        {
            try
            {
                var request = await _httpClient.GetAsync("/doc/get-docs");
                if (request.IsSuccessStatusCode)
                {
                    var response = await request.Content.ReadAsStringAsync();
                    return Ok(response);
                }
                return BadRequest("Invalid login attempt.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("detail-document")]
        public async Task<IActionResult> DetailDoc(string docId)
        {
            if (string.IsNullOrEmpty(docId)) return BadRequest("Document ID cannot be null or empty.");
            try
            {
                var response = await _httpClient.GetAsync($"/doc/get-doc?docId={docId}");
                if (response.IsSuccessStatusCode)
                {
                    var document = await response.Content.ReadAsStringAsync();
                    return Ok(document);
                }
                return NotFound("Document not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching document details.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
