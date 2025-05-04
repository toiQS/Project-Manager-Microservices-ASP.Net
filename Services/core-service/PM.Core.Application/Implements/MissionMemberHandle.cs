using PM.Core.Application.Interfaces;
using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos.auths;
using PM.Shared.Dtos.cores.members;
using PM.Shared.Dtos.users;
using PM.Shared.Handle.Interfaces;

namespace PM.Core.Application.Implements
{
    public class MissionMemberHandle : IMissionMemberHandle
    {
        private readonly IUnitOfWork<CoreDbContext> _unitOfWork;
        private readonly IAPIService<UserDetail> _userAPI;
        private readonly IPositionHandle _positionHandle;

        public MissionMemberHandle(
            IUnitOfWork<CoreDbContext> unitOfWork,
            IAPIService<UserDetail> userAPI,
            IPositionHandle positionHandle)
        {
            _unitOfWork = unitOfWork;
            _userAPI = userAPI;
            _positionHandle = positionHandle;
        }

        /// <summary>
        /// Lấy danh sách các thành viên đang tham gia một nhiệm vụ cụ thể.
        /// </summary>
        /// <param name="missionId">ID của nhiệm vụ</param>
        /// <returns>Danh sách thành viên kiểu IndexMemberModel</returns>
        public async Task<ServiceResult<IEnumerable<IndexMemberModel>>> GetAsync(string missionId)
        {
            if (string.IsNullOrWhiteSpace(missionId))
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Mission ID is required.");
            }

            try
            {
                ServiceResult<IEnumerable<MissionMember>> missionMembers = await _unitOfWork.Repository<MissionMember>().GetManyAsync("MissionId", missionId);
                if (!missionMembers.IsSuccess())
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Failed to retrieve mission members.");
                }

                List<IndexMemberModel> results = [];
                foreach (MissionMember missionMember in missionMembers.Data)
                {
                    ServiceResult<ProjectMember> projectMemberResult = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", missionMember.MemberId);
                    if (!projectMemberResult.IsSuccess())
                    {
                        return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Project member not found.");
                    }

                    ServiceResult<UserDetail> userResult = await _userAPI.APIsGetAsync($"/api/get-user?userId={projectMemberResult.Data.UserId}");
                    if (!userResult.IsSuccess())
                    {
                        return ServiceResult<IEnumerable<IndexMemberModel>>.Error("User API call failed.");
                    }

                    ServiceResult<Position> positionResult = await _unitOfWork.Repository<Position>().GetOneAsync("Id", projectMemberResult.Data.PositionId);

                    results.Add(new IndexMemberModel
                    {
                        Id = missionMember.Id,
                        ProjectMemberId = missionMember.MemberId,
                        Username = userResult.Data.UserName,
                        PositionName = positionResult.Data?.Name ?? "Unknown"
                    });
                }

                return ServiceResult<IEnumerable<IndexMemberModel>>.Success(results);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.FromException(ex);
            }
        }

        /// <summary>
        /// Thêm một thành viên vào nhiệm vụ, với điều kiện người dùng hiện tại là chủ nhiệm vụ.
        /// </summary>
        /// <param name="userId">ID của người thực hiện yêu cầu</param>
        /// <param name="model">Model chứa thông tin thành viên cần thêm</param>
        /// <returns>Danh sách thành viên mới sau khi thêm</returns>
        public async Task<ServiceResult<IEnumerable<IndexMemberModel>>> AddAsync(string userId, AddMemberModel model)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(model.MissionId) || string.IsNullOrWhiteSpace(model.MemberId))
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Missing parameters.");
            }

            try
            {
                ServiceResult<ProjectMember> requestor = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("UserId", userId);
                ServiceResult<ProjectMember> targetMember = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", model.MemberId);
                ServiceResult<Mission> mission = await _unitOfWork.Repository<Mission>().GetOneAsync("Id", model.MissionId);

                if (!requestor.IsSuccess() || !targetMember.IsSuccess() || !mission.IsSuccess())
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Data fetch failed.");
                }

                if (mission.Data.ProjectMemberId != requestor.Data.Id)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Permission denied.");
                }

                ServiceResult<IEnumerable<IndexMemberModel>> existingMembers = await GetAsync(model.MissionId);
                if (existingMembers.Data.Any(x => x.ProjectMemberId == model.MemberId))
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Member already exists.");
                }

                MissionMember newMember = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    MemberId = model.MemberId,
                    MissionId = model.MissionId
                };

                ServiceResult<bool> addResult = await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<MissionMember>().AddAsync(newMember));

                return !addResult.IsSuccess() ? ServiceResult<IEnumerable<IndexMemberModel>>.Error("Add failed.") : await GetAsync(model.MissionId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.FromException(ex);
            }
        }

        // <summary>
        /// Xoá một thành viên ra khỏi nhiệm vụ, với điều kiện người dùng hiện tại là chủ nhiệm vụ.
        /// </summary>
        /// <param name="userId">ID người thực hiện thao tác</param>
        /// <param name="model">Model chứa thông tin thành viên cần xoá</param>
        /// <returns>Danh sách thành viên còn lại sau khi xoá</returns>
        public async Task<ServiceResult<IEnumerable<IndexMemberModel>>> DeleteAsync(string userId, DeleteMemberModel model)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(model.MemberId) || string.IsNullOrWhiteSpace(model.MissionId))
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Missing parameters.");
            }

            try
            {
                ServiceResult<ProjectMember> requestor = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("UserId", userId);
                ServiceResult<ProjectMember> targetMember = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", model.MemberId);
                ServiceResult<Mission> mission = await _unitOfWork.Repository<Mission>().GetOneAsync("Id", model.MissionId);

                if (!requestor.IsSuccess() || !targetMember.IsSuccess() || !mission.IsSuccess())
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Data fetch failed.");
                }

                if (mission.Data.ProjectMemberId != requestor.Data.Id)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Permission denied.");
                }

                MissionMember? memberToRemove = (await _unitOfWork.Repository<MissionMember>().GetManyAsync("MissionId", model.MissionId))
                    .Data?.FirstOrDefault(x => x.MemberId == model.MemberId);

                if (memberToRemove == null)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Member not found in mission.");
                }

                ServiceResult<bool> deleteResult = await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<MissionMember>().DeleteAsync(memberToRemove));

                return !deleteResult.IsSuccess()
                    ? ServiceResult<IEnumerable<IndexMemberModel>>.Error("Delete failed.")
                    : await GetAsync(model.MissionId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.FromException(ex);
            }
        }

        // <summary>
        /// Xoá toàn bộ thành viên ra khỏi một nhiệm vụ cụ thể.
        /// Dùng khi xoá nhiệm vụ hoặc làm mới danh sách.
        /// </summary>
        /// <param name="missionId">ID của nhiệm vụ</param>
        /// <returns>Danh sách thành viên trống (sau khi xoá)</returns>
        public async Task<ServiceResult<IEnumerable<IndexMemberModel>>> DeleteManyAsync(string missionId)
        {
            if (string.IsNullOrWhiteSpace(missionId))
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Mission ID is required.");
            }

            try
            {
                ServiceResult<IEnumerable<MissionMember>> members = await _unitOfWork.Repository<MissionMember>().GetManyAsync("MissionId", missionId);
                if (!members.IsSuccess())
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("Fetch failed.");
                }

                ServiceResult<bool> deleteResult = await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<MissionMember>().DeleteAsync(members.Data.ToList()));

                return !deleteResult.IsSuccess() ? ServiceResult<IEnumerable<IndexMemberModel>>.Error("Delete failed.") : await GetAsync(missionId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.FromException(ex);
            }
        }
    }

    internal static class ServiceResultExtensions
    {
        public static bool IsSuccess<T>(this ServiceResult<T> result)
        {
            return result.Status == ResultStatus.Success && result.Data != null;
        }
    }
}
