using Microsoft.AspNetCore.Mvc;
using PM.Application.Interfaces;
using PM.Application.Models.missions;
using System.Threading.Tasks;

namespace ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly IMissionLogic _missionLogic;

        public MissionController(IMissionLogic missionLogic)
        {
            _missionLogic = missionLogic;
        }

        #region GetIndexMissions
        /// <summary>
        /// Retrieves all missions.
        /// </summary>
        [HttpGet("get-index-missions")]
        public async Task<IActionResult> GetIndexMissions()
        {
            return await _missionLogic.GetIndexMissions();
        }
        #endregion

        #region GetIndexMissionsInPlan
        /// <summary>
        /// Retrieves all missions within a specific plan.
        /// </summary>
        [HttpGet("get-index-missions-in-plan")]
        public async Task<IActionResult> GetIndexMissionsInPlan([FromQuery] string planId)
        {
            return await _missionLogic.GetIndexMissionsInPlan(planId);
        }
        #endregion

        #region GetDetailMission
        /// <summary>
        /// Retrieves details of a specific mission.
        /// </summary>
        [HttpGet("get-detail-mission")]
        public async Task<IActionResult> GetDetailMission([FromQuery] string missionId)
        {
            return await _missionLogic.GetDetailMission(missionId);
        }
        #endregion

        #region CreateMission
        /// <summary>
        /// Creates a new mission.
        /// </summary>
        [HttpPost("create-mission")]
        public async Task<IActionResult> CreateMission([FromBody] AddMissonModel model)
        {
            return await _missionLogic.CreateMission(model);
        }
        #endregion

        #region UpdateMission
        /// <summary>
        /// Updates an existing mission.
        /// </summary>
        [HttpPut("update-mission")]
        public async Task<IActionResult> UpdateMission([FromBody] UpdateMissionModel model)
        {
            return await _missionLogic.UpdateMission(model);
        }
        #endregion

        #region DeleteMission
        /// <summary>
        /// Deletes a mission.
        /// </summary>
        [HttpDelete("delete-mission")]
        public async Task<IActionResult> DeleteMission([FromBody] DeleteMissionModel model)
        {
            return await _missionLogic.DeleteMission(model);
        }
        #endregion

        #region AddMembers
        /// <summary>
        /// Adds members to a mission.
        /// </summary>
        [HttpPost("add-members")]
        public async Task<IActionResult> AddMembers([FromBody] AddMembertMissionModel model)
        {
            return await _missionLogic.AddMembers(model);
        }
        #endregion

        #region DeleteMember
        /// <summary>
        /// Removes a member from a mission.
        /// </summary>
        [HttpDelete("delete-member")]
        public async Task<IActionResult> DeleteMember([FromBody] DeleteMemberMissionModel model)
        {
            return await _missionLogic.DeleteMember(model);
        }
        #endregion
    }
}
