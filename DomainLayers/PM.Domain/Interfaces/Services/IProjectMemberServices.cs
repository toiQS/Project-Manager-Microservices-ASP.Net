using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IProjectMemberServices
    {
        public Task<ServicesResult<IEnumerable<ProjectMember>>> GetMembers();
        public Task<ServicesResult<IEnumerable<ProjectMember>>> GetProjectMembersInProject(string projectId);
        public Task<ServicesResult<IEnumerable<ProjectMember>>> GetMemberOtherRolesIsNotOwner(string projectId, string roleId);

        public Task<ServicesResult<ProjectMember>> GetDetailMember(string memberId);
        public Task<ServicesResult<ProjectMember>> GetOwnerProject(string projectId, string roleId);
        public Task<ServicesResult<bool>> AddMember(ProjectMember member);
        public Task<ServicesResult<bool>> UpdateMember(ProjectMember member);
        public Task<ServicesResult<bool>> PatchMember(string memberId, ProjectMember member);
        public Task<ServicesResult<bool>> DeleteMember(string memberId);
        public Task<ServicesResult<bool>> DeleteMembersInProject(string projectId);
    }
}
