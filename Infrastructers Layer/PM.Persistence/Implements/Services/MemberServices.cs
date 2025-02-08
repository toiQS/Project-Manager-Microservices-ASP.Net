using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Models.members;
using PM.Domain.Models.tasks;
using Shared.member;

namespace PM.Persistence.Implements.Services
{
    internal class MemberServices
    {
        private readonly IUnitOfWork _unitOfWork;
        /// <summary>
        /// Retrieves all members.
        /// </summary>
        public async Task<ServicesResult<IEnumerable<IndexMember>>> GetMembers()
        {
            var response = new List<IndexMember>();
            try
            {
                var members = await _unitOfWork.ProjectMemberRepository.GetAllAsync();
                if (!members.Status) return ServicesResult<IEnumerable<IndexMember>>.Failure(members.Message);

                foreach (var member in members.Data)
                {
                    var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.UserId);
                    if (!user.Status) return ServicesResult<IEnumerable<IndexMember>>.Failure(user.Message);

                    response.Add(new IndexMember
                    {
                        UserAvata = user.Data.AvatarPath,
                        PositionWorkName = member.PositionWork,
                        UserName = user.Data.NickName,
                        RoleUserInProjectId = member.Id
                    });
                }
                return ServicesResult<IEnumerable<IndexMember>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexMember>>.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves members associated with a specific project.
        /// </summary>
        public async Task<ServicesResult<IEnumerable<IndexMember>>> GetMemberInProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId)) return ServicesResult<IEnumerable<IndexMember>>.Failure("Invalid project ID");

            var response = new List<IndexMember>();
            try
            {
                var members = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!members.Status) return ServicesResult<IEnumerable<IndexMember>>.Failure(members.Message);

                foreach (var member in members.Data)
                {
                    var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.UserId);
                    if (!user.Status) return ServicesResult<IEnumerable<IndexMember>>.Failure(user.Message);

                    response.Add(new IndexMember
                    {
                        UserAvata = user.Data.AvatarPath,
                        PositionWorkName = member.PositionWork,
                        UserName = user.Data.NickName,
                        RoleUserInProjectId = member.Id
                    });
                }
                return ServicesResult<IEnumerable<IndexMember>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexMember>>.Failure(ex.Message);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific member.
        /// </summary>
        public async Task<ServicesResult<DetailMember>> GetDetailMember(string memberId)
        {
            if (string.IsNullOrEmpty(memberId)) return ServicesResult<DetailMember>.Failure("Invalid member ID");

            try
            {
                var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!member.Status) return ServicesResult<DetailMember>.Failure(member.Message);

                var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.Data.UserId);
                if (!user.Status) return ServicesResult<DetailMember>.Failure(user.Message);

                var response = new DetailMember
                {
                    MemberId = memberId,
                    MemberName = user.Data.NickName,
                    PositionWork = member.Data.PositionWork
                };

                var role = await _unitOfWork.RoleInProjectRepository.GetOneByKeyAndValue("Id", member.Data.RoleId);
                if (!role.Status) return ServicesResult<DetailMember>.Failure(role.Message);
                response.RoleMember = role.Data.Name;

                var tasks = await _unitOfWork.MissionAssignmentRepository.GetManyByKeyAndValue("ProjectMemberId", memberId);
                if (!tasks.Status) return ServicesResult<DetailMember>.Failure(tasks.Message);

                foreach (var item in tasks.Data)
                {
                    var task = await _unitOfWork.MissionRepository.GetOneByKeyAndValue("Id", item.MissionId);
                    if (!task.Status) return ServicesResult<DetailMember>.Failure(task.Message);

                    var status = await _unitOfWork.StatusRepository.GetOneByKeyAndValue("Id", task.Data.StatusId);
                    if (!status.Status) return ServicesResult<DetailMember>.Failure(status.Message);

                    response.Tasks.Add(new IndexTask
                    {
                        TaskId = task.Data.Id,
                        TaskName = task.Data.Name,
                        Status = status.Data.Name
                    });
                }
                return ServicesResult<DetailMember>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailMember>.Failure(ex.Message);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        /// <summary>
        /// Adds a new member to a project.
        /// </summary>
        public async Task<ServicesResult<DetailMember>> AddMember(string userId, string projectId, AddMember addMember)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId) || addMember == null)
                return ServicesResult<DetailMember>.Failure("Invalid input parameters");

            try
            {
                var userExists = await _unitOfWork.UserRepository.ExistAsync("Id", userId);
                if (!userExists) return ServicesResult<DetailMember>.Failure("User not found");

                var members = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!members.Status || members.Data == null) return ServicesResult<DetailMember>.Failure("Error: No members found in this project");

                if (members.Data.Any(x => x.UserId == addMember.UserId))
                    return ServicesResult<DetailMember>.Failure("Member already exists in this project");

                var newMember = new ProjectMember
                {
                    Id = Guid.NewGuid().ToString(), // Generate a unique ID
                    PositionWork = addMember.PositionWork,
                    ProjectId = projectId,
                    RoleId = addMember.RoleId,
                    UserId = addMember.UserId,
                };

                var addResult = await _unitOfWork.ProjectMemberRepository.AddAsync(newMember);
                if (!addResult.Status) return ServicesResult<DetailMember>.Failure(addResult.Message);

                //tạo bảng ghi
                var infoMember = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", addMember.UserId);
                if(infoMember.Status == false) return ServicesResult<DetailMember>.Failure(infoMember.Message);
                var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id",projectId);
                if(project.Status == false ) return ServicesResult<DetailMember>.Failure(project.Message);
                var infoUser = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (infoUser.Status == false) return ServicesResult<DetailMember>.Failure(infoMember.Message);
                var log = new ActivityLog()
                {
                    Id ="",
                    Action = $"Add member {newMember.Id} {infoMember.Data.NickName} to project {project.Data.Name} by {userId} {infoUser.Data.NickName}",
                    ActionDate = DateTime.Now,
                    ProjectId=projectId,
                    UserId=userId,
                };
                // tạo kết quả trả về cho hoạt động
                return await GetDetailMember(newMember.Id);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailMember>.Failure(ex.Message);
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }
        public Task<ServicesResult<DetailMember>> UpdateMember(string userId, string memberId, UpdateMember updateMember)
        {

        }
        public Task<ServicesResult<IEnumerable<IndexMember>>> DeleteMember(string userId, string memberId);
    }
}
