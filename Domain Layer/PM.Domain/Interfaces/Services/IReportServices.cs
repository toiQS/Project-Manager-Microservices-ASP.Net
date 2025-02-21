using PM.Domain.Models.reports;

namespace PM.Domain.Interfaces.Services
{
    public interface IReportServices
    {
        public Task<ServicesResult<IEnumerable<IndexReport>>> GetReportsInPlan(string planId);
        public Task<ServicesResult<string>> AddReport(string memberId, string planId, string reportDetail);
        public Task<ServicesResult<string>> UpdateReport(string memberId, string reportId, string reportDetail);
        public Task<ServicesResult<string>> DeleteReport(string memberId, string reportId);
    }
}
