// Refactored PlanHandle.cs - Clean Code, SOLID, DDD Principles

using PM.Core.Application.Interfaces;
using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos.auths;
using PM.Shared.Dtos.cores;
using PM.Shared.Dtos.cores.missions;
using PM.Shared.Dtos.cores.plans;
using PM.Shared.Dtos.users;
using PM.Shared.Handle.Implements;
using PM.Shared.Handle.Interfaces;

namespace PM.Core.Application.Implements
{
    public class PlanHandle
    {
        private readonly IUnitOfWork<CoreDbContext> _unitOfWork;
        private readonly IAPIService<UserDetail> _userAPI;
        private readonly IPositionHandle _positionHandle;
        private readonly IMissionHandle _missionHandle;

        private Position _ownerPosition;
        private Position _managerPosition;

        public PlanHandle(
            IUnitOfWork<CoreDbContext> unitOfWork,
            IAPIService<UserDetail> userAPI,
            IPositionHandle positionHandle,
            IMissionHandle missionHandle)
        {
            _unitOfWork = unitOfWork;
            _userAPI = userAPI;
            _positionHandle = positionHandle;
            _missionHandle = missionHandle;
            InitializeAsync().GetAwaiter().GetResult();
        }

        public async Task InitializeAsync()
        {
            _ownerPosition = (await _positionHandle.GetPositionByName("Product Owner")).Data;
            _managerPosition = (await _positionHandle.GetPositionByName("Project Manager")).Data;
        }

        public async Task<ServiceResult<IEnumerable<IndexPlanModel>>> GetPlansProject(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
                return ServiceResult<IEnumerable<IndexPlanModel>>.Error("Project ID is required.");

            var plans = await _unitOfWork.Repository<Plan>().GetManyAsync("ProjectId", projectId);
            if (plans.Status != ResultStatus.Success)
                return ServiceResult<IEnumerable<IndexPlanModel>>.Error(plans.Message);

            var result = plans.Data?.Select(x => new IndexPlanModel
            {
                Id = x.Id,
                Name = x.Name,
                Goal = x.Goal,
            }).ToList() ?? new List<IndexPlanModel>();

            return ServiceResult<IEnumerable<IndexPlanModel>>.Success(result);
        }

        public async Task<ServiceResult<DetailPlanModel>> GetDetailPlan(string planId)
        {
            if (string.IsNullOrWhiteSpace(planId))
                return ServiceResult<DetailPlanModel>.Error("Plan ID is required.");

            var planResult = await _unitOfWork.Repository<Plan>().GetOneAsync("Id", planId);
            if (planResult.Status != ResultStatus.Success || planResult.Data == null)
                return ServiceResult<DetailPlanModel>.Error("Plan not found.");

            var memberResult = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", planResult.Data.ProjectMemberId);
            if (memberResult.Data == null)
                return ServiceResult<DetailPlanModel>.Error("Project member not found.");

            var userResult = await _userAPI.APIsGetAsync($"api/user/get-user?userId={memberResult.Data.UserId}");
            if (userResult.Status != ResultStatus.Success)
                return ServiceResult<DetailPlanModel>.Error(userResult.Message);

            var detail = new DetailPlanModel
            {
                Id = planResult.Data.Id,
                Name = planResult.Data.Name,
                Goal = planResult.Data.Goal,
                CreateBy = userResult.Data.UserName,
                CreateDate = planResult.Data.CreateDate,
                StartDate = planResult.Data.StartDate,
                EndDate = planResult.Data.EndDate,
                ProjectId = planResult.Data.ProjectId,
                Status = planResult.Data.Status.ToString()
            };

            return ServiceResult<DetailPlanModel>.Success(detail);
        }

        public async Task<ServiceResult<DetailPlanModel>> AddAsync(string userId, AddPlanModel model)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(model.Name) ||
                string.IsNullOrWhiteSpace(model.Goal) ||
                string.IsNullOrWhiteSpace(model.ProjectId))
                return ServiceResult<DetailPlanModel>.Error("Invalid input data.");

            var plans = await _unitOfWork.Repository<Plan>().GetManyAsync("ProjectId", model.ProjectId);
            if (plans.Status != ResultStatus.Success)
                return ServiceResult<DetailPlanModel>.Error("Failed to fetch existing plans.");

            if (plans.Data != null)
            {
                var isDuplicate = await _unitOfWork.Repository<Plan>().IsExistName(plans.Data, model.Name);
                if (isDuplicate.Status != ResultStatus.Success || isDuplicate.Data)
                    return ServiceResult<DetailPlanModel>.Error("Plan name already exists.");
            }

            return await AddAction(userId, model);
        }

        public async Task<ServiceResult<DetailPlanModel>> PatchAsync(string userId, string planId, PacthPlanModel model)
        {
            if (string.IsNullOrWhiteSpace(planId) ||
                string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(model.Name) ||
                string.IsNullOrWhiteSpace(model.Goal))
                return ServiceResult<DetailPlanModel>.Error("Invalid input data.");

            var planResult = await _unitOfWork.Repository<Plan>().GetOneAsync("Id", planId);
            if (planResult.Status != ResultStatus.Success || planResult.Data == null)
                return ServiceResult<DetailPlanModel>.Error("Plan not found.");

            var members = (await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", planResult.Data.ProjectId)).Data;
            if (!HasManagerPermission(userId, members))
                return ServiceResult<DetailPlanModel>.Error("Permission denied.");

            var plans = await _unitOfWork.Repository<Plan>().GetManyAsync("ProjectId", planResult.Data.ProjectId);
            if (plans.Status != ResultStatus.Success)
                return ServiceResult<DetailPlanModel>.Error("Could not verify name uniqueness.");

            var isDuplicate = await _unitOfWork.Repository<Plan>().IsExistName(plans.Data, model.Name);
            if (isDuplicate.Status != ResultStatus.Success || isDuplicate.Data)
                return ServiceResult<DetailPlanModel>.Error("Plan name already exists.");

            return await PatchAction(planResult.Data, model);
        }

        private async Task<ServiceResult<DetailPlanModel>> AddAction(string userId, AddPlanModel model)
        {
            var members = (await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", model.ProjectId)).Data;
            var member = members.FirstOrDefault(x => x.UserId == userId);

            if (member == null)
                return ServiceResult<DetailPlanModel>.Error("User is not a member of the project.");

            var plan = new Plan
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                Goal = model.Goal,
                ProjectId = model.ProjectId,
                ProjectMemberId = member.Id,
                CreateDate = DateTime.Now,
                StartDate = new DateTime(model.StartDate.Day, model.StartDate.Month, model.StartDate.Year),
                EndDate = new DateTime(model.EndDate.Day, model.EndDate.Month, model.EndDate.Year),
                Status = (TypeStatus)new StatusHandle(new DateTime(model.StartDate.Day, model.StartDate.Month, model.StartDate.Year), new DateTime(model.EndDate.Day, model.EndDate.Month, model.EndDate.Year), false).GetStatus()
            };

            var result = await _unitOfWork.ExecuteTransactionAsync(() => _unitOfWork.Repository<Plan>().AddAsync(plan));
            return result.Status != ResultStatus.Success
                ? ServiceResult<DetailPlanModel>.Error("Failed to create plan.")
                : await GetDetailPlan(plan.Id);
        }

        private async Task<ServiceResult<DetailPlanModel>> PatchAction(Plan plan, PacthPlanModel model)
        {
            var parameters = new Dictionary<string, object>
            {
                { nameof(plan.Name), model.Name },
                { nameof(plan.Goal), model.Goal },
                { nameof(plan.StartDate), new DateTime(model.StartDate.Day, model.StartDate.Month, model.StartDate.Year) },
                { nameof(plan.EndDate), new DateTime(model.EndDate.Day, model.EndDate.Month, model.EndDate.Year) },
                { "Status", (TypeStatus)new StatusHandle(new DateTime(model.StartDate.Day, model.StartDate.Month, model.StartDate.Year), new DateTime(model.EndDate.Day, model.EndDate.Month, model.EndDate.Year), false).GetStatus() }
            };

            var result = await _unitOfWork.ExecuteTransactionAsync(() => _unitOfWork.Repository<Plan>().PatchAsync(plan, parameters));
            return result.Status != ResultStatus.Success
                ? ServiceResult<DetailPlanModel>.Error("Failed to update plan.")
                : await GetDetailPlan(plan.Id);
        }

        private bool HasManagerPermission(string userId, IEnumerable<ProjectMember> members)
        {
            return members.Any(x => x.UserId == userId &&
                (x.PositionId == _managerPosition.Id || x.PositionId == _ownerPosition.Id));
        }
        public async Task<ServiceResult<IEnumerable<IndexPlanModel>>> DeleteAsync(string userId, string planId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(planId))
                return ServiceResult<IEnumerable<IndexPlanModel>>.Error("User ID and Plan ID are required.");

            try
            {
                var planResult = await _unitOfWork.Repository<Plan>().GetOneAsync("Id", planId);
                if (planResult.Status != ResultStatus.Success || planResult.Data == null)
                    return ServiceResult<IEnumerable<IndexPlanModel>>.Error("Plan not found.");

                if (!await HasManagerPermission(userId, planResult.Data.ProjectId))
                    return ServiceResult<IEnumerable<IndexPlanModel>>.Error("Permission denied.");

                var deleteMissionResult = await _missionHandle.DeleteManyAsync(userId, planId);
                if (deleteMissionResult.Status != ResultStatus.Success)
                    return ServiceResult<IEnumerable<IndexPlanModel>>.Error("Failed to delete missions in plan.");

                var deletePlanResult = await _unitOfWork.ExecuteTransactionAsync(
                    () => _unitOfWork.Repository<Plan>().DeleteAsync(planResult.Data)
                );

                return deletePlanResult.Status != ResultStatus.Success
                    ? ServiceResult<IEnumerable<IndexPlanModel>>.Error("Failed to delete plan.")
                    : await GetPlansProject(planResult.Data.ProjectId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexPlanModel>>.FromException(ex);
            }
        }

        public async Task<ServiceResult<IEnumerable<IndexPlanModel>>> DeleteManyAsync(string userId, string projectId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(projectId))
                return ServiceResult<IEnumerable<IndexPlanModel>>.Error("User ID and Project ID are required.");

            try
            {
                var plansResult = await _unitOfWork.Repository<Plan>().GetManyAsync("ProjectId", projectId);
                if (plansResult.Status != ResultStatus.Success)
                    return ServiceResult<IEnumerable<IndexPlanModel>>.Error("Could not retrieve plans.");

                var plans = plansResult.Data?.ToList();
                if (plans == null || !plans.Any())
                    return await GetPlansProject(projectId);

                foreach (var plan in plans)
                {
                    var deleteMissionResult = await _missionHandle.DeleteManyAsync(userId, plan.Id);
                    if (deleteMissionResult.Status != ResultStatus.Success)
                        return ServiceResult<IEnumerable<IndexPlanModel>>.Error($"Failed to delete missions in plan: {plan.Name}");
                }

                var deletePlansResult = await _unitOfWork.ExecuteTransactionAsync(
                    () => _unitOfWork.Repository<Plan>().DeleteAsync(plans)
                );

                return deletePlansResult.Status != ResultStatus.Success
                    ? ServiceResult<IEnumerable<IndexPlanModel>>.Error("Failed to delete plans.")
                    : await GetPlansProject(projectId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexPlanModel>>.FromException(ex);
            }
        }

        private async Task<bool> HasManagerPermission(string userId, string projectId)
        {
            var memberResult = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", projectId);
            if (memberResult.Status != ResultStatus.Success || memberResult.Data == null)
                return false;

            return memberResult.Data.Any(x =>
                x.UserId == userId &&
                (x.PositionId == _managerPosition.Id || x.PositionId == _ownerPosition.Id)
            );
        }

    }
}
