using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing roles in a project.
    /// </summary>
    public interface IRoleInProjectServices
    {
        /// <summary>
        /// Retrieves a role in the project by its name.
        /// </summary>
        /// <param name="name">The name of the role.</param>
        /// <returns>The role associated with the provided name.</returns>
        public Task<ServicesResult<RoleInProject>> GetByName(string name);

        /// <summary>
        /// Retrieves a role in the project by its unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the role.</param>
        /// <returns>The role associated with the provided ID.</returns>
        public Task<ServicesResult<RoleInProject>> GetById(string Id);

        /// <summary>
        /// Retrieves the owner role in the project.
        /// </summary>
        /// <returns>The owner role.</returns>
        public Task<ServicesResult<RoleInProject>> GetOwnerRole();

        /// <summary>
        /// Retrieves the leader role in the project.
        /// </summary>
        /// <returns>The leader role.</returns>
        public Task<ServicesResult<RoleInProject>> GetLeaderRole();

        /// <summary>
        /// Retrieves the manager role in the project.
        /// </summary>
        /// <returns>The manager role.</returns>
        public Task<ServicesResult<RoleInProject>> GetManagerRole();

        /// <summary>
        /// Retrieves the member role in the project.
        /// </summary>
        /// <returns>The member role.</returns>
        public Task<ServicesResult<RoleInProject>> GetMemberRole();

        /// <summary>
        /// Retrieves a custom role in the project based on provided text.
        /// </summary>
        /// <param name="text">Custom role description.</param>
        /// <returns>The custom role associated with the provided text.</returns>
        public Task<ServicesResult<RoleInProject>> GetOtherRole(string text);
    }
}