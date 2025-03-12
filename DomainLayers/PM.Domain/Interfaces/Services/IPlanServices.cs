using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IPlanServices
    {
        public Task<ServicesResult<IEnumerable<Plan>>> GetPlans();
        public Task<ServicesResult<IEnumerable<Plan>>> GetPlansInProject(string projectId);
        public Task<ServicesResult<Plan>> GetDetailPlan(string planId);
        public Task<ServicesResult<bool>> AddAsync(Plan plan);
        public Task<ServicesResult<bool>> UpdateAsync(Plan plan);
        public Task<ServicesResult<bool>> PatchAsync(string planId, Plan plan);
        public Task<ServicesResult<bool>> DeleteAsync(string planId);
    }
}
