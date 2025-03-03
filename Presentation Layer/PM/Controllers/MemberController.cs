using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Models.membes;
using System.Reflection;

namespace PM.Controllers
{
    //[Route("api/[controller]")]
    [Route("/")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MemberController> _logger;
        public MemberController(ILogger<MemberController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _httpClient.BaseAddress = new Uri("https://localhost:7203");
        }
        /// <summary>
        /// Retrieves the list of members.
        /// </summary>
        /// <returns>A list of members in JSON format or an error message.</returns>
        [HttpGet("member/get-members")]
        public async Task<IActionResult> GetMembers()
        {
            try
            {
                var response = await _httpClient.GetAsync("/member/get-members");
                if (response.IsSuccessStatusCode)
                {
                    var members = await response.Content.ReadAsStringAsync();
                    return Ok(members);
                }
                return BadRequest("Failed to retrieve members.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching members.");
                return StatusCode(500, "Internal server error.");
            }
        }
        /// <summary>
        /// Retrieves the list of members in a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A list of members in JSON format or an error message.</returns>
        [HttpGet("project/member/get-members-in-project")]
        public async Task<IActionResult> GetMembersInProject([FromQuery] string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return BadRequest("Project ID cannot be null or empty.");

            try
            {
                var response = await _httpClient.GetAsync($"/project/member/get-members-in-project?projectId={projectId}");
                if (response.IsSuccessStatusCode)
                {
                    var members = await response.Content.ReadAsStringAsync();
                    return Ok(members);
                }
                return NotFound("No members found for the given project.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching members in project.");
                return StatusCode(500, "Internal server error.");
            }
        }
        /// <summary>
        /// Retrieves a specific member by their ID.
        /// </summary>
        /// <param name="memberId">The ID of the member.</param>
        /// <returns>The member details in JSON format or an error message.</returns>
        [HttpGet("project/member/get-member")]
        public async Task<IActionResult> GetMember([FromQuery] string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
                return BadRequest("Member ID cannot be null or empty.");

            try
            {
                var response = await _httpClient.GetAsync($"/project/member/get-member?memberId={memberId}");
                if (response.IsSuccessStatusCode)
                {
                    var member = await response.Content.ReadAsStringAsync();
                    return Ok(member);
                }
                return NotFound("Member not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching member details.");
                return StatusCode(500, "Internal server error.");
            }
        }
        /// <summary>
        /// Adds a new member to a project.
        /// </summary>
        /// <param name="model">The member details to be added.</param>
        /// <returns>The added member's details or an error message.</returns>
        [HttpPost("project/member/add-member")]
        public async Task<IActionResult> AddMember([FromBody] AddMemberModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/project/member/add-member", model);
                if (response.IsSuccessStatusCode)
                {
                    var member = await response.Content.ReadAsStringAsync();
                    return Ok(member);
                }
                return BadRequest("Failed to add the member.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding member to project.");
                return StatusCode(500, "Internal server error.");
            }
        }
        /// <summary>
        /// Updates an existing member's details in a project.
        /// </summary>
        /// <param name="model">The member update model containing new details.</param>
        /// <returns>The updated member details or an error message.</returns>
        [HttpPut("project/member/update-member")]
        public async Task<IActionResult> UpdateMember([FromBody] UpdateMemberModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            try
            {
                var response = await _httpClient.PutAsJsonAsync("/project/member/update-member", model);
                if (response.IsSuccessStatusCode)
                {
                    var member = await response.Content.ReadAsStringAsync();
                    return Ok(member);
                }
                return BadRequest("Failed to update member.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating member.");
                return StatusCode(500, "Internal server error.");
            }
        }
        /// <summary>
        /// Deletes a member from the project based on the provided member ID.
        /// </summary>
        /// <param name="memberId">The ID of the member to be deleted.</param>
        /// <returns>Success message if deleted, or an error response.</returns>
        [HttpDelete("project/member/delete-member")]
        public async Task<IActionResult> DeleteMember([FromQuery] DeleteMemberModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Member ID cannot be null or empty.");

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/project/member/delete-member", model);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                return NotFound("Member not found or could not be deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting member.");
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}
