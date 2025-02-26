using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.members;
using PM.Domain.Models.missions;
using Shared.member;

namespace PM.Persistence.Implements.Services
{
    public class MemberServices : IMemberServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private string _ownRoleId;
        public MemberServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #region Retrieve members
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
                        UserName = user.Data.UserName,
                        MemberId = member.Id
                    });
                }
                return ServicesResult<IEnumerable<IndexMember>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexMember>>.Failure(ex.Message);
            }
        }
        #endregion

        #region Retrieve member details
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
                        UserName = user.Data.UserName,
                        MemberId = member.Id
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
        #endregion


        #region retrieve member details
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
                    MemberName = user.Data.UserName,
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
        #endregion

        #region add a new member to a project  
        /// <summary>
        /// Adds a new member to a project if the requesting member has sufficient permissions.
        /// </summary>
        /// <param name="memberCurrentId">The ID of the member performing the action.</param>
        /// <param name="projectId">The ID of the project to which the new member is being added.</param>
        /// <param name="addMember">The details of the member to be added.</param>
        /// <returns>A service result containing the added member's details or an error message.</returns>
        public async Task<ServicesResult<DetailMember>> AddMember(string memberCurrentId, string projectId, AddMember addMember)
        {
            if (string.IsNullOrEmpty(memberCurrentId) || string.IsNullOrEmpty(projectId) || addMember == null)
                return ServicesResult<DetailMember>.Failure("Invalid input parameters.");

            try
            {
                // Validate ownership role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to validate ownership role: {ownRoleResult.Message}");

                // Retrieve the requesting member
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberCurrentId);
                if (!memberResult.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to retrieve member: {memberResult.Message}");

                var member = memberResult.Data;

                // Ensure the requesting member has permission
                if (member.RoleId != _ownRoleId || member.ProjectId != projectId)
                    return ServicesResult<DetailMember>.Failure("User does not have sufficient permissions to add members to this project.");

                // Retrieve existing project members
                var membersResult = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!membersResult.Status || membersResult.Data == null)
                    return ServicesResult<DetailMember>.Failure($"Failed to retrieve project members: {membersResult.Message}");

                // Check if the user is already a member of the project
                if (membersResult.Data.Any(x => x.UserId == addMember.UserId))
                    return ServicesResult<DetailMember>.Failure("This user is already a member of the project.");

                // Create new project member
                var newMember = new ProjectMember
                {
                    Id = Guid.NewGuid().ToString(),
                    PositionWork = addMember.PositionWork,
                    ProjectId = projectId,
                    RoleId = addMember.RoleId,
                    UserId = addMember.UserId
                };

                var addResult = await _unitOfWork.ProjectMemberRepository.AddAsync(newMember);
                if (!addResult.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to add new member: {addResult.Message}");

                // Retrieve user and project details for logging
                var addedUserResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", addMember.UserId);
                if (!addedUserResult.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to retrieve added user details: {addedUserResult.Message}");

                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!projectResult.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to retrieve project details: {projectResult.Message}");

                var requestingUserResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.UserId);
                if (!requestingUserResult.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to retrieve requesting user details: {requestingUserResult.Message}");

                // Log the action
                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = $"Added member {addedUserResult.Data.UserName} to project {projectResult.Data.Name} by {requestingUserResult.Data.UserName}.",
                    ActionDate = DateTime.Now,
                    ProjectId = projectId,
                    UserId = addedUserResult.Data.Id
                };

                var logResult = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!logResult.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to create activity log: {logResult.Message}");

                // Return detailed member information
                return await GetDetailMember(newMember.Id);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailMember>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        #endregion

        #region update a member in a project
        /// <summary>
        /// Updates a project member's details if the requesting user has the required permissions.
        /// </summary>
        /// <param name="memberCurrentId">The ID of the user performing the update.</param>
        /// <param name="memberId">The ID of the member being updated.</param>
        /// <param name="updateMember">The new details for the member.</param>
        /// <returns>A service result containing the updated member's details or an error message.</returns>
        public async Task<ServicesResult<DetailMember>> UpdateMember(string memberCurrentId, string memberId, UpdateMember updateMember)
        {
            if (string.IsNullOrEmpty(memberCurrentId) || string.IsNullOrEmpty(memberId) || updateMember == null)
                return ServicesResult<DetailMember>.Failure("Invalid input parameters.");

            try
            {
                // Validate ownership role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to validate ownership role: {ownRoleResult.Message}");

                // Retrieve the member to be updated
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!memberResult.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to retrieve member: {memberResult.Message}");

                var member = memberResult.Data;

                // Retrieve the requesting member
                var requesterResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberCurrentId);
                if (!requesterResult.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to retrieve requesting user: {requesterResult.Message}");

                var requester = requesterResult.Data;

                // Check if the requesting user has permission
                if (requester.RoleId != _ownRoleId || requester.ProjectId != member.ProjectId)
                    return ServicesResult<DetailMember>.Failure("User does not have sufficient permissions to update this member.");

                // Retrieve project details
                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", member.ProjectId);
                if (!projectResult.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to retrieve project details: {projectResult.Message}");

                var project = projectResult.Data;

                // Retrieve updated user details
                var updatedUserResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", updateMember.UserId);
                if (!updatedUserResult.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to retrieve updated user details: {updatedUserResult.Message}");

                var updatedUser = updatedUserResult.Data;

                // Update member details
                member.PositionWork = updateMember.PositionWork is null ? member.PositionWork : updateMember.PositionWork;
                member.ProjectId = updateMember.ProjectId is null ? member.ProjectId : updateMember.ProjectId;
                member.UserId = updateMember.UserId is null ? member.UserId : updateMember.UserId;

                var updateResponse = await _unitOfWork.ProjectMemberRepository.UpdateAsync(member);
                if (!updateResponse.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to update member: {updateResponse.Message}");

                // Log the action
                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = $"{requester.UserId} updated {updatedUser.UserName}'s details in project {project.Name}.",
                    ActionDate = DateTime.Now,
                    ProjectId = member.ProjectId,
                    UserId = updatedUser.Id
                };

                var logResponse = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!logResponse.Status)
                    return ServicesResult<DetailMember>.Failure($"Failed to create activity log: {logResponse.Message}");

                // Return updated member details
                return await GetDetailMember(memberId);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailMember>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        #endregion

        #region delete a member in a project
        /// <summary>
        /// Deletes a project member and any associated missions, logging the action.
        /// </summary>
        /// <param name="memberCurrentId">The ID of the user performing the deletion.</param>
        /// <param name="memberId">The ID of the member being deleted.</param>
        /// <returns>A service result containing the updated list of project members or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexMember>>> DeleteMember(string memberCurrentId, string memberId)
        {
            if (string.IsNullOrEmpty(memberCurrentId) || string.IsNullOrEmpty(memberId))
                return ServicesResult<IEnumerable<IndexMember>>.Failure("Invalid input parameters.");

            try
            {
                // Validate ownership role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure($"Failed to validate ownership role: {ownRoleResult.Message}");

                // Retrieve the project member to be deleted
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!memberResult.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure("Member not found.");

                var member = memberResult.Data;

                // Retrieve the requesting user
                var requesterResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberCurrentId);
                if (!requesterResult.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure($"Failed to retrieve requesting user: {requesterResult.Message}");

                var requester = requesterResult.Data;

                // Verify if the requesting user has the required role and belongs to the same project
                if (requester.RoleId != _ownRoleId || requester.ProjectId != member.ProjectId)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure("User does not have sufficient permissions to remove this member.");

                // Retrieve assigned missions for the member
                var missionsResult = await _unitOfWork.MissionAssignmentRepository.GetManyByKeyAndValue("ProjectMemberId", memberId);
                if (!missionsResult.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure($"Failed to retrieve missions: {missionsResult.Message}");

                // Delete all assigned missions
                foreach (var mission in missionsResult.Data)
                {
                    var deleteMissionResult = await _unitOfWork.MissionAssignmentRepository.DeleteAsync(mission.MissionId);
                    if (!deleteMissionResult.Status)
                        return ServicesResult<IEnumerable<IndexMember>>.Failure($"Failed to delete mission {mission.MissionId}: {deleteMissionResult.Message}");
                }

                // Delete the member from the project
                var deleteMemberResult = await _unitOfWork.ProjectMemberRepository.DeleteAsync(memberId);
                if (!deleteMemberResult.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure($"Failed to delete project member: {deleteMemberResult.Message}");

                // Retrieve user and project details for logging
                var ownerResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", requester.UserId);
                if (!ownerResult.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure($"Failed to retrieve owner info: {ownerResult.Message}");

                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", member.ProjectId);
                if (!projectResult.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure($"Failed to retrieve project info: {projectResult.Message}");

                var owner = ownerResult.Data;
                var project = projectResult.Data;

                // Log the deletion action
                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = $"{owner.UserName} removed a member and deleted related missions in project {project.Name}.",
                    ActionDate = DateTime.Now,
                    ProjectId = project.Id,
                    UserId = requester.UserId
                };

                var logResult = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!logResult.Status)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure($"Failed to create activity log: {logResult.Message}");

                // Return the updated list of project members
                return await GetMemberInProject(project.Id);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexMember>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        #endregion

        #region Helper methods
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
        }
        #endregion
    }
}
