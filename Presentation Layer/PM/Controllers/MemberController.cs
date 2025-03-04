using Microsoft.AspNetCore.Mvc;
using PM.Models.membes;

namespace PM.Controllers
{
    //[Route("api/[controller]")]
    [Route("/")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MemberController> _logger;
        private string _baseUrl = "https://localhost:7203";
        public MemberController(ILogger<MemberController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the list of members.
        /// </summary>
        /// <returns>Returns the list of members if successful, otherwise an error message.</returns>
        [HttpGet("member/get-members")]
        public async Task<IActionResult> GetMembers()
        {
            try
            {
                var requestUri = new Uri($"{_baseUrl}/member/get-members");

                using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var members = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(members) ? Ok(members) : StatusCode(204, "No members found.");
                }

                _logger.LogWarning("Failed to retrieve members. Status Code: {StatusCode}. Reason: {Reason}",
                    response.StatusCode, response.ReasonPhrase ?? "Unknown error");

                return StatusCode((int)response.StatusCode, response.ReasonPhrase ?? "An error occurred while retrieving members.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching members.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Retrieves the list of members in a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>Returns a list of project members or an error message.</returns>
        [HttpGet("project/member/get-members-in-project")]
        public async Task<IActionResult> GetMembersInProject([FromQuery] string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
                return BadRequest("Project ID cannot be null or empty.");

            try
            {
                var requestUri = new Uri($"{_baseUrl}/project/member/get-members-in-project?projectId={projectId}");

                using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var members = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(members) ? Ok(members) : StatusCode(204, "No members found in project.");
                }

                _logger.LogWarning("Failed to retrieve members in project {ProjectId}. Status Code: {StatusCode}. Reason: {Reason}",
                    projectId, response.StatusCode, response.ReasonPhrase ?? "Unknown error");

                return StatusCode((int)response.StatusCode, response.ReasonPhrase ?? "An error occurred while retrieving project members.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching members for project {ProjectId}.", projectId);
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Retrieves details of a specific project member.
        /// </summary>
        /// <param name="memberId">The ID of the member.</param>
        /// <returns>Returns member details or an appropriate error message.</returns>
        [HttpGet("project/member/get-member")]
        public async Task<IActionResult> GetMember([FromQuery] string memberId)
        {
            if (string.IsNullOrWhiteSpace(memberId))
                return BadRequest("Member ID cannot be null or empty.");

            try
            {
                var requestUri = new Uri($"{_baseUrl}/project/member/get-member?memberId={memberId}");

                using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var member = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(member) ? Ok(member) : StatusCode(204, "No member details available.");
                }

                _logger.LogWarning("Failed to retrieve member {MemberId}. Status Code: {StatusCode}. Reason: {Reason}",
                    memberId, response.StatusCode, response.ReasonPhrase ?? "Unknown error");

                return StatusCode((int)response.StatusCode, response.ReasonPhrase ?? "An error occurred while retrieving member details.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching details for member {MemberId}.", memberId);
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Adds a new member to a project.
        /// </summary>
        /// <param name="model">The member details including project ID.</param>
        /// <returns>Returns the newly added member details or an error response.</returns>
        [HttpPost("project/member/add-member")]
        public async Task<IActionResult> AddMember([FromBody] AddMemberModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            try
            {
                var requestUri = new Uri($"{_baseUrl}/project/member/add-member");
                var response = await _httpClient.PostAsJsonAsync(requestUri, model);

                if (response.IsSuccessStatusCode)
                {
                    var member = await response.Content.ReadAsStringAsync();
                    return Ok(member);
                }

                _logger.LogWarning("Failed to add member. Status Code: {StatusCode}. Reason: {Reason}",
                    response.StatusCode, response.ReasonPhrase ?? "Unknown error");

                return StatusCode((int)response.StatusCode, response.ReasonPhrase ?? "Failed to add the member.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding member.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Updates an existing project member.
        /// </summary>
        /// <param name="model">The member details to be updated.</param>
        /// <returns>Returns updated member details or an error response.</returns>
        [HttpPut("project/member/update-member")]
        public async Task<IActionResult> UpdateMember([FromBody] UpdateMemberModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            try
            {
                var requestUri = new Uri($"{_baseUrl}/project/member/update-member");
                var response = await _httpClient.PutAsJsonAsync(requestUri, model);

                if (response.IsSuccessStatusCode)
                {
                    var member = await response.Content.ReadAsStringAsync();
                    return Ok(member);
                }

                _logger.LogWarning("Failed to update member. Status Code: {StatusCode}. Reason: {Reason}",
                    response.StatusCode, response.ReasonPhrase ?? "Unknown error");

                return StatusCode((int)response.StatusCode, response.ReasonPhrase ?? "Failed to update the member.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating member.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        /// <summary>
        /// Deletes a project member.
        /// </summary>
        /// <param name="model">The member details to be deleted.</param>
        /// <returns>Returns success message or an error response.</returns>
        [HttpDelete("project/member/delete-member")]
        public async Task<IActionResult> DeleteMember([FromBody] DeleteMemberModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            try
            {
                var requestUri = new Uri($"{_baseUrl}/project/member/delete-member");
                var response = await _httpClient.SendAsync(new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    Content = JsonContent.Create(model),
                    RequestUri = requestUri
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }

                _logger.LogWarning("Failed to delete member. Status Code: {StatusCode}. Reason: {Reason}",
                    response.StatusCode, response.ReasonPhrase ?? "Unknown error");

                return StatusCode((int)response.StatusCode, response.ReasonPhrase ?? "Failed to delete the member.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting member.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


    }
}
