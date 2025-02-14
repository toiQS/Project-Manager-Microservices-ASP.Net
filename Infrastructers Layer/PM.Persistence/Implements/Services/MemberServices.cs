using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.members;
using PM.Domain.Models.missions;
using Shared.member;

namespace PM.Persistence.Implements.Services
{
    internal class MemberServices : IMemberServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private string _ownRoleId;
        public MemberServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
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

                var missions = await _unitOfWork.MissionAssignmentRepository.GetManyByKeyAndValue("ProjectMemberId", memberId);
                if (!missions.Status) return ServicesResult<DetailMember>.Failure(missions.Message);

                foreach (var item in missions.Data)
                {
                    var task = await _unitOfWork.MissionRepository.GetOneByKeyAndValue("Id", item.MissionId);
                    if (!task.Status) return ServicesResult<DetailMember>.Failure(task.Message);

                    var status = await _unitOfWork.StatusRepository.GetOneByKeyAndValue("Id", task.Data.StatusId);
                    if (!status.Status) return ServicesResult<DetailMember>.Failure(status.Message);

                    response.missions.Add(new IndexMission
                    {
                        MissionId = task.Data.Id,
                        MissionName = task.Data.Name,
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
                var own = await GetOwnRole();
                if(own .Status == false) return ServicesResult<DetailMember>.Failure(own.Message);
                // Check if the user exists
                var userExists = await _unitOfWork.UserRepository.ExistAsync("Id", userId);
                if (!userExists) return ServicesResult<DetailMember>.Failure("User not found");

                // Retrieve project members
                var members = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!members.Status || members.Data == null) return ServicesResult<DetailMember>.Failure("Error: No members found in this project");

                var isCheckRole = members.Data.Where(x => x.UserId == userId && x.ProjectId == projectId && x.RoleId == _ownRoleId ).Any();
                if (!isCheckRole) return ServicesResult<DetailMember>.Failure("User has not enough role in this project");
                // Check if the user is already a member
                if (members.Data.Any(x => x.UserId == addMember.UserId))
                    return ServicesResult<DetailMember>.Failure("Member already exists in this project");

                // Create new project member
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

                // Log the action
                var infoMember = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", addMember.UserId);
                if (!infoMember.Status) return ServicesResult<DetailMember>.Failure(infoMember.Message);
                var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!project.Status) return ServicesResult<DetailMember>.Failure(project.Message);
                var infoUser = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!infoUser.Status) return ServicesResult<DetailMember>.Failure(infoUser.Message);

                var log = new ActivityLog()
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = $"Added member {infoMember.Data.NickName} to project {project.Data.Name} by {infoUser.Data.NickName}",
                    ActionDate = DateTime.Now,
                    ProjectId = projectId,
                    UserId = userId,
                };

                var responseAddLog = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!responseAddLog.Status) return ServicesResult<DetailMember>.Failure(responseAddLog.Message);

                // Return detailed member information
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

        /// <summary>
        /// Updates an existing project member.
        /// </summary>
        public async Task<ServicesResult<DetailMember>> UpdateMember(string userId, string memberId, UpdateMember updateMember)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(memberId) || updateMember == null)
                return ServicesResult<DetailMember>.Failure("Invalid input parameters");

            try
            {

                var own = await GetOwnRole();
                if (own.Status == false) return ServicesResult<DetailMember>.Failure(own.Message);
                // Retrieve member
                var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!member.Status) return ServicesResult<DetailMember>.Failure(member.Message);

                // Verify if the user is the project owner
                var memberProject = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", member.Data.ProjectId);
                if (!memberProject.Status || !memberProject.Data.Any()) return ServicesResult<DetailMember>.Failure("Error retrieving project members");

                var isOwner = memberProject.Data.Any(x => x.ProjectId == member.Data.ProjectId && x.UserId == userId && x.RoleId == _ownRoleId);
                if (!isOwner) return ServicesResult<DetailMember>.Failure("User is not the owner of the project");

                // Update member details
                member.Data.PositionWork = updateMember.PositionWork;
                member.Data.ProjectId = updateMember.ProjectId;
                member.Data.UserId = updateMember.UserId;

                var responseUpdate = await _unitOfWork.ProjectMemberRepository.UpdateAsync(member.Data);
                if (!responseUpdate.Status) return ServicesResult<DetailMember>.Failure(responseUpdate.Message);

                // Log the action
                var infoMember = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", updateMember.UserId);
                if (!infoMember.Status) return ServicesResult<DetailMember>.Failure(infoMember.Message);
                var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", member.Data.ProjectId);
                if (!project.Status) return ServicesResult<DetailMember>.Failure(project.Message);
                var infoUser = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!infoUser.Status) return ServicesResult<DetailMember>.Failure(infoUser.Message);

                var log = new ActivityLog()
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = $"{infoUser.Data.NickName} updated a member in project {project.Data.Name}",
                    ActionDate = DateTime.Now,
                    ProjectId = member.Data.ProjectId,
                    UserId = userId
                };

                var responseAddLog = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!responseAddLog.Status) return ServicesResult<DetailMember>.Failure(responseAddLog.Message);

                return await GetDetailMember(memberId);
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
        /// <summary>
        /// Deletes a member from a project and removes any associated mission assignments.
        /// </summary>
        public async Task<ServicesResult<IEnumerable<IndexMember>>> DeleteMember(string userId, string memberId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(memberId))
                return ServicesResult<IEnumerable<IndexMember>>.Failure("Invalid input parameters");

            try
            {
                // Retrieve the project member to be deleted
                var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!member.Status) return ServicesResult<IEnumerable<IndexMember>>.Failure("Member not found");

                // Retrieve project members and check if the user is the owner
                var memberProject = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", member.Data.ProjectId);
                if (!memberProject.Status || !memberProject.Data.Any())
                    return ServicesResult<IEnumerable<IndexMember>>.Failure("Failed to retrieve project members");

                bool isOwner = memberProject.Data.Any(x => x.ProjectId == member.Data.ProjectId && x.UserId == userId && x.RoleId == _ownRoleId);
                if (!isOwner)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure("User is not the project owner");

                // Retrieve mission assignments associated with the member
                var missions = await _unitOfWork.MissionAssignmentRepository.GetManyByKeyAndValue("ProjectMemberId", memberId);
                if (!missions.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure(missions.Message);

                // If the member has assigned missions, delete them first
                if (missions.Data.Any())
                {
                    foreach (var mission in missions.Data)
                    {
                        var deleteMissionResult = await _unitOfWork.MissionAssignmentRepository.DeleteAsync(mission.MissionId);
                        if (!deleteMissionResult.Status)
                            return ServicesResult<IEnumerable<IndexMember>>.Failure(deleteMissionResult.Message);
                    }
                }

                // Now, delete the member from the project
                var deleteMemberResult = await _unitOfWork.ProjectMemberRepository.DeleteAsync(memberId);
                if (!deleteMemberResult.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure(deleteMemberResult.Message);

                // Retrieve user and project information for logging
                var ownerInfo = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!ownerInfo.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure(ownerInfo.Message);

                var projectInfo = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", member.Data.ProjectId);
                if (!projectInfo.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure(projectInfo.Message);

                // Log the deletion action
                var log = new ActivityLog()
                {
                    Id = "",
                    Action = $"{ownerInfo.Data.NickName} removed a member and deleted related missions in project {projectInfo.Data.Name}",
                    ActionDate = DateTime.Now,
                    ProjectId = member.Data.ProjectId,
                    UserId = userId
                };

                var logResult = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!logResult.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure(logResult.Message);

                // Return updated project members list
                return await GetMemberInProject(projectInfo.Data.Id);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexMember>>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }
        /// <summary>
        /// Gets the role ID for the owner role.
        /// </summary>
        /// <returns>Service result indicating success or failure</returns>
        private async Task<ServicesResult<bool>> GetOwnRole()
        {
            try
            {
                var ownRole = await _unitOfWork.RoleInProjectRepository.GetOneByKeyAndValue("Name", "Owner");
                if (!ownRole.Status)
                    return ServicesResult<bool>.Failure(ownRole.Message);

                _ownRoleId = ownRole.Data.Id;
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure(ex.Message);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

    }
}
