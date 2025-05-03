using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing project members.
    /// </summary>
    public interface IProjectMemberServices
    {
        /// <summary>
        /// Retrieves all members.
        /// </summary>
        /// <returns>A collection of all project members.</returns>
        public Task<ServicesResult<IEnumerable<ProjectMember>>> GetMembers();

        /// <summary>
        /// Retrieves members associated with a specific project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project.</param>
        /// <returns>A collection of project members.</returns>
        public Task<ServicesResult<IEnumerable<ProjectMember>>> GetProjectMembersInProject(string projectId);

        /// <summary>
        /// Retrieves members in a project who are not owners.
        /// </summary>
        /// <param name="projectId">The project ID.</param>
        /// <param name="roleId">The role ID to exclude.</param>
        /// <returns>A collection of project members with roles other than owner.</returns>
        public Task<ServicesResult<IEnumerable<ProjectMember>>> GetMemberOtherRolesIsNotOwner(string projectId, string roleId);

        /// <summary>
        /// Retrieves details of a specific member.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <returns>The project member details.</returns>
        public Task<ServicesResult<ProjectMember>> GetDetailMember(string memberId);

        /// <summary>
        /// Retrieves the owner of a project.
        /// </summary>
        /// <param name="projectId">The project ID.</param>
        /// <param name="roleId">The role ID representing the owner role.</param>
        /// <returns>The owner of the project.</returns>
        public Task<ServicesResult<ProjectMember>> GetOwnerProject(string projectId, string roleId);
        public Task<ServicesResult<IEnumerable<ProjectMember>>> GetProjectsByUserId(string userId);
        /// <summary>
        /// Adds a new member to the project.
        /// </summary>
        /// <param name="member">The member to add.</param>
        /// <returns>True if the addition was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> AddMember(ProjectMember member);

        /// <summary>
        /// Updates an existing project member.
        /// </summary>
        /// <param name="member">The updated member information.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> UpdateMember(ProjectMember member);

        /// <summary>
        /// Applies partial updates to a project member.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <param name="member">The member data to patch.</param>
        /// <returns>True if the patch was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> PatchMember(string memberId, ProjectMember member);

        /// <summary>
        /// Deletes a specific project member.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> DeleteMember(string memberId);

        /// <summary>
        /// Deletes all members in a specific project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> DeleteMembersInProject(string projectId);
    }
}