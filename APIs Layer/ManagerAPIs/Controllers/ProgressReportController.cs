using Microsoft.AspNetCore.Mvc;
using PM.Application.Interfaces;
using PM.Application.Models.reports;
using System.Threading.Tasks;

namespace ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressReportController : ControllerBase
    {
        private readonly IReportLogic _reportLogic;

        public ProgressReportController(IReportLogic reportLogic)
        {
            _reportLogic = reportLogic;
        }

        #region GetReportsInPlan
        /// <summary>
        /// Retrieves all reports within a specific plan.
        /// </summary>
        [HttpGet("get-reports-in-plan")]
        public async Task<IActionResult> GetReportsInPlan([FromQuery] string planId)
        {
            return await _reportLogic.GetReportsInPlan(planId);
        }
        #endregion

        #region AddReport
        /// <summary>
        /// Adds a new report.
        /// </summary>
        [HttpPost("add-report")]
        public async Task<IActionResult> AddReport([FromBody] AddReportModel model)
        {
            return await _reportLogic.AddReport(model);
        }
        #endregion

        #region UpdateReport
        /// <summary>
        /// Updates an existing report.
        /// </summary>
        [HttpPut("update-report")]
        public async Task<IActionResult> UpdateReport([FromBody] UpdateReportModel model)
        {
            return await _reportLogic.UpdateReport(model);
        }
        #endregion

        #region DeleteReport
        /// <summary>
        /// Deletes a report.
        /// </summary>
        [HttpDelete("delete-report")]
        public async Task<IActionResult> DeleteReport([FromBody] DeleteReportModel model)
        {
            return await _reportLogic.DeleteReport(model);
        }
        #endregion
    }
}