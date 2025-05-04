using PM.Shared.Dtos;
using PM.Shared.Dtos.cores.members;

namespace PM.Core.Application.Interfaces
{
    public interface IMissionMemberHandle
    {
        Task<ServiceResult<IEnumerable<IndexMemberModel>>> GetAsync(string missionId);
        Task<ServiceResult<IEnumerable<IndexMemberModel>>> AddAsync(string userId, AddMemberModel model);
        Task<ServiceResult<IEnumerable<IndexMemberModel>>> DeleteAsync(string userId, DeleteMemberModel model);
        Task<ServiceResult<IEnumerable<IndexMemberModel>>> DeleteManyAsync(string missonId);
    }
}
