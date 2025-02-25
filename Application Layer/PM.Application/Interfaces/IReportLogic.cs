using Microsoft.AspNetCore.Mvc;
using PM.Application.Models.reports;

namespace PM.Application.Interfaces
{
    public interface IReportLogic
    {
        public Task<IActionResult> GetReportsInPlan(string planId);
        public Task<IActionResult> AddReport(AddReportModel model);
        public Task<IActionResult> UpdateReport(UpdateReportModel model);
        public Task<IActionResult> DeleteReport(DeleteReportModel model);
    }
}
