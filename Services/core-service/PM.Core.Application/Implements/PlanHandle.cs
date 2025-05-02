using PM.Core.Application.Interfaces;
using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos.auths;
using PM.Shared.Dtos.cores;
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
        private readonly Position _ownerPosition;
        private readonly Position _managerPosition;

        public PlanHandle(IUnitOfWork<CoreDbContext> unitOfWork, IAPIService<UserDetail> userAPI, IPositionHandle positionHandle)
        {
            _unitOfWork = unitOfWork;
            _userAPI = userAPI;
            _positionHandle = positionHandle;
            _ownerPosition = _positionHandle.GetPositionByName("Product Owner").GetAwaiter().GetResult().Data;
            _managerPosition = _positionHandle.GetPositionByName("Project Manager").GetAwaiter().GetResult().Data;
        }

        /// <summary>
        /// Lấy danh sách kế hoạch theo project
        /// </summary>
        public async Task<ServiceResult<IEnumerable<IndexPlanModel>>> GetPlansProject(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return ServiceResult<IEnumerable<IndexPlanModel>>.Error("Project ID is required.");
            }

            try
            {
                ServiceResult<IEnumerable<Plan>> plans = await _unitOfWork.Repository<Plan>().GetManyAsync("ProjectId", projectId);
                if (plans.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexPlanModel>>.Error(plans.Message);
                }

                List<IndexPlanModel> result = plans.Data?.Select(x => new IndexPlanModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Goal = x.Goal,
                }).ToList() ?? new List<IndexPlanModel>();

                return ServiceResult<IEnumerable<IndexPlanModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexPlanModel>>.FromException(ex);
            }
        }

        /// <summary>
        /// Lấy chi tiết kế hoạch
        /// </summary>
        public async Task<ServiceResult<DetailPlanModel>> GetDetailPlan(string planId)
        {
            if (string.IsNullOrWhiteSpace(planId))
            {
                return ServiceResult<DetailPlanModel>.Error("Plan ID is required.");
            }

            try
            {
                ServiceResult<Plan> planResult = await _unitOfWork.Repository<Plan>().GetOneAsync("Id", planId);
                if (planResult.Status != ResultStatus.Success || planResult.Data == null)
                {
                    return ServiceResult<DetailPlanModel>.Error("Plan not found.");
                }

                ServiceResult<ProjectMember> memberResult = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", planResult.Data.ProjectMemberId);
                if (memberResult.Data == null)
                {
                    return ServiceResult<DetailPlanModel>.Error("Project member not found.");
                }

                ServiceResult<UserDetail> userResult = await _userAPI.APIsGetAsync($"api/user/get-user?userId={memberResult.Data.UserId}");
                if (userResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<DetailPlanModel>.Error(userResult.Message);
                }

                DetailPlanModel detail = new DetailPlanModel
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
            catch (Exception ex)
            {
                return ServiceResult<DetailPlanModel>.FromException(ex);
            }
        }

        /// <summary>
        /// Thêm mới kế hoạch
        /// </summary>
        public async Task<ServiceResult<DetailPlanModel>> AddAsync(string userId, AddPlanModel model)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(model.Name) ||
                string.IsNullOrWhiteSpace(model.Goal) ||
                string.IsNullOrWhiteSpace(model.ProjectId))
            {
                return ServiceResult<DetailPlanModel>.Error("Invalid input data.");
            }

            try
            {
                ServiceResult<IEnumerable<Plan>> plans = await _unitOfWork.Repository<Plan>().GetManyAsync("ProjectId", model.ProjectId);
                if (plans.Status != ResultStatus.Success)
                {
                    return ServiceResult<DetailPlanModel>.Error("Failed to fetch existing plans.");
                }

                if (plans.Data != null)
                {
                    ServiceResult<bool> isDuplicate = await _unitOfWork.Repository<Plan>().IsExistName(plans.Data, model.Name);
                    if (isDuplicate.Status != ResultStatus.Success || isDuplicate.Data)
                    {
                        return ServiceResult<DetailPlanModel>.Error("Plan name already exists.");
                    }
                }

                return await AddAction(userId, model);
            }
            catch (Exception ex)
            {
                return ServiceResult<DetailPlanModel>.FromException(ex);
            }
        }

        /// <summary>
        /// Cập nhật kế hoạch
        /// </summary>
        public async Task<ServiceResult<DetailPlanModel>> PacthAsync(string userId, string planId, PacthPlanModel model)
        {
            if (string.IsNullOrWhiteSpace(planId) ||
                string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(model.Name) ||
                string.IsNullOrWhiteSpace(model.Goal))
            {
                return ServiceResult<DetailPlanModel>.Error("Invalid input data.");
            }

            try
            {
                ServiceResult<Plan> planResult = await _unitOfWork.Repository<Plan>().GetOneAsync("Id", planId);
                if (planResult.Status != ResultStatus.Success || planResult.Data == null)
                {
                    return ServiceResult<DetailPlanModel>.Error("Plan not found.");
                }

                // Check user is Product Owner or Project Manager
                IEnumerable<ProjectMember> members = (await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", planResult.Data.ProjectId)).Data;
                bool hasPermission = members.Any(x => x.UserId == userId && (x.PositionId == _ownerPosition.Id || x.PositionId == _managerPosition.Id));
                if (!hasPermission)
                {
                    return ServiceResult<DetailPlanModel>.Error("You do not have permission to edit this plan.");
                }

                ServiceResult<IEnumerable<Plan>> plans = await _unitOfWork.Repository<Plan>().GetManyAsync("ProjectId", planResult.Data.ProjectId);
                if (plans.Status != ResultStatus.Success)
                {
                    return ServiceResult<DetailPlanModel>.Error("Could not verify name uniqueness.");
                }

                ServiceResult<bool> isDuplicate = await _unitOfWork.Repository<Plan>().IsExistName(plans.Data, model.Name);
                if (isDuplicate.Status != ResultStatus.Success || isDuplicate.Data)
                {
                    return ServiceResult<DetailPlanModel>.Error("Plan name already exists.");
                }

                return await PacthAction(planResult.Data, model);
            }
            catch (Exception ex)
            {
                return ServiceResult<DetailPlanModel>.FromException(ex);
            }
        }

        // ------------------------------------
        // PRIVATE HELPERS
        // ------------------------------------

        private async Task<ServiceResult<DetailPlanModel>> AddAction(string userId, AddPlanModel model)
        {
            IEnumerable<ProjectMember> members = (await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", model.ProjectId)).Data;
            ProjectMember? member = members.FirstOrDefault(x => x.UserId == userId);

            if (member == null)
            {
                return ServiceResult<DetailPlanModel>.Error("User is not a member of the project.");
            }

            Plan plan = new Plan
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                Goal = model.Goal,
                ProjectId = model.ProjectId,
                ProjectMemberId = member.Id,
                CreateDate = DateTime.Now,
                StartDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day),
                EndDate = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day),
            };

            plan.Status = (TypeStatus)new StatusHandle(plan.StartDate, plan.EndDate, false).GetStatus();

            ServiceResult<bool> result = await _unitOfWork.ExecuteTransactionAsync(() => _unitOfWork.Repository<Plan>().AddAsync(plan));
            if (result.Status != ResultStatus.Success)
            {
                return ServiceResult<DetailPlanModel>.Error("Failed to create plan.");
            }

            return await GetDetailPlan(plan.Id);
        }

        private async Task<ServiceResult<DetailPlanModel>> PacthAction(Plan plan, PacthPlanModel model)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { nameof(plan.Name), model.Name },
                { nameof(plan.Goal), model.Goal },
                { nameof(plan.StartDate), new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day) },
                { nameof(plan.EndDate), new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day) },
                { "Status", (TypeStatus)new StatusHandle( new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day), new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day), false).GetStatus() }
            };

            ServiceResult<bool> result = await _unitOfWork.ExecuteTransactionAsync(() => _unitOfWork.Repository<Plan>().PatchAsync(plan, parameters));
            if (result.Status != ResultStatus.Success)
            {
                return ServiceResult<DetailPlanModel>.Error("Failed to update plan.");
            }

            return await GetDetailPlan(plan.Id);
        }
    }

}
