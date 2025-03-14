using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing progress reports.
    /// </summary>
    public interface IProgressReportServices
    {
        /// <summary>
        /// Retrieves all progress reports.
        /// </summary>
        /// <returns>A collection of progress reports.</returns>
        public Task<ServicesResult<IEnumerable<ProgressReport>>> GetReports();

        /// <summary>
        /// Retrieves progress reports associated with a specific plan.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan.</param>
        /// <returns>A collection of progress reports for the specified plan.</returns>
        public Task<ServicesResult<IEnumerable<ProgressReport>>> GetReportsInPlan(string planId);

        /// <summary>
        /// Adds a new progress report.
        /// </summary>
        /// <param name="progressReport">The progress report to add.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> AddAsync(ProgressReport progressReport);

        /// <summary>
        /// Updates an existing progress report.
        /// </summary>
        /// <param name="progressReport">The updated progress report data.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> UpdateAsync(ProgressReport progressReport);

        /// <summary>
        /// Applies partial updates to a progress report.
        /// </summary>
        /// <param name="progressReportId">The unique identifier of the progress report.</param>
        /// <param name="progressReport">The progress report data to patch.</param>
        /// <returns>True if the patch was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> PatchAsync(string progressReportId, ProgressReport progressReport);

        /// <summary>
        /// Deletes a progress report.
        /// </summary>
        /// <param name="progressReportId">The unique identifier of the progress report to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> DeleteAsync(string progressReportId);
    }
}
