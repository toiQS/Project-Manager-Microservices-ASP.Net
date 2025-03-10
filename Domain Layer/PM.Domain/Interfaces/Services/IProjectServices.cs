using PM.Domain.Models.projects;

namespace PM.Domain.Interfaces.Services
{
    public interface 
        IProjectServices
    {
        public Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsAsync();
        public Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsOwnerAsync(string userId);
        public Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsLeaderAsync(string userId);
        public Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsManagerAsync(string userId);
        public Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsMemberAsync(string userId);
        public Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserHasJoined(string userId);
        public Task<ServicesResult<DetailProject>> GetDetailProjectAsync(string projectId);
        public Task<ServicesResult<bool>> CreateProjectAsync(string userId, AddProject project);
        public Task<ServicesResult<bool>> UpdateProjectAsync(string userId, string projectId, UpdateProject project);
        public Task<ServicesResult<bool>> DeleteProjectAsync(string userId, string projectId);
        public Task<ServicesResult<bool>> UpdateIsDeleteAsync(string userId, string projectId);
        public Task<ServicesResult<bool>> UpdateIsCompletedAsync(string userId, string projectId);
    }
}
