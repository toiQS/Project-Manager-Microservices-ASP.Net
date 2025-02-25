using Microsoft.AspNetCore.Mvc;
using PM.Application.Models.missions;

namespace PM.Application.Interfaces
{
    public interface IMissionLogic
    {
        public Task<IActionResult> GetIndexMissions();
        public Task<IActionResult> GetIndexMissionsInPlan(string planId);
        public Task<IActionResult> GetDetailMission(string missionId);
        public Task<IActionResult> CreateMission(AddMissonModel model);
        public Task<IActionResult> UpdateMission(UpdateMissionModel model);
        public Task<IActionResult> DeleteMission(DeleteMissionModel model);
        public Task<IActionResult> AddMembers(AddMembertMissionModel model);
        public Task<IActionResult> DeleteMember(DeleteMemberMissionModel model);
    }
}
