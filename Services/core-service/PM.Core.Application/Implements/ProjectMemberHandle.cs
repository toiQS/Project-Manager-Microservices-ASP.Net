using PM.Core.Application.Interfaces;
using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos;
using PM.Shared.Dtos.cores.members.projects;
using PM.Shared.Dtos.users;
using PM.Shared.Handle.Interfaces;

namespace PM.Core.Application.Implements
{
    public class ProjectMemberHandle :IProjectMemberHandle
    {
        private readonly IUnitOfWork<CoreDbContext> _unitOfWork;
        private readonly IAPIService<UserDetail> _userAPI;
        private readonly IPositionHandle _positionHandle;
        private readonly IPlanHandle _planHandle;
        private readonly Position _ownerPosition;

        public ProjectMemberHandle(
            IUnitOfWork<CoreDbContext> unitOfWork,
            IAPIService<UserDetail> userAPI,
            IPositionHandle positionHandle,
            IPlanHandle planHandle)
        {
            _unitOfWork = unitOfWork;
            _userAPI = userAPI;
            _positionHandle = positionHandle;
            _planHandle = planHandle;

            // Lưu thông tin vị trí Owner (Product Owner)
            _ownerPosition = _positionHandle.GetPositionByName("Product Owner").GetAwaiter().GetResult().Data;
        }

        // Lấy danh sách thành viên của dự án
        public async Task<ServiceResult<IEnumerable<IndexProjectMemberModel>>> GetMembersInProject(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
                return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Project ID is required.");

            List<IndexProjectMemberModel> result = new();
            try
            {
                var members = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", projectId);
                if (!members.IsSuccess() || members.Data == null)
                    return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Success(result);

                foreach (var item in members.Data)
                {
                    var userResult = await _userAPI.APIsGetAsync($"/api/user/get-user?userId={item.UserId}");
                    var position = await _unitOfWork.Repository<Position>().GetOneAsync("Id", item.PositionId);

                    if (!userResult.IsSuccess() || !position.IsSuccess() || position.Data == null)
                        return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Failed to fetch user or position info.");

                    result.Add(new IndexProjectMemberModel
                    {
                        Id = item.Id,
                        PositionName = position.Data.Name,
                        UserName = userResult.Data.UserName
                    });
                }

                return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexProjectMemberModel>>.FromException(ex);
            }
        }

        // Thêm thành viên vào dự án (chỉ Owner được thêm)
        public async Task<ServiceResult<IEnumerable<IndexProjectMemberModel>>> AddAsync(string userId, AddProjectMemberModel model)
        {
            if (string.IsNullOrEmpty(userId))
                return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("User ID is required.");

            try
            {
                var members = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", model.ProjectId);
                if (!members.IsSuccess())
                    return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Failed to retrieve members.");

                // Chỉ cho phép Owner thêm thành viên
                if (!IsOwner(userId, members.Data))
                    return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Permission denied.");

                // Không cho phép thêm trùng User
                if (members.Data.Any(x => x.UserId == model.UserId))
                    return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Member already exists.");

                return await AddAction(model);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexProjectMemberModel>>.FromException(ex);
            }
        }

        // Cập nhật thành viên (chỉ Owner được sửa)
        public async Task<ServiceResult<IEnumerable<IndexProjectMemberModel>>> PatchAsync(string userId, string memberId, PacthProjectMemberModel model)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.PositionId))
                return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Missing required data.");

            try
            {
                var member = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", memberId);
                if (!member.IsSuccess() || member.Data == null)
                    return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Member not found.");

                var members = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", member.Data.ProjectId);
                if (!members.IsSuccess() || !IsOwner(userId, members.Data))
                    return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Permission denied.");

                // Không cho phép cập nhật nếu user đã tồn tại trong danh sách
                if (members.Data.Any(x => x.UserId == model.UserId))
                    return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Duplicate user.");

                var patchData = new Dictionary<string, object>
                {
                    { nameof(model.UserId), model.UserId },
                    { nameof(model.PositionId), model.PositionId }
                };

                var patchResult = await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<ProjectMember>().PatchAsync(member.Data, patchData));

                return !patchResult.IsSuccess()
                    ? ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Failed to update member.")
                    : await GetMembersInProject(member.Data.ProjectId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexProjectMemberModel>>.FromException(ex);
            }
        }

        // Xoá thành viên (chỉ Owner)
        public async Task<ServiceResult<IEnumerable<IndexProjectMemberModel>>> DeleteAsync(string userId, string memberId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(memberId))
                return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Missing data.");

            try
            {
                var member = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", memberId);
                if (!member.IsSuccess() || member.Data == null)
                    return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Member not found.");

                var members = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", member.Data.ProjectId);
                if (!members.IsSuccess() || !IsOwner(userId, members.Data))
                    return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Permission denied.");

                var deleteResult = await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<ProjectMember>().DeleteAsync(member.Data));

                return !deleteResult.IsSuccess()
                    ? ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Failed to delete member.")
                    : await GetMembersInProject(member.Data.ProjectId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexProjectMemberModel>>.FromException(ex);
            }
        }

        // Xoá toàn bộ thành viên và kế hoạch khi xoá dự án
        public async Task<ServiceResult<IEnumerable<IndexProjectMemberModel>>> DeteleManyAsync(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("User ID or Project ID is missing.");

            try
            {
                var members = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", projectId);
                if (!members.IsSuccess())
                    return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Failed to retrieve project members.");

                var deletePlanResult = await _planHandle.DeleteManyAsync( projectId);
                if (!deletePlanResult.IsSuccess())
                    return ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Failed to delete plans.");

                var deleteMemberResult = await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<ProjectMember>().DeleteAsync(members.Data.ToList()));

                return !deleteMemberResult.IsSuccess()
                    ? ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Failed to delete members.")
                    : await GetMembersInProject(projectId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexProjectMemberModel>>.FromException(ex);
            }
        }

        // Thực hiện thêm mới thành viên
        private async Task<ServiceResult<IEnumerable<IndexProjectMemberModel>>> AddAction(AddProjectMemberModel model)
        {
            try
            {
                var member = new ProjectMember
                {
                    Id = Guid.NewGuid().ToString(),
                    PositionId = model.PositionId,
                    ProjectId = model.ProjectId,
                    UserId = model.UserId,
                };

                var result = await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<ProjectMember>().AddAsync(member));

                return !result.IsSuccess()
                    ? ServiceResult<IEnumerable<IndexProjectMemberModel>>.Error("Failed to add member.")
                    : await GetMembersInProject(model.ProjectId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexProjectMemberModel>>.FromException(ex);
            }
        }

        // Kiểm tra xem user có phải là Owner không
        private bool IsOwner(string userId, IEnumerable<ProjectMember> members)
        {
            return members.Any(x => x.UserId == userId && x.PositionId == _ownerPosition.Id);
        }
    }
}
