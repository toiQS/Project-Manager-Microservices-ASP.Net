using PM.Core.Application.Interfaces;
using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos;
using PM.Shared.Dtos.cores.members.missions;
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

        
        public async Task<ServiceResult<IEnumerable<IndexMemberMissionModel>>> GetAsync(string missionId)
        {
            if (string.IsNullOrWhiteSpace(missionId))
            {
                return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Mission ID is required.");
            }

            try
            {
                ServiceResult<IEnumerable<MissionMember>> missionMembers = await _unitOfWork.Repository<MissionMember>().GetManyAsync("MissionId", missionId);
                if (!missionMembers.IsSuccess())
                {
                    return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Failed to retrieve mission members.");
                }

                List<IndexMemberMissionModel> results = [];
                foreach (MissionMember missionMember in missionMembers.Data)
                {
                    ServiceResult<ProjectMember> projectMemberResult = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", missionMember.MemberId);
                    if (!projectMemberResult.IsSuccess())
                    {
                        return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Project member not found.");
                    }

                    ServiceResult<UserDetail> userResult = await _userAPI.APIsGetAsync($"/api/user/get-user?userId={projectMemberResult.Data.UserId}");
                    if (!userResult.IsSuccess())
                    {
                        return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("User API call failed.");
                    }

                    ServiceResult<Position> positionResult = await _unitOfWork.Repository<Position>().GetOneAsync("Id", projectMemberResult.Data.PositionId);

                    results.Add(new IndexMemberMissionModel
                    {
                        Id = missionMember.Id,
                        ProjectMemberId = missionMember.MemberId,
                        Username = userResult.Data.UserName,
                        PositionName = positionResult.Data?.Name ?? "Unknown"
                    });
                }

                return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Success(results);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexMemberMissionModel>>.FromException(ex);
            }
        }

        
        public async Task<ServiceResult<IEnumerable<IndexMemberMissionModel>>> AddAsync(string userId, AddMemberMissionModel model)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(model.MissionId) || string.IsNullOrWhiteSpace(model.MemberId))
            {
                return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Missing parameters.");
            }

            try
            {
                ServiceResult<ProjectMember> requestor = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("UserId", userId);
                ServiceResult<ProjectMember> targetMember = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", model.MemberId);
                ServiceResult<Mission> mission = await _unitOfWork.Repository<Mission>().GetOneAsync("Id", model.MissionId);

                if (!requestor.IsSuccess() || !targetMember.IsSuccess() || !mission.IsSuccess())
                {
                    return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Data fetch failed.");
                }

                if (mission.Data.ProjectMemberId != requestor.Data.Id)
                {
                    return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Permission denied.");
                }

                ServiceResult<IEnumerable<IndexMemberMissionModel>> existingMembers = await GetAsync(model.MissionId);
                if (existingMembers.Data.Any(x => x.ProjectMemberId == model.MemberId))
                {
                    return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Member already exists.");
                }

                MissionMember newMember = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    MemberId = model.MemberId,
                    MissionId = model.MissionId
                };

                ServiceResult<bool> addResult = await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<MissionMember>().AddAsync(newMember));

                return !addResult.IsSuccess() ? ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Add failed.") : await GetAsync(model.MissionId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexMemberMissionModel>>.FromException(ex);
            }
        }

        
        public async Task<ServiceResult<IEnumerable<IndexMemberMissionModel>>> DeleteAsync(string userId, DeleteMemberMissionModel model)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(model.MemberId) || string.IsNullOrWhiteSpace(model.MissionId))
            {
                return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Missing parameters.");
            }

            try
            {
                ServiceResult<ProjectMember> requestor = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("UserId", userId);
                ServiceResult<ProjectMember> targetMember = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", model.MemberId);
                ServiceResult<Mission> mission = await _unitOfWork.Repository<Mission>().GetOneAsync("Id", model.MissionId);

                if (!requestor.IsSuccess() || !targetMember.IsSuccess() || !mission.IsSuccess())
                {
                    return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Data fetch failed.");
                }

                if (mission.Data.ProjectMemberId != requestor.Data.Id)
                {
                    return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Permission denied.");
                }

                MissionMember? memberToRemove = (await _unitOfWork.Repository<MissionMember>().GetManyAsync("MissionId", model.MissionId))
                    .Data?.FirstOrDefault(x => x.MemberId == model.MemberId);

                if (memberToRemove == null)
                {
                    return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Member not found in mission.");
                }

                ServiceResult<bool> deleteResult = await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<MissionMember>().DeleteAsync(memberToRemove));

                return !deleteResult.IsSuccess()
                    ? ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Delete failed.")
                    : await GetAsync(model.MissionId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexMemberMissionModel>>.FromException(ex);
            }
        }

        
        public async Task<ServiceResult<IEnumerable<IndexMemberMissionModel>>> DeleteManyAsync(string missionId)
        {
            if (string.IsNullOrWhiteSpace(missionId))
            {
                return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Mission ID is required.");
            }

            try
            {
                ServiceResult<IEnumerable<MissionMember>> members = await _unitOfWork.Repository<MissionMember>().GetManyAsync("MissionId", missionId);
                if (!members.IsSuccess())
                {
                    return ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Fetch failed.");
                }

                ServiceResult<bool> deleteResult = await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<MissionMember>().DeleteAsync(members.Data.ToList()));

                return !deleteResult.IsSuccess() ? ServiceResult<IEnumerable<IndexMemberMissionModel>>.Error("Delete failed.") : await GetAsync(missionId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexMemberMissionModel>>.FromException(ex);
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
