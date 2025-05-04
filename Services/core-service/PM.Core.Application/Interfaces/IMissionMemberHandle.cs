using PM.Shared.Dtos;
using PM.Shared.Dtos.cores.members.missions;

namespace PM.Core.Application.Interfaces
{
    public interface IMissionMemberHandle
    {
        Task<ServiceResult<IEnumerable<IndexMemberMissionModel>>> GetAsync(string missionId);
        Task<ServiceResult<IEnumerable<IndexMemberMissionModel>>> AddAsync(string userId, AddMemberMissionModel model);
        Task<ServiceResult<IEnumerable<IndexMemberMissionModel>>> DeleteAsync(string userId, DeleteMemberMissionModel model);
        Task<ServiceResult<IEnumerable<IndexMemberMissionModel>>> DeleteManyAsync(string missonId);
    }
}
