using PM.Shared.Dtos;
using PM.Shared.Dtos.cores.projects;

namespace PM.Core.Application.Interfaces
{
    public interface IProjectHandle
    {
        Task<ServiceResult<IEnumerable<IndexProjectModel>>> ProjectsUserHasJoined(string userId);
    }
}
