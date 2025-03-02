using Microsoft.AspNetCore.Mvc;
using PM.Application.Interfaces;
using PM.Application.Models.membes;

namespace AuthAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberLogic _memberLogic;

        public MemberController(IMemberLogic memberLogic)
        {
            _memberLogic = memberLogic;
        }

        /// <summary>
        /// Retrieves all members.
        /// </summary>
        /// <returns>An IActionResult containing the list of members or an error message.</returns>
        [HttpGet("get-members")]
        public async Task<IActionResult> GetMembers()
        {
            return await _memberLogic.GetMembers();
        }

        /// <summary>
        /// Retrieves members for a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>An IActionResult containing the list of project members or an error message.</returns>
        [HttpGet("get-members-in-project")]
        public async Task<IActionResult> GetMembers([FromQuery] string projectId)
        {
            return await _memberLogic.GetMembers(projectId);
        }

        /// <summary>
        /// Retrieves detailed information about a specific member.
        /// </summary>
        /// <param name="memberId">The ID of the member.</param>
        /// <returns>An IActionResult containing the member details or an error message.</returns>
        [HttpGet("get-member")]
        public async Task<IActionResult> GetMember(string memberId)
        {
            return await _memberLogic.GetMember(memberId);
        }

        /// <summary>
        /// Adds a new member.
        /// </summary>
        /// <param name="model">The model containing member details.</param>
        /// <returns>An IActionResult indicating the result of the add operation.</returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddMember([FromBody] AddMemberModel model)
        {
            return await _memberLogic.AddMember(model);
        }

        /// <summary>
        /// Deletes a member.
        /// </summary>
        /// <param name="model">The model containing member deletion details.</param>
        /// <returns>An IActionResult indicating the result of the deletion.</returns>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteMember([FromBody] DeleteMemberModel model)
        {
            return await _memberLogic.DeleteMember(model);
        }

        /// <summary>
        /// Updates a member's information.
        /// </summary>
        /// <param name="model">The model containing updated member details.</param>
        /// <returns>An IActionResult indicating the result of the update.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateMember([FromBody] UpdateMemberModel model)
        {
            return await _memberLogic.UpdateMember(model);
        }
    }
}
