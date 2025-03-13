using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IMissionAssignmentServices
    {
        public Task<ServicesResult<IEnumerable<MissionAssignment>>> GetMissionAssignmentsAsync();
        public Task<ServicesResult<IEnumerable<MissionAssignment>>> GetMissionAssignmentsInMissionAsync(string missionId);
        public Task<ServicesResult<MissionAssignment>> GetMissionAssignmentAsync(string missionAssistanceId);
        public Task<ServicesResult<bool>> AddAsync(MissionAssignment missionAssignment);
        public Task<ServicesResult<bool>> AddManyAsync(List<MissionAssignment> missionAssignment);
        public Task<ServicesResult<bool>> RemoveAsync(MissionAssignment missionAssignment);
        public Task<ServicesResult<bool>> UpdateAsync(MissionAssignment missionAssignment);
    }
}
