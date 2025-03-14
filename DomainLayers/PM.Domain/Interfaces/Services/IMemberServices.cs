using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IMemberServices
    {
        public Task<ServicesResult<IEnumerable<ProjectMember>>> GetMembers();
        public Task<ServicesResult<IEnumerable<ProjectMember>>> GetMembersInProject(string projectId);
        public Task<ServicesResult<ProjectMember>> GetDetailMember(string memberId);
        public Task<ServicesResult<bool>> AddMember(ProjectMember member);
        public Task<ServicesResult<bool>> UpdateMember(ProjectMember member);
        public Task<ServicesResult<bool>> PatchMember(string memberId, ProjectMember member);
        public Task<ServicesResult<bool>> DeleteMember(string memberId);
        public Task<ServicesResult<bool>> DeteleMembersInProject(string projectId);
    }
}
