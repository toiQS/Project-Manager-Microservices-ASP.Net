using PM.Domain.Models.missions;

namespace PM.Domain.Interfaces.Services
{
    public interface IMissionServices
    {
        public Task<ServicesResult<IEnumerable<IndexMission>>> GetIndexMissions();
        public Task<ServicesResult<IEnumerable<IndexMission>>> GetIndexMissionsInPlan(string planId);
        public Task<ServicesResult<DetailMission>> GetDetailMission(string missionId);
        public Task<ServicesResult<DetailMission>> CreateMission(string memberId, string planId ,AddMission mission);
        public Task<ServicesResult<DetailMission>> UpdateMission(string memberId, string missionId, UpdateMission mission);
        public Task<ServicesResult<IEnumerable<IndexMission>>> DeleteMission(string memberId, string missionId);
        public Task<ServicesResult<bool>> DeleteMissionFunc(string memberId, string missionId);
        public Task<ServicesResult<DetailMission>> AddMembers(string memberCurrentId, string missionId, List<string> memberIds);
        public Task<ServicesResult<DetailMission>> DeleteMember(string memberCurrentId, string missionId, string memberId);
    }
}
