using PM.Domain.Models.projects;

namespace PM.Domain.Interfaces.Services
{
    public interface IProjectServices
    {
        Task<ServicesResult<IEnumerable<IndexProject>>> GetProductListUserHasJoined(string userId);
        //Task<ServicesResult<IEnumerable<IndexProject>>> GetProjects();
        Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasOwner(string userId);
        Task<ServicesResult<DetailProject>> Add(string userId, AddProject addProject);
        Task<ServicesResult<DetailProject>> UpdateInfo(string userId, string projectId, UpdateProject updateProject);
        Task<ServicesResult<IEnumerable<IndexProject>>> Delete(string userId, string projectId);
        Task<ServicesResult<DetailProject>> UpdateIsDelete(string userId, string projectId);
        Task<ServicesResult<DetailProject>> UpdateIsCompleted(string userId, string projectId);
    }
}
