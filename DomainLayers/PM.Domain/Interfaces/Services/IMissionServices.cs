using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IMissionServices
    {
        public Task<ServicesResult<IEnumerable<Mission>>> GetMissions();
        public Task<ServicesResult<IEnumerable<Mission>>> GetMissionsInPlan(string planId);
        public Task<ServicesResult<Mission>> GetDetailMission(string missionId);
        public Task<ServicesResult<bool>> CreateMission(Mission mission);
        public Task<ServicesResult<bool>> UpdateMission(Mission mission);
        public Task<ServicesResult<bool>> PatchMission(string missionId, Mission mission);
        public Task<ServicesResult<bool>> DeleteMission(string missionId);
       
    }
}
