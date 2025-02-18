using PM.Domain.Models.members;
using PM.Domain.Models.missions;
using PM.Domain.Models.projects;

namespace PM.Domain.Interfaces.Services
{
    public interface IMissionServices
    {
        public Task<ServicesResult<IEnumerable<IndexMission>>> GetIndexMissions();
        public Task<ServicesResult<IEnumerable<IndexMission>>> GetIndexMissionsInPlan(string planId);
        public Task<ServicesResult<DetailMission>> GetDetailMission(string missionId);
        public Task<ServicesResult<DetailMission>> CreateMission(string memberId, string planId ,AddMission mission);
        public Task<ServicesResult<DetailMission>> UpdateMission(string memberId, string missionId, UpdateMission mission);
        public Task<ServicesResult<DetailMission>> DeleteMission(string memberId, string missionId);

    }
}
