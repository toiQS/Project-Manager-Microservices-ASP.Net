using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing plans.
    /// </summary>
    public interface IPlanServices
    {
        /// <summary>
        /// Retrieves all plans.
        /// </summary>
        /// <returns>A collection of plans.</returns>
        public Task<ServicesResult<IEnumerable<Plan>>> GetPlans();

        /// <summary>
        /// Retrieves plans associated with a specific project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project.</param>
        /// <returns>A collection of plans for the specified project.</returns>
        public Task<ServicesResult<IEnumerable<Plan>>> GetPlansInProject(string projectId);

        /// <summary>
        /// Retrieves the details of a specific plan.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan.</param>
        /// <returns>The plan associated with the provided ID.</returns>
        public Task<ServicesResult<Plan>> GetDetailPlan(string planId);

        /// <summary>
        /// Adds a new plan.
        /// </summary>
        /// <param name="plan">The plan to add.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> AddAsync(Plan plan);

        /// <summary>
        /// Updates an existing plan.
        /// </summary>
        /// <param name="plan">The updated plan data.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> UpdateAsync(Plan plan);

        /// <summary>
        /// Applies partial updates to a plan.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan.</param>
        /// <param name="plan">The plan data to patch.</param>
        /// <returns>True if the patch was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> PatchAsync(string planId, Plan plan);

        /// <summary>
        /// Deletes a plan.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> DeleteAsync(string planId);
    }
}
