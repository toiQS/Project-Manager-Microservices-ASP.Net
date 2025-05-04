using PM.Shared.Dtos;
using PM.Shared.Dtos.cores.missions;

namespace PM.Core.Application.Interfaces
{
    public interface IMissionHandle
    {
        Task<ServiceResult<IEnumerable<IndexMissionModel>>> GetAsync(string planId);
        Task<ServiceResult<DetailMissionModel>> GetDetailAsync(string missionId);
        Task<ServiceResult<DetailMissionModel>> AddAsync(string userId, AddMissonModel model);
        Task<ServiceResult<DetailMissionModel>> PatchMissionAsync(string userId, string missionId, PatchMissionModel model);
        Task<ServiceResult<IEnumerable<IndexMissionModel>>> DeleteAsync(string userId, string missionId);
        Task<ServiceResult<IEnumerable<IndexMissionModel>>> DeleteManyAsync(string userId, string planId);
    }
}
