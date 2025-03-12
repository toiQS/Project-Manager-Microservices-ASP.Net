using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface
        IProjectServices
    {
        public Task<ServicesResult<IEnumerable<Project>>> GetProjectsAsync();
        public Task<ServicesResult<IEnumerable<Project>>> GetProjectsUserHasJoined(string userId);
        public Task<ServicesResult<IEnumerable<Project>>> GetProjectsUserIsOwner(string userId);
        public Task<ServicesResult<IEnumerable<Project>>> GetProjectsUserIsLeader(string userId);
        public Task<ServicesResult<IEnumerable<Project>>> GetProjectsUserIsManager(string userId);
        public Task<ServicesResult<IEnumerable<Project>>> GetProjectsUserIsMember(string userId);
        public Task<ServicesResult<IEnumerable<Project>>> GetProjectsUserIsOrtherPosition(string userId);
        public Task<ServicesResult<Project>> GetProjectAsync(string projectId);
        public Task<ServicesResult<bool>> CreateProjectAsync(Project project);
        public Task<ServicesResult<bool>> UpdateProjectAsync(Project project);
        public Task<ServicesResult<bool>> PatchProjectAsync(string projectId, Project project);
        public Task<ServicesResult<bool>> DeleteProjectAsync(string projectId);
    }
}
