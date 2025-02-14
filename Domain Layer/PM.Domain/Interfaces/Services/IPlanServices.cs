using PM.Domain.Models.plans;

namespace PM.Domain.Interfaces.Services
{
    public interface IPlanServices
    {
        public Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlans();
        public Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlansInProject(string projectId);
        public Task<ServicesResult<DetailPlan>> GetDetailPlan(string planId);
        public Task<ServicesResult<DetailPlan>> AddAsync(string memberId, string projectId, AddPlan addPlan);
        public Task<ServicesResult<DetailPlan>> UpdateAsync(string memberId, string planId, UpdatePlan updatePlan);
        public Task<ServicesResult<IEnumerable<IndexPlan>>> DeleteAsync(string memberId, string planId);
    }
}
