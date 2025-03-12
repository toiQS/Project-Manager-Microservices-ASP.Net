using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IMissionServices
    {
        public Task<ServicesResult<IEnumerable<Mission>>> GetIndexMissions();
        public Task<ServicesResult<IEnumerable<Mission>>> GetIndexMissionsInPlan(string planId);
        public Task<ServicesResult<Mission>> GetDetailMission(string missionId);
        public Task<ServicesResult<bool>> CreateMission(Mission mission);
        public Task<ServicesResult<bool>> UpdateMission(Mission mission);
        public Task<ServicesResult<bool>> PacthMission(string missionId, Mission mission);
        public Task<ServicesResult<bool>> DeleteMission(string missionId);
        public Task<ServicesResult<bool>> AddMembers( string missionId, List<string> memberIds);
        public Task<ServicesResult<bool>> DeleteMember( string missionId, string memberId);
    }
}
