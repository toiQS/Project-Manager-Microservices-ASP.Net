using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Application.Interfaces;
using PM.Application.Models.reports;
using PM.Domain.Interfaces.Services;

namespace PM.Application.Implements
{
    public class ReportLogic : ControllerBase, IReportLogic
    {
        private readonly IReportServices _reportServices;
        private readonly ILogger<ReportLogic> _logger;
        public ReportLogic(IReportServices reportServices, ILogger<ReportLogic> logger)
        {
            _reportServices = reportServices;
            _logger = logger;
        }
        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetReportsInPlan(string planId)
        {
            try
            {
                var reportResponse = await _reportServices.GetReportsInPlan(planId);
                if (reportResponse.Status == false) return BadRequest(reportResponse.Message);
                return Ok(reportResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving joined products.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddReport(AddReportModel model)
        {
            if (!ModelState.IsValid) return BadRequest("");
            try
            {
                var reportResponse = await _reportServices.AddReport(model.MemberId, model.PlanId, model.ReportDetail);
                if (reportResponse.Status == false) return BadRequest(reportResponse.Message);
                return Ok(reportResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving joined products.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateReport(UpdateReportModel model)
        {
            if (!ModelState.IsValid) return BadRequest("");
            try
            {
                var reportResponse = await _reportServices.UpdateReport(model.MemberId, model.ReportId, model.ReportDetail);
                if (reportResponse.Status == false) return BadRequest(reportResponse.Message);
                return Ok(reportResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving joined products.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteReport(DeleteReportModel model)
        {
            if (!ModelState.IsValid) return BadRequest("");
            try
            {
                var reportResponse = await _reportServices.DeleteReport(model.MemberId, model.ReportId);
                if (reportResponse.Status == false) return BadRequest(reportResponse.Message);
                return Ok(reportResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving joined products.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion
    }
}
