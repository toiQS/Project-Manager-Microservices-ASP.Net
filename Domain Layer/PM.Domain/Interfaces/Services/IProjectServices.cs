using PM.Domain.Models.projects;

namespace PM.Domain.Interfaces.Services
{
    public interface IProjectServices
    {
        Task<ServicesResult<IEnumerable<IndexProject>>> GetProductListUserHasJoined(string userId);
        Task<ServicesResult<IEnumerable<IndexProject>>> GetProjects();
        Task<ServicesResult<IEnumerable<IndexProject>>> GetProductListUserHasOwner(string userId);
        Task<ServicesResult<bool>> Add(string userId, AddProject addProject);
        Task<ServicesResult<bool>> UpdateInfo(string userId, string projectId, UpdateProject updateProject);
        Task<ServicesResult<bool>> Delete(string userId, string projectId);
        Task<ServicesResult<bool>> UpdateIsDelete(string userId, string projectId);
        Task<ServicesResult<bool>> UpdateIsAccessed(string userId, string projectId);        
        Task<ServicesResult<bool>> UpdateIsDone(string userId, string projectId);
        Task<ServicesResult<bool>> UpdateStatus(string userId, string projectId);
    }
}
