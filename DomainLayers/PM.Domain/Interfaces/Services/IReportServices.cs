using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IReportServices
    {
        public Task<ServicesResult<IEnumerable<ProgressReport>>> GetReports();
        public Task<ServicesResult<IEnumerable<ProgressReport>>> GetReportsInPlan(string planId);
        public Task<ServicesResult<bool>> AddAsync(ProgressReport progressReport);
        public Task<ServicesResult<bool>> UpdateAsync(ProgressReport progressReport);
        public Task<ServicesResult<bool>> PatchAsync(string progressReportId, ProgressReport progressReport);
        public Task<ServicesResult<bool>> DeleteAsync(string progressReportId);
    }
}
