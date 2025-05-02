using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Abstractions;
using PM.Core.Application.Interfaces;
using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos.auths;
using PM.Shared.Dtos.cores.members;
using PM.Shared.Dtos.users;
using PM.Shared.Handle.Interfaces;
using System.Security.Principal;

namespace PM.Core.Application.Implements
{
    public class MissionMemberHandle
    {
        private readonly IUnitOfWork<CoreDbContext> _unitOfWork;
        private readonly IAPIService<UserDetail> _userAPI;
        private readonly IPositionHandle _positionHandle;
        public MissionMemberHandle(IUnitOfWork<CoreDbContext> unitOfWork, IAPIService<UserDetail> userAPI, IPositionHandle positionHandle)
        {
            _unitOfWork = unitOfWork;
            _userAPI = userAPI;
            _positionHandle = positionHandle;
        }

        // toàn bộ thành viên trong nhiệm vụ
        public async Task<ServiceResult<IEnumerable<IndexMemberModel>>> GetAsync(string missionId)
        {
            if (string.IsNullOrEmpty(missionId))
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
            }

            List<IndexMemberModel> result = new List<IndexMemberModel>();
            try
            {
                ServiceResult<IEnumerable<MissionMember>> memberMissions = await _unitOfWork.Repository<MissionMember>().GetManyAsync("MissionId", missionId);
                if (memberMissions.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                }

                foreach (MissionMember item in memberMissions.Data)
                {
                    ServiceResult<ProjectMember> member = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", item.MemberId);
                    if (member.Status != ResultStatus.Success)
                    {
                        return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                    }

                    ServiceResult<UserDetail> userResult = await _userAPI.APIsGetAsync($"/api/get-user?userId={member.Data.UserId}");
                    if (userResult.Status != ResultStatus.Success)
                    {
                        return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                    }

                    ServiceResult<Position> position = await _unitOfWork.Repository<Position>().GetOneAsync("Id", member.Data.PositionId);
                    IndexMemberModel index = new IndexMemberModel()
                    {
                        Id = item.Id,
                        ProjectMemberId = item.MemberId,
                        PositionName = position.Data.Name,
                        Username = userResult.Data.UserName,
                    };
                    result.Add(index);
                }
                return ServiceResult<IEnumerable<IndexMemberModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.FromException(ex);
            }
        }
        // thêm thành viên
        public async Task<ServiceResult<IEnumerable<IndexMemberModel>>> AddAsync(string userId, AddMemberModel model)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
            }

            if (string.IsNullOrEmpty(model.MissionId) || string.IsNullOrEmpty(model.MemberId))
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
            }

            try
            {
                ServiceResult<ProjectMember> member = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("UserId", userId);
                if (member.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                }
                ServiceResult<ProjectMember> memberIsAdded = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", model.MemberId);
                if (memberIsAdded.Status != ResultStatus.Success || memberIsAdded.Data == null)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                }

                ServiceResult<Mission> mission = await _unitOfWork.Repository<Mission>().GetOneAsync("Id", model.MissionId);
                if (mission.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                }

                if (mission.Data.ProjectMemberId != member.Data.Id)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                }

                bool icheck = (await GetAsync(model.MissionId)).Data.Any(x => x.ProjectMemberId == model.MemberId);
                if (icheck)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                }

                MissionMember missionMember = new MissionMember()
                {
                    Id = Guid.NewGuid().ToString(),
                    MemberId = model.MemberId,
                    MissionId = model.MissionId,
                };
                ServiceResult<bool> addMissionResult = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.Repository<MissionMember>().AddAsync(missionMember));
                if (addMissionResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                }

                return await GetAsync(model.MissionId);

            }
            catch (Exception ex)

            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.FromException(ex);
            }
        }
        // xóa thành viên
        public async Task< ServiceResult<IEnumerable<IndexMemberModel>>> Delete(string userId, DeleteMemberModel model)
        {
            if (userId == null || model.MemberId == null || model.MissionId == null)
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
            }

            try
            {
                ServiceResult<ProjectMember> member = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("UserId", userId);
                if (member.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                }
                ServiceResult<ProjectMember> memberIsAdded = await _unitOfWork.Repository<ProjectMember>().GetOneAsync("Id", model.MemberId);
                if (memberIsAdded.Status != ResultStatus.Success || memberIsAdded.Data == null)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                }

                ServiceResult<Mission> mission = await _unitOfWork.Repository<Mission>().GetOneAsync("Id", model.MissionId);
                if (mission.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                }

                if (mission.Data.ProjectMemberId != member.Data.Id)
                {
                    return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                }
                var memberMisson = (await _unitOfWork.Repository<MissionMember>().GetManyAsync("MissionId", model.MissionId)).Data.Where(x => x.MemberId == model.MemberId).FirstOrDefault();
                if (memberMisson == null) return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                var deleteMemberResult = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.Repository<MissionMember>().DeleteAsync(memberMisson));
                if (deleteMemberResult.Status != ResultStatus.Success) return ServiceResult<IEnumerable<IndexMemberModel>>.Error("");
                return await GetAsync(model.MissionId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexMemberModel>>.FromException(ex);
            }
        }
        // xóa toàn bộ thành viên
    }
}
