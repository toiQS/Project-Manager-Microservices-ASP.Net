namespace PM.Core.Application.Implements
{
    //public class MissionHandle : IMissionHandle
    //{
    //    private readonly IUnitOfWork<CoreDbContext> _unitOfWork;
    //    private readonly IAPIService<UserDetail> _userAPI;
    //    private readonly IMissionMemberHandle _missionMemberHandle;

    //    public MissionHandle(
    //        IUnitOfWork<CoreDbContext> unitOfWork,
    //        IAPIService<UserDetail> userAPI,
    //        IMissionMemberHandle missionMemberHandle)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _userAPI = userAPI;
    //        _missionMemberHandle = missionMemberHandle;
    //    }

    //    /// <summary>
    //    /// Lấy danh sách nhiệm vụ theo PlanId.
    //    /// </summary>
    //    public async Task<ServiceResult<IEnumerable<IndexMissionModel>>> GetAsync(string planId)
    //    {
    //        if (string.IsNullOrEmpty(planId))
    //        {
    //            return ServiceResult<IEnumerable<IndexMissionModel>>.Error("PlanId không hợp lệ.");
    //        }

    //        try
    //        {
    //            ServiceResult<IEnumerable<Mission>> missions = await _unitOfWork.Repository<Mission>().GetManyAsync("PlanId", planId);
    //            if (missions.Status != ResultStatus.Success)
    //            {
    //                return ServiceResult<IEnumerable<IndexMissionModel>>.Error("Không lấy được danh sách nhiệm vụ.");
    //            }

    //            List<IndexMissionModel> result = missions.Data.Select(m => new IndexMissionModel
    //            {
    //                Id = m.Id,
    //                Name = m.Name,
    //                Request = m.Request
    //            }).ToList();

    //            return ServiceResult<IEnumerable<IndexMissionModel>>.Success(result);
    //        }
    //        catch (Exception ex)
    //        {
    //            return ServiceResult<IEnumerable<IndexMissionModel>>.FromException(ex);
    //        }
    //    }

    //    /// <summary>
    //    /// Lấy thông tin chi tiết một nhiệm vụ.
    //    /// </summary>
    //    public async Task<ServiceResult<DetailMissionModel>> GetDetailAsync(string missionId)
    //    {
    //        if (string.IsNullOrEmpty(missionId))
    //        {
    //            return ServiceResult<DetailMissionModel>.Error("MissionId không hợp lệ.");
    //        }

    //        try
    //        {
    //            ServiceResult<Mission> mission = await _unitOfWork.Repository<Mission>().GetOneAsync("Id", missionId);
    //            if (mission.Status != ResultStatus.Success)
    //            {
    //                return ServiceResult<DetailMissionModel>.Error("Không tìm thấy nhiệm vụ.");
    //            }

    //            ServiceResult<ProjectMember> member = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", mission.Data.ProjectMemberId);
    //            if (member.Status != ResultStatus.Success)
    //            {
    //                return ServiceResult<DetailMissionModel>.Error("Không tìm thấy thành viên dự án.");
    //            }

    //            ServiceResult<UserDetail> user = await _userAPI.APIsGetAsync($"/api/user/get-user?userId={member.Data.UserId}");

    //            DetailMissionModel detail = new()
    //            {
    //                Id = mission.Data.Id,
    //                Name = mission.Data.Name,
    //                CreateDate = mission.Data.CreateDate,
    //                StartDate = mission.Data.StartDate,
    //                EndDate = mission.Data.EndDate,
    //                Goal = mission.Data.Request,
    //                CreateBy = user.Data.UserName
    //            };

    //            return ServiceResult<DetailMissionModel>.Success(detail);
    //        }
    //        catch (Exception ex)
    //        {
    //            return ServiceResult<DetailMissionModel>.FromException(ex);
    //        }
    //    }

    //    /// <summary>
    //    /// Thêm mới một nhiệm vụ.
    //    /// </summary>
    //    public async Task<ServiceResult<DetailMissionModel>> AddAsync(string userId, AddMissonModel model)
    //    {
    //        if (string.IsNullOrEmpty(userId) || model == null ||
    //            string.IsNullOrEmpty(model.PlanId) ||
    //            string.IsNullOrEmpty(model.ProjectMemberId) ||
    //            string.IsNullOrEmpty(model.Name) ||
    //            string.IsNullOrEmpty(model.Request))
    //        {
    //            return ServiceResult<DetailMissionModel>.Error("Thông tin không hợp lệ.");
    //        }

    //        try
    //        {
    //            ServiceResult<Plan> plan = await _unitOfWork.Repository<Plan>().GetOneAsync("Id", model.PlanId);
    //            if (plan.Status != ResultStatus.Success)
    //            {
    //                return ServiceResult<DetailMissionModel>.Error("Không tìm thấy kế hoạch.");
    //            }

    //            ServiceResult<Project> project = await _unitOfWork.Repository<Project>().GetOneAsync("Id", plan.Data.ProjectId);
    //            if (project.Status != ResultStatus.Success)
    //            {
    //                return ServiceResult<DetailMissionModel>.Error("Không tìm thấy dự án.");
    //            }

    //            IEnumerable<ProjectMember> members = (await _unitOfWork.Repository<ProjectMember>().GetManyAsync("UserId", userId)).Data
    //                .Where(m => m.ProjectId == project.Data.Id);
    //            if (!members.Any())
    //            {
    //                return ServiceResult<DetailMissionModel>.Error("Không tìm thấy thành viên dự án.");
    //            }

    //            Mission mission = new()
    //            {
    //                Id = Guid.NewGuid().ToString(),
    //                Name = model.Name,
    //                ProjectMemberId = project.Data.Id,
    //                PlanId = plan.Data.Id,
    //                CreateDate = DateTime.Now,
    //                StartDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day),
    //                EndDate = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day),
    //                Request = model.Request,
    //                Status = (TypeStatus)new StatusHandle(
    //                    new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day),
    //                    new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day),
    //                    false).GetStatus()
    //            };

    //            ServiceResult<bool> result = await _unitOfWork.ExecuteTransactionAsync(() =>
    //                _unitOfWork.Repository<Mission>().AddAsync(mission));

    //            return result.Status != ResultStatus.Success
    //                ? ServiceResult<DetailMissionModel>.Error("Thêm nhiệm vụ thất bại.")
    //                : await GetDetailAsync(mission.Id);
    //        }
    //        catch (Exception ex)
    //        {
    //            return ServiceResult<DetailMissionModel>.FromException(ex);
    //        }
    //    }

    //    /// <summary>
    //    /// Cập nhật thông tin nhiệm vụ.
    //    /// </summary>
    //    public async Task<ServiceResult<DetailMissionModel>> PatchMissionAsync(string userId, string missionId, PatchMissionModel model)
    //    {
    //        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(missionId) ||
    //            string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Request))
    //        {
    //            return ServiceResult<DetailMissionModel>.Error("Thông tin không hợp lệ.");
    //        }

    //        try
    //        {
    //            ServiceResult<Mission> mission = await _unitOfWork.Repository<Mission>().GetOneAsync("Id", missionId);
    //            if (mission.Status != ResultStatus.Success)
    //            {
    //                return ServiceResult<DetailMissionModel>.Error("Không tìm thấy nhiệm vụ.");
    //            }

    //            ServiceResult<Plan> plan = await _unitOfWork.Repository<Plan>().GetOneAsync("Id", mission.Data.PlanId);
    //            ServiceResult<Project> project = await _unitOfWork.Repository<Project>().GetOneAsync("Id", plan.Data.ProjectId);
    //            ProjectMember? member = (await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", project.Data.Id)).Data
    //                .FirstOrDefault(m => m.UserId == userId);

    //            if (member == null || mission.Data.ProjectMemberId != member.Id)
    //            {
    //                return ServiceResult<DetailMissionModel>.Error("Không có quyền chỉnh sửa nhiệm vụ này.");
    //            }

    //            Dictionary<string, object> patchData = new()
    //            {
    //            { nameof(model.Name), model.Name },
    //            { nameof(model.Request), model.Request },
    //            { nameof(model.StartDate), new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day) },
    //            { nameof(model.EndDate), new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day) }
    //        };

    //            ServiceResult<bool> patchResult = await _unitOfWork.ExecuteTransactionAsync(() =>
    //                _unitOfWork.Repository<Mission>().PatchAsync(mission.Data, patchData));

    //            return patchResult.Status != ResultStatus.Success
    //                ? ServiceResult<DetailMissionModel>.Error("Cập nhật thất bại.")
    //                : await GetDetailAsync(missionId);
    //        }
    //        catch (Exception ex)
    //        {
    //            return ServiceResult<DetailMissionModel>.FromException(ex);
    //        }
    //    }

    //    /// <summary>
    //    /// Xóa một nhiệm vụ cụ thể.
    //    /// </summary>
    //    public async Task<ServiceResult<IEnumerable<IndexMissionModel>>> DeleteAsync(string userId, string missionId)
    //    {
    //        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(missionId))
    //        {
    //            return ServiceResult<IEnumerable<IndexMissionModel>>.Error("Thông tin không hợp lệ.");
    //        }

    //        try
    //        {
    //            ServiceResult<Mission> mission = await _unitOfWork.Repository<Mission>().GetOneAsync("Id", missionId);
    //            ServiceResult<Plan> plan = await _unitOfWork.Repository<Plan>().GetOneAsync("Id", mission.Data.PlanId);
    //            ServiceResult<Project> project = await _unitOfWork.Repository<Project>().GetOneAsync("Id", plan.Data.ProjectId);
    //            ProjectMember? member = (await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", project.Data.Id)).Data
    //                .FirstOrDefault(m => m.UserId == userId);

    //            if (member == null || mission.Data.ProjectMemberId != member.Id)
    //            {
    //                return ServiceResult<IEnumerable<IndexMissionModel>>.Error("Không có quyền xóa nhiệm vụ này.");
    //            }

    //            ServiceResult<IEnumerable<IndexMemberMissionModel>> deleteSubResult = await _missionMemberHandle.DeleteManyAsync(missionId);
    //            if (deleteSubResult.Status != ResultStatus.Success)
    //            {
    //                return ServiceResult<IEnumerable<IndexMissionModel>>.Error("Xóa thành viên nhiệm vụ thất bại.");
    //            }

    //            ServiceResult<bool> deleteResult = await _unitOfWork.ExecuteTransactionAsync(() =>
    //                _unitOfWork.Repository<Mission>().DeleteAsync(mission.Data));

    //            return deleteResult.Status != ResultStatus.Success
    //                ? ServiceResult<IEnumerable<IndexMissionModel>>.Error("Xóa nhiệm vụ thất bại.")
    //                : await GetAsync(mission.Data.PlanId);
    //        }
    //        catch (Exception ex)
    //        {
    //            return ServiceResult<IEnumerable<IndexMissionModel>>.FromException(ex);
    //        }
    //    }

    //    /// <summary>
    //    /// Xóa tất cả các nhiệm vụ thuộc một kế hoạch.
    //    /// </summary>
    //    public async Task<ServiceResult<IEnumerable<IndexMissionModel>>> DeleteManyAsync( string planId)
    //    {
    //        if ( string.IsNullOrEmpty(planId))
    //        {
    //            return ServiceResult<IEnumerable<IndexMissionModel>>.Error("Thông tin không hợp lệ.");
    //        }

    //        try
    //        {

    //            ServiceResult<IEnumerable<Mission>> missions = await _unitOfWork.Repository<Mission>().GetManyAsync("PlanId", planId);
    //            foreach (Mission m in missions.Data)
    //            {
    //                ServiceResult<IEnumerable<IndexMemberMissionModel>> subDelete = await _missionMemberHandle.DeleteManyAsync(m.Id);
    //                if (subDelete.Status != ResultStatus.Success)
    //                {
    //                    return ServiceResult<IEnumerable<IndexMissionModel>>.Error("Xóa phụ nhiệm vụ thất bại.");
    //                }
    //            }

    //            ServiceResult<bool> delete = await _unitOfWork.ExecuteTransactionAsync(() =>
    //                _unitOfWork.Repository<Mission>().DeleteAsync(missions.Data.ToList()));

    //            return delete.Status != ResultStatus.Success
    //                ? ServiceResult<IEnumerable<IndexMissionModel>>.Error("Xóa nhiệm vụ thất bại.")
    //                : await GetAsync(planId);
    //        }
    //        catch (Exception ex)
    //        {
    //            return ServiceResult<IEnumerable<IndexMissionModel>>.FromException(ex);
    //        }
    //    }

    //}


}
