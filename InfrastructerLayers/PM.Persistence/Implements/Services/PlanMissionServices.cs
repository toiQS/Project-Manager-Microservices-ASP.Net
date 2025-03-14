using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using PM.Domain.Interfaces.Services;

namespace PM.Persistence.Implements.Services
{
    public class PlanMissionServices : IPlanServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PlanMissionServices> _logger;

        public PlanMissionServices(IUnitOfWork unitOfWork, ILogger<PlanMissionServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region GET Methods
        public async Task<ServicesResult<IEnumerable<Plan>>> GetPlans()
        {
            _logger.LogInformation("[Service] Fetching all Plans...");
            var response = await _unitOfWork.PlanQueryRepository.GetAllAsync(1, 100);

            if (!response.Status)
            {
                _logger.LogError("[Service] GetPlans failed: {Message}", response.Message);
                return ServicesResult<IEnumerable<Plan>>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully fetched {Count} Plans", response.Data?.Count());
            return ServicesResult<IEnumerable<Plan>>.Success(response.Data!);
        }

        public async Task<ServicesResult<IEnumerable<Plan>>> GetPlansInProject(string projectId)
        {
            _logger.LogInformation("[Service] Fetching Plans for ProjectId={ProjectId}", projectId);
            var response = await _unitOfWork.PlanQueryRepository.GetManyByKeyAndValue("ProjectId", projectId);

            if (!response.Status)
            {
                _logger.LogError("[Service] GetPlansInProject failed for ProjectId={ProjectId}: {Message}", projectId, response.Message);
                return ServicesResult<IEnumerable<Plan>>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Found {Count} Plans for ProjectId={ProjectId}", response.Data?.Count(), projectId);
            return ServicesResult<IEnumerable<Plan>>.Success(response.Data!);
        }

        public async Task<ServicesResult<Plan>> GetDetailPlan(string planId)
        {
            _logger.LogInformation("[Service] Fetching Plan details: Id={PlanId}", planId);
            var response = await _unitOfWork.PlanQueryRepository.GetOneByKeyAndValue("Id", planId);

            if (!response.Status)
            {
                _logger.LogError("[Service] GetDetailPlan failed for Id={PlanId}: {Message}", planId, response.Message);
                return ServicesResult<Plan>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully fetched Plan: Id={PlanId}", planId);
            return ServicesResult<Plan>.Success(response.Data!);
        }
        #endregion

        #region CREATE/UPDATE Methods
        public async Task<ServicesResult<bool>> AddAsync(Plan plan)
        {
            _logger.LogInformation("[Service] Adding new Plan: ProjectId={ProjectId}", plan.ProjectId);
            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.PlanCommandRepository.AddAsync(new List<Plan> { plan }, plan)
            );

            if (!response.Status)
            {
                _logger.LogError("[Service] AddAsync failed: {Message}", response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully added Plan: Id={PlanId}", plan.Id);
            return ServicesResult<bool>.Success(true);
        }

        public async Task<ServicesResult<bool>> UpdateAsync(Plan plan)
        {
            _logger.LogInformation("[Service] Updating Plan: Id={PlanId}", plan.Id);
            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.PlanCommandRepository.UpdateAsync(new List<Plan> { plan }, plan)
            );

            if (!response.Status)
            {
                _logger.LogError("[Service] UpdateAsync failed for Id={PlanId}: {Message}", plan.Id, response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully updated Plan: Id={PlanId}", plan.Id);
            return ServicesResult<bool>.Success(true);
        }

        public async Task<ServicesResult<bool>> PatchAsync(string planId, Plan plan)
        {
            _logger.LogInformation("[Service] Patching Plan: Id={PlanId}", planId);
            var keyValuePairs = new Dictionary<string, object>
            {
                {"ProjectId", plan.ProjectId},
                {"Name", plan.Name},
                {"Description", plan.Description},
                {"StartDate", plan.StartDate},
                {"EndDate", plan.EndDate}
            };

            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.PlanCommandRepository.PatchAsync(new List<Plan> { plan }, planId, keyValuePairs)
            );

            if (!response.Status)
            {
                _logger.LogError("[Service] PatchAsync failed for Id={PlanId}: {Message}", planId, response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully patched Plan: Id={PlanId}", planId);
            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region DELETE Methods
        public async Task<ServicesResult<bool>> DeleteAsync(string planId)
        {
            _logger.LogInformation("[Service] Deleting Plan: Id={PlanId}", planId);
            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.PlanCommandRepository.DeleteAsync(planId)
            );

            if (!response.Status)
            {
                _logger.LogError("[Service] DeleteAsync failed for Id={PlanId}: {Message}", planId, response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully deleted Plan: Id={PlanId}", planId);
            return ServicesResult<bool>.Success(true);
        }

        public async Task<ServicesResult<bool>> DeleteMissionsInPlan(string planId)
        {
            _logger.LogInformation("[Service] Deleting Missions for PlanId={PlanId}", planId);
            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionCommandRepository.DeleteManyAsync("PlanId", planId)
            );

            if (!response.Status)
            {
                _logger.LogError("[Service] DeleteMissionsInPlan failed for PlanId={PlanId}: {Message}", planId, response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully deleted Missions for PlanId={PlanId}", planId);
            return ServicesResult<bool>.Success(true);
        }
        #endregion
    }
}
