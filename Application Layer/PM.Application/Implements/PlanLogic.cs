using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Application.Interfaces;
using PM.Application.Models.plans;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.plans;

namespace PM.Application.Implements
{
    /// <summary>
    /// Handles business logic related to plans.
    /// </summary>
    public class PlanLogic : ControllerBase, IPlanLogic
    {
        private readonly IPlanServices _planServices;
        private readonly ILogger<PlanLogic> _logger;

        public PlanLogic(IPlanServices planServices, ILogger<PlanLogic> logger)
        {
            _planServices = planServices;
            _logger = logger;
        }

        #region GetPlans
        /// <summary>
        /// Retrieves all plans.
        /// </summary>
        /// <returns>List of plans</returns>
        public async Task<IActionResult> GetPlans()
        {
            try
            {
                var planResponse = await _planServices.GetPlans();
                if (!planResponse.Status) return BadRequest(planResponse.Message);
                return Ok(planResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving plans.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region GetPlansInProject
        /// <summary>
        /// Retrieves all plans associated with a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project</param>
        /// <returns>List of plans in the project</returns>
        public async Task<IActionResult> GetPlansInProject(string projectId)
        {
            try
            {
                var planResponse = await _planServices.GetPlansInProject(projectId);
                if (!planResponse.Status) return BadRequest(planResponse.Message);
                return Ok(planResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving plans for project {ProjectId}.", projectId);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region GetDetailPlan
        /// <summary>
        /// Retrieves details of a specific plan.
        /// </summary>
        /// <param name="planId">The ID of the plan</param>
        /// <returns>Plan details</returns>
        public async Task<IActionResult> GetDetailPlan(string planId)
        {
            try
            {
                var planResponse = await _planServices.GetDetailPlan(planId);
                if (!planResponse.Status) return BadRequest(planResponse.Message);
                return Ok(planResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving details for plan {PlanId}.", planId);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region AddPlan
        /// <summary>
        /// Adds a new plan.
        /// </summary>
        /// <param name="model">Plan details</param>
        /// <returns>Created plan</returns>
        public async Task<IActionResult> AddPlan(AddPlanModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid input model");
            try
            {
                var addPlan = new AddPlan
                {
                    Description = model.Description,
                    PlanName = model.PlanName,
                    StartAt = model.StartAt,
                    EndAt = model.EndAt
                };

                var planResponse = await _planServices.AddAsync(model.MemberId, model.ProjectId, addPlan);
                if (!planResponse.Status) return BadRequest(planResponse.Message);
                return Ok(planResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new plan.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region UpdateAsync
        /// <summary>
        /// Updates an existing plan.
        /// </summary>
        /// <param name="model">Updated plan details</param>
        /// <returns>Updated plan</returns>
        public async Task<IActionResult> UpdateAsync(UpdatePlanModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid input model");
            try
            {
                var updatePlan = new UpdatePlan
                {
                    Description = model.Description,
                    PlanName = model.PlanName,
                    StartAt = model.StartAt,
                    EndAt = model.EndAt
                };

                var planResponse = await _planServices.UpdateAsync(model.MemberId, model.PlanId, updatePlan);
                if (!planResponse.Status) return BadRequest(planResponse.Message);
                return Ok(planResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating plan {PlanId}.", model.PlanId);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region DeleteAsync
        /// <summary>
        /// Deletes a specific plan.
        /// </summary>
        /// <param name="model">Plan delete request</param>
        /// <returns>Result of delete operation</returns>
        public async Task<IActionResult> DeleteAsync(DeletePlanModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid input model");
            try
            {
                var planResponse = await _planServices.DeleteAsync(model.MemberId, model.PlanId);
                if (!planResponse.Status) return BadRequest(planResponse.Message);
                return Ok(planResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting plan {PlanId}.", model.PlanId);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion
    }
}