using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing statuses.
    /// </summary>
    public interface IStatusServices
    {
        /// <summary>
        /// Retrieves all statuses.
        /// </summary>
        /// <returns>A collection of statuses.</returns>
        public Task<ServicesResult<IEnumerable<Status>>> GetStatusesAsync();

        /// <summary>
        /// Retrieves a status by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the status.</param>
        /// <returns>The status associated with the provided ID.</returns>
        public Task<ServicesResult<Status>> GetStatusByIdAsync(int id);

        /// <summary>
        /// Determines the status for creating an entity based on the start date.
        /// </summary>
        /// <param name="startDate">The start date of the entity.</param>
        /// <returns>The status code for creation.</returns>
        public int StatusForCreateAsync(DateTime startDate);

        /// <summary>
        /// Determines the status for updating an entity based on the start and end dates.
        /// </summary>
        /// <param name="startDate">The start date of the entity.</param>
        /// <param name="endDate">The end date of the entity.</param>
        /// <returns>The status code for update.</returns>
        public int StatusForUpdateAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Determines the final status based on the end date.
        /// </summary>
        /// <param name="endDate">The end date of the entity.</param>
        /// <returns>The status code for finalization.</returns>
        public int StatusForFinallyAsync(DateTime endDate);
    }
}