using Microsoft.AspNetCore.Mvc;
using PM.Application.Models.plans;

namespace PM.Application.Interfaces
{
    public interface IPlanLogic
    {
        public Task<IActionResult> GetPlans();
        public Task<IActionResult> GetPlansInProject(string projectId);
        public Task<IActionResult> GetDetailPlan(string planId);
        public Task<IActionResult> AddPlan(AddPlanModel model);
        public Task<IActionResult> UpdateAsync(UpdatePlanModel model);
        public Task<IActionResult> DeleteAsync(DeletePlanModel model);
    }
}
