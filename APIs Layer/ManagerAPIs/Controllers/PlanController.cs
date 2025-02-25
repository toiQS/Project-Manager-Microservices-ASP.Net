using Microsoft.AspNetCore.Mvc;
using PM.Application.Interfaces;
using PM.Application.Models.plans;
using System.Threading.Tasks;

namespace ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly IPlanLogic _planLogic;

        public PlanController(IPlanLogic planLogic)
        {
            _planLogic = planLogic;
        }

        #region GetPlans
        /// <summary>
        /// Retrieves all plans.
        /// </summary>
        [HttpGet("get-plans")]
        public async Task<IActionResult> GetPlans()
        {
            return await _planLogic.GetPlans();
        }
        #endregion

        #region GetPlansInProject
        /// <summary>
        /// Retrieves all plans within a specific project.
        /// </summary>
        [HttpGet("get-plans-in-project")]
        public async Task<IActionResult> GetPlansInProject([FromQuery] string projectId)
        {
            return await _planLogic.GetPlansInProject(projectId);
        }
        #endregion

        #region GetDetailPlan
        /// <summary>
        /// Retrieves details of a specific plan.
        /// </summary>
        [HttpGet("get-detail-plan")]
        public async Task<IActionResult> GetDetailPlan([FromQuery] string planId)
        {
            return await _planLogic.GetDetailPlan(planId);
        }
        #endregion

        #region AddPlan
        /// <summary>
        /// Adds a new plan.
        /// </summary>
        [HttpPost("add-plan")]
        public async Task<IActionResult> AddPlan([FromBody] AddPlanModel model)
        {
            return await _planLogic.AddPlan(model);
        }
        #endregion

        #region UpdatePlan
        /// <summary>
        /// Updates an existing plan.
        /// </summary>
        [HttpPut("update-plan")]
        public async Task<IActionResult> UpdatePlan([FromBody] UpdatePlanModel model)
        {
            return await _planLogic.UpdateAsync(model);
        }
        #endregion

        #region DeletePlan
        /// <summary>
        /// Deletes a plan.
        /// </summary>
        [HttpDelete("delete-plan")]
        public async Task<IActionResult> DeletePlan([FromBody] DeletePlanModel model)
        {
            return await _planLogic.DeleteAsync(model);
        }
        #endregion
    }
}