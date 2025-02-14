using PM.Domain.Models.plans;

namespace PM.Domain.Interfaces.Services
{
    internal class PlanServices
    {
        private readonly  IUnitOfWork _unitOfWork;
        private string _own;
        private string _leader;
        private string _manager;
        //eveyone can create, update, and delete when your role is owner, leader, manager
        public async Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlans()
        {
            try
            {
                var plans = await _unitOfWork.PlanRepository.GetAllAsync();
                if(plans.Status == false ) return ServicesResult<IEnumerable<IndexPlan>>.Failure(plans.Message);
                var response = plans.Data.Select(x => new IndexPlan()
                {
                    PlanName = x.Name,
                    Description = x.Description,
                    PlanId = x.Id,
                }).ToList();
                return ServicesResult<IEnumerable<IndexPlan>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexPlan>>.Failure("");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlansInProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId)) return ServicesResult<IEnumerable<IndexPlan>>.Failure("");
            try
            {
                var planProjects = await _unitOfWork.PlanRepository.GetManyByKeyAndValue("ProjectId",projectId);
                if (planProjects.Status == false ) return ServicesResult<IEnumerable<IndexPlan>>.Failure(planProjects.Message);
                var response = planProjects.Data.Select(x => new IndexPlan()
                {
                    PlanName = x.Name,
                    Description = x.Description,
                    PlanId = x.Id,
                }).ToList();
                return ServicesResult<IEnumerable<IndexPlan>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexPlan>>.Failure("");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ServicesResult<DetailPlan>> GetDetailPlan(string planId)
        {
            if (string.IsNullOrEmpty(planId)) return ServicesResult<DetailPlan>.Failure("");
            try
            {
                var plan = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id",planId);
                if(plan.Status == false) return ServicesResult<DetailPlan>.Failure(plan.Message); 
                var response = new DetailPlan()
                {
                    PlanId = planId,
                    PlanName = plan.Data.Name,
                    Description = plan.Data.Description,
                    StartDate = new DateOnly(plan.Data.StartDate.Year, plan.Data.StartDate.Month, plan.Data.StartDate.Day),
                    EndDate = new DateOnly(plan.Data.EndDate.Year, plan.Data.EndDate.Month, plan.Data.EndDate.Day),
                    IsCompleted = plan.Data.IsCompleted,
                };
                var statusResponse = await _unitOfWork.StatusRepository.GetOneByKeyAndValue("Id", plan.Data.StatusId);
                if(statusResponse.Status == false) return ServicesResult<DetailPlan>.Failure(statusResponse.Message);
                response.Status = statusResponse.Data.Name;
                var missions = await _unitOfWork.MissionRepository.GetManyByKeyAndValue("PlanId", planId);
                if(missions.Status == false) return ServicesResult<DetailPlan>.Failure(missions.Message);
                response.Missions = missions.Data
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _unitOfWork.Dispose();
            }

        }
        public Task<ServicesResult<DetailPlan>> AddAsync(string memberId, string projectId, AddPlan addPlan);
        public Task<ServicesResult<DetailPlan>> UpdateAsync(string memberId, UpdatePlan updatePlan);
        public Task<ServicesResult<IEnumerable<IndexPlan>>> DeleteAsync(string memberId, string planId);
    }
}
