using PM.Domain;
using PM.Domain.Interfaces;
using PM.Domain.Models.members;
using PM.Domain.Models.projects;
using Shared.member;
using System.Data.Entity.Infrastructure.DependencyResolution;

namespace PM.Persistence.Implements.Services
{
    internal class MemberServices
    {
        private readonly IUnitOfWork _unitOfWork;
        public async Task<ServicesResult<IEnumerable<IndexMember>>> GetMembers()
        {
            var response = new List<IndexMember>();
            try
            {
                var members = await _unitOfWork.ProjectMemberRepository.GetAllAsync();
                if (members.Status == false) return ServicesResult<IEnumerable<IndexMember>>.Failure(members.Message);
                foreach (var member in members.Data)
                {
                    var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.UserId);
                    if (user.Status == false) return ServicesResult<IEnumerable<IndexMember>>.Failure(user.Message);
                    var index = new IndexMember()
                    {
                        UserAvata = user.Data.AvatarPath,
                        PositionWorkName = member.PositionWork,
                        UserName = user.Data.NickName,
                        RoleUserInProjectId = member.Id
                    };
                    response.Add(index);
                }
                return ServicesResult<IEnumerable<IndexMember>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexMember>>.Failure("");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ServicesResult<IEnumerable<IndexMember>>> GetMemberInProject(string projectId)
        {
            var response = new List<IndexMember>();
            if (string.IsNullOrEmpty(projectId)) return ServicesResult<IEnumerable<IndexMember>>.Failure("");
            try
            {
                var members = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId",projectId);
                if (members.Status == false) return ServicesResult<IEnumerable<IndexMember>>.Failure(members.Message);
                foreach (var member in members.Data)
                {
                    var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.UserId);
                    if (user.Status == false) return ServicesResult<IEnumerable<IndexMember>>.Failure(user.Message);
                    var index = new IndexMember()
                    {
                        UserAvata = user.Data.AvatarPath,
                        PositionWorkName = member.PositionWork,
                        UserName = user.Data.NickName,
                        RoleUserInProjectId = member.Id
                    };
                    response.Add(index);
                }
                return ServicesResult<IEnumerable<IndexMember>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexMember>>.Failure("");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public Task<ServicesResult<DetailMember>> GetDetailMember(string memberId)
        {
            if (memberId == null) return ServicesResult<DetailMember>.Failure("");
            try
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public Task<ServicesResult<DetailMember>> AddMember(string userId, string projectId, AddMember addMember);
        public Task<ServicesResult<DetailMember>> UpdateMember(string userId, string memberId, UpdateMember updateMember);
        public Task<ServicesResult<IEnumerable<IndexMember>>> DeleteMember(string userId, string memberId);
    }
}
