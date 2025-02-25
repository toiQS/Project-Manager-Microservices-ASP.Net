using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Application.Interfaces;
using PM.Application.Models.missions;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.missions;

namespace PM.Application.Implements
{
    /// <summary>
    /// Mission logic implementation handling mission-related operations.
    /// </summary>
    public class MissionLogic : ControllerBase, IMissionLogic
    {
        private readonly IMissionServices _missionServices;
        private readonly ILogger<MissionLogic> _logger;

        #region Constructor
        public MissionLogic(IMissionServices missionServices, ILogger<MissionLogic> logger)
        {
            _missionServices = missionServices;
            _logger = logger;
        }
        #endregion

        #region Get all missions
        /// <summary>
        /// Retrieves all missions.
        /// </summary>
        /// <returns>List of missions.</returns>
        public async Task<IActionResult> GetIndexMissions()
        {
            try
            {
                var missionResponse = await _missionServices.GetIndexMissions();
                if (!missionResponse.Status) return BadRequest(missionResponse.Message);
                return Ok(missionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving missions.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Get missions in a plan
        /// <summary>
        /// Retrieves missions associated with a specific plan.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <returns>List of missions under the specified plan.</returns>
        public async Task<IActionResult> GetIndexMissionsInPlan(string planId)
        {
            try
            {
                var missionResponse = await _missionServices.GetIndexMissionsInPlan(planId);
                if (!missionResponse.Status) return BadRequest(missionResponse.Message);
                return Ok(missionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving missions in plan.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Get mission details
        /// <summary>
        /// Retrieves details of a specific mission.
        /// </summary>
        /// <param name="missionId">The mission identifier.</param>
        /// <returns>Mission details.</returns>
        public async Task<IActionResult> GetDetailMission(string missionId)
        {
            try
            {
                var missionResponse = await _missionServices.GetDetailMission(missionId);
                if (!missionResponse.Status) return BadRequest(missionResponse.Message);
                return Ok(missionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving mission details.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Create a mission
        /// <summary>
        /// Creates a new mission.
        /// </summary>
        /// <param name="model">Mission creation model.</param>
        /// <returns>Created mission data.</returns>
        public async Task<IActionResult> CreateMission(AddMissonModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model state.");
            try
            {
                var addMission = new AddMission()
                {
                    TaskName = model.TaskName,
                    Description = model.Description,
                    EndAt = model.EndAt,
                    StartAt = model.StartAt,
                };
                var missionResponse = await _missionServices.CreateMission(model.MemberId, model.PlanId, addMission);
                if (!missionResponse.Status) return BadRequest(missionResponse.Message);
                return Ok(missionResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating mission.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Update mission
        /// <summary>
        /// Updates an existing mission.
        /// </summary>
        /// <param name="model">Mission update model.</param>
        /// <returns>Updated mission data.</returns>
        public async Task<IActionResult> UpdateMission(UpdateMissionModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model state.");
            try
            {
                var updateMission = new UpdateMission()
                {
                    EndAt = model.EndAt,
                    TaskName = model.TaskName,
                    StartAt = model.StartAt,
                    TaskDescription = model.TaskDescription,
                };
                var missionResponse = await _missionServices.UpdateMission(model.MemnberId, model.MissionId, updateMission);
                if (!missionResponse.Status) return BadRequest(missionResponse.Message);
                return Ok(missionResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating mission.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Delete mission
        /// <summary>
        /// Deletes a mission.
        /// </summary>
        /// <param name="model">Mission deletion model.</param>
        /// <returns>Deletion status.</returns>
        public async Task<IActionResult> DeleteMission(DeleteMissionModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model state.");
            try
            {
                var missionResponse = await _missionServices.DeleteMission(model.MemberId, model.MissionId);
                if (!missionResponse.Status) return BadRequest(missionResponse.Message);
                return Ok(missionResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting mission.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Manage mission members
        /// <summary>
        /// Adds members to a mission.
        /// </summary>
        /// <param name="model">Model containing mission and member details.</param>
        /// <returns>Updated mission data.</returns>
        public async Task<IActionResult> AddMembers(AddMembertMissionModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model state.");
            try
            {
                var missionResponse = await _missionServices.AddMembers(model.MemeberId, model.MissisonId, model.MemberIds);
                if (!missionResponse.Status) return BadRequest(missionResponse.Message);
                return Ok(missionResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding members to mission.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a member from a mission.
        /// </summary>
        /// <param name="model">Model containing member and mission details.</param>
        /// <returns>Updated mission data.</returns>
        public async Task<IActionResult> DeleteMember(DeleteMemberMissionModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model state.");
            try
            {
                var missionResponse = await _missionServices.DeleteMember(model.MemeberId, model.MissionId, model.MemberDeleteId);
                if (!missionResponse.Status) return BadRequest(missionResponse.Message);
                return Ok(missionResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing member from mission.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion
    }
}
