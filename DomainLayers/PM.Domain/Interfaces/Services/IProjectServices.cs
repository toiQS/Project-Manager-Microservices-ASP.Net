using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing projects.
    /// </summary>
    public interface IProjectServices
    {
        /// <summary>
        /// Retrieves all projects.
        /// </summary>
        /// <returns>A collection of projects.</returns>
        public Task<ServicesResult<IEnumerable<Project>>> GetProjectsAsync();

        /// <summary>
        /// Retrieves a specific project by its unique identifier.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project.</param>
        /// <returns>The project associated with the provided ID.</returns>
        public Task<ServicesResult<Project>> GetProjectAsync(string projectId);

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="arr">A collection of projects to which the new project will be added.</param>
        /// <param name="project">The project to create.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> CreateProjectAsync(IEnumerable<Project> arr, Project project);

        /// <summary>
        /// Updates an existing project.
        /// </summary>
        /// <param name="arr">A collection of projects for reference during the update.</param>
        /// <param name="project">The updated project data.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> UpdateProjectAsync(IEnumerable<Project> arr, Project project);

        /// <summary>
        /// Applies partial updates to a project.
        /// </summary>
        /// <param name="arr">A collection of projects for reference during the update.</param>
        /// <param name="projectId">The unique identifier of the project to patch.</param>
        /// <param name="project">The project data to patch.</param>
        /// <returns>True if the patch was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> PatchProjectAsync(IEnumerable<Project> arr, string projectId, Project project);

        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> DeleteProjectAsync(string projectId);
    }
}