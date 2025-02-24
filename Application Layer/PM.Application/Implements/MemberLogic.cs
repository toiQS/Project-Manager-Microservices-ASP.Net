using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Application.Interfaces;
using PM.Application.Models.membes;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.members;

namespace PM.Application.Implements
{
    
    public class MemberLogic : ControllerBase, IMemberLogic
    {
        private readonly IMemberServices _memberServices;
        private readonly ILogger<MemberLogic> _logger;

        public MemberLogic(IMemberServices memberServices, ILogger<MemberLogic> logger)
        {
            _memberServices = memberServices;
            _logger = logger;
        }
        #region  Retrieves all members.
        /// <summary>
        /// Retrieves all members.
        /// </summary>
        /// <returns>An IActionResult containing the list of members or an error message.</returns>

        public async Task<IActionResult> GetMembers()
        {
            try
            {
                var memberResponse = await _memberServices.GetMembers();
                if (!memberResponse.Status)
                    return BadRequest(memberResponse.Message);

                return Ok(memberResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all members.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Retrieves members in a specific project.
        /// <summary>
        /// Retrieves members in a specific project.
        /// </summary>
        /// <param name="projectId">The project ID.</param>
        /// <returns>An IActionResult containing the list of members in the project or an error message.</returns>

        public async Task<IActionResult> GetMembers([FromQuery] string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return BadRequest("Project ID is required.");

            try
            {
                var memberResponse = await _memberServices.GetMemberInProject(projectId);
                if (!memberResponse.Status)
                    return BadRequest(memberResponse.Message);

                return Ok(memberResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving project members.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Retrieves detailed information about a specific member.
        /// <summary>
        /// Retrieves detailed information about a specific member.
        /// </summary>
        /// <param name="memberId">The member ID.</param>
        /// <returns>An IActionResult containing member details or an error message.</returns>

        public async Task<IActionResult> GetMember(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
                return BadRequest("Member ID is required.");

            try
            {
                var memberResponse = await _memberServices.GetDetailMember(memberId);
                if (!memberResponse.Status)
                    return BadRequest(memberResponse.Message);

                return Ok(memberResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving member details.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Adds a new member to a project.
        /// <summary>
        /// Adds a new member to a project.
        /// </summary>
        /// <param name="model">The model containing new member information.</param>
        /// <returns>An IActionResult indicating the outcome of the add operation.</returns>

        public async Task<IActionResult> AddMember([FromBody] AddMemberModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid member data.");

            try
            {
                var newMember = new AddMember
                {
                    RoleId = model.RoleId,
                    PositionWork = model.PositionWork,
                    UserId = model.UserId,
                };

                var memberResponse = await _memberServices.AddMember(model.MemberCurrentId, model.ProjectId, newMember);
                if (!memberResponse.Status)
                    return BadRequest(memberResponse.Message);

                return Ok(memberResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a member.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Deletes a member from a project.
        /// <summary>
        /// Deletes a member from a project.
        /// </summary>
        /// <param name="model">The model containing IDs for deletion.</param>
        /// <returns>An IActionResult indicating the outcome of the deletion.</returns>
        
        public async Task<IActionResult> DeleteMember([FromBody] DeleteMemberModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            try
            {
                var memberResponse = await _memberServices.DeleteMember(model.MemberCurrentId, model.MemberId);
                if (!memberResponse.Status)
                    return BadRequest(memberResponse.Message);

                return Ok(memberResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a member.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region  Updates information of an existing member.
        /// <summary>
        /// Updates information of an existing member.
        /// </summary>
        /// <param name="model">The model containing updated member information.</param>
        /// <returns>An IActionResult with the updated member details or an error message.</returns>
        
        public async Task<IActionResult> UpdateMember([FromBody] UpdateMemberModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            try
            {
                var updateMember = new UpdateMember
                {
                    ProjectId = model.ProjectId,
                    PositionWork = model.PositionWork,
                    UserId = model.UserId,
                };

                var memberResponse = await _memberServices.UpdateMember(model.MemberCurrentId, model.MemberId, updateMember);
                if (!memberResponse.Status)
                    return BadRequest(memberResponse.Message);

                return Ok(memberResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating member.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion
    }
}
