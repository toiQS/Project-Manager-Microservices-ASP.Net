using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Models.members;
using PM.Domain.Models.missions;

namespace PM.Persistence.Implements.Services
{
    public class MissionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private string _ownerId;
        private string _leaderId;
        private string _mamagerId;
        public MissionServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Retrieve all missions from the repository and converts them to IndexMission objects with status information
        /// <summary>
        /// Retrieves all missions from the repository and converts them to IndexMission objects with status information.
        /// </summary>
        /// <returns>A ServicesResult containing a list of IndexMission objects or an error message if the operation fails.</returns>
        public async Task<ServicesResult<IEnumerable<IndexMission>>> GetIndexMissions()
        {
            try
            {
                // Retrieve all missions from the repository
                var missions = await _unitOfWork.MissionRepository.GetAllAsync();

                // Check if the retrieval operation was successful
                if (!missions.Status)
                {
                    return ServicesResult<IEnumerable<IndexMission>>.Failure(missions.Message);
                }

                var indexMissions = new List<IndexMission>();

                // Iterate through each mission to create IndexMission objects
                foreach (var item in missions.Data)
                {
                    var index = new IndexMission
                    {
                        MissionId = item.Id,
                        MissionName = item.Name
                    };

                    // Retrieve the status information for the current mission
                    var status = await _unitOfWork.StatusRepository.GetOneByKeyAndValue("Id", item.StatusId);

                    // Check if the status retrieval was successful
                    if (!status.Status)
                    {
                        return ServicesResult<IEnumerable<IndexMission>>.Failure(status.Message);
                    }

                    // Set the status name for the mission
                    index.Status = status.Data.Name;

                    // Add the completed IndexMission object to the result list
                    indexMissions.Add(index);
                }

                // Return the successful result with the list of IndexMission objects
                return ServicesResult<IEnumerable<IndexMission>>.Success(indexMissions);
            }
            catch (Exception e)
            {
                // Handle any exceptions by returning a failure result with the exception message
                return ServicesResult<IEnumerable<IndexMission>>.Failure(e.Message);
            }
            finally
            {
                // Ensure the UnitOfWork is disposed to release any resources
                _unitOfWork.Dispose();
            }
        }
        #endregion


        #region Retrieve all missions associated with the specified plan ID and converts them to IndexMission objects with status information
        /// <summary>
        /// Retrieves all missions associated with the specified plan ID and converts them to IndexMission objects with status information.
        /// </summary>
        /// <param name="planId">The ID of the plan whose missions are to be retrieved.</param>
        /// <returns>A ServicesResult containing a list of IndexMission objects or an error message if the operation fails.</returns>
        public async Task<ServicesResult<IEnumerable<IndexMission>>> GetIndexMissionsInPlan(string planId)
        {
            // Validate input to ensure planId is not null or empty
            if (string.IsNullOrEmpty(planId))
            {
                return ServicesResult<IEnumerable<IndexMission>>.Failure("PlanId is null");
            }

            try
            {
                // Retrieve missions that match the given plan ID
                var missions = await _unitOfWork.MissionRepository.GetManyByKeyAndValue("PlanId", planId);

                // Check if the retrieval operation was successful
                if (!missions.Status)
                {
                    return ServicesResult<IEnumerable<IndexMission>>.Failure(missions.Message);
                }

                var indexMissions = new List<IndexMission>();

                // Iterate through each mission and convert it to an IndexMission object
                foreach (var item in missions.Data)
                {
                    var index = new IndexMission
                    {
                        MissionId = item.Id,
                        MissionName = item.Name
                    };

                    // Retrieve the status information for the current mission
                    var status = await _unitOfWork.StatusRepository.GetOneByKeyAndValue("Id", item.StatusId);

                    // Check if the status retrieval was successful
                    if (!status.Status)
                    {
                        return ServicesResult<IEnumerable<IndexMission>>.Failure(status.Message);
                    }

                    // Set the status name for the mission
                    index.Status = status.Data.Name;

                    // Add the completed IndexMission object to the result list
                    indexMissions.Add(index);
                }

                // Return the successful result with the list of IndexMission objects
                return ServicesResult<IEnumerable<IndexMission>>.Success(indexMissions);
            }
            catch (Exception e)
            {
                // Handle any exceptions by returning a failure result with the exception message
                return ServicesResult<IEnumerable<IndexMission>>.Failure(e.Message);
            }
            finally
            {
                // Ensure the UnitOfWork is disposed to release any resources
                _unitOfWork.Dispose();
            }
        }
        #endregion


        #region Retrieve the detailed information for the mission with the specified ID, including status and assignments
        /// <summary>
        /// Retrieves the detailed information for the mission with the specified ID, including status and assignments.
        /// </summary>
        /// <param name="missionId">The ID of the mission to retrieve details for.</param>
        /// <returns>A ServicesResult containing the detailed mission information or an error message if the operation fails.</returns>
        public async Task<ServicesResult<DetailMission>> GetDetailMission(string missionId)
        {
            // Validate input to ensure missionId is not null or empty
            if (string.IsNullOrEmpty(missionId))
            {
                return ServicesResult<DetailMission>.Failure("MissionId is null");
            }

            try
            {
                // Retrieve the mission details by mission ID
                var detailMission = await _unitOfWork.MissionRepository.GetOneByKeyAndValue("Id", missionId);

                if (!detailMission.Status)
                {
                    return ServicesResult<DetailMission>.Failure(detailMission.Message);
                }

                // Map the retrieved data to a DetailMission object
                var responseDetail = new DetailMission
                {
                    MissionName = detailMission.Data.Name,
                    StartAt = detailMission.Data.StartDate,
                    CreateAt = detailMission.Data.CreateDate,
                    EndAt = detailMission.Data.EndDate,
                    IsDone = detailMission.Data.IsCompleted,
                    Description = detailMission.Data.Description
                };

                // Retrieve the mission status
                var status = await _unitOfWork.StatusRepository.GetOneByKeyAndValue("Id", detailMission.Data.StatusId);

                if (!status.Status)
                {
                    return ServicesResult<DetailMission>.Failure(status.Message);
                }

                responseDetail.Status = status.Data.Name;

                // Retrieve mission assignments for the given mission ID
                var assignments = await _unitOfWork.MissionAssignmentRepository.GetManyByKeyAndValue("MissionId", missionId);

                if (!assignments.Status)
                {
                    return ServicesResult<DetailMission>.Failure(assignments.Message);
                }

                // If there are no assignments, return the mission details
                if (!assignments.Data.Any())
                {
                    return ServicesResult<DetailMission>.Success(responseDetail);
                }

                // Process each assignment and add corresponding member details
                foreach (var item in assignments.Data)
                {
                    // Retrieve project member information
                    var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", item.ProjectMemberId);
                    if (!member.Status)
                    {
                        return ServicesResult<DetailMission>.Failure(member.Message);
                    }

                    // Retrieve user information for the project member
                    var infoMember = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.Data.UserId);
                    if (!infoMember.Status)
                    {
                        return ServicesResult<DetailMission>.Failure(infoMember.Message);
                    }

                    // Create an IndexMember object and add it to the mission detail
                    var index = new IndexMember
                    {
                        MemberId = item.ProjectMemberId,
                        UserName = infoMember.Data.UserName,
                        UserAvata = infoMember.Data.AvatarPath,
                        PositionWorkName = member.Data.PositionWork
                    };

                    responseDetail.IndexMembers.Add(index);
                }

                // Return the detailed mission information
                return ServicesResult<DetailMission>.Success(responseDetail);
            }
            catch (Exception e)
            {
                // Handle any exceptions by returning a failure result with the exception message
                return ServicesResult<DetailMission>.Failure(e.Message);
            }
            finally
            {
                // Ensure the UnitOfWork is disposed to release any resources
                _unitOfWork.Dispose();
            }
        }
        #endregion

        #region Creates a new mission for the specified plan and member
        /// <summary>
        /// Creates a new mission for the specified plan and member.
        /// </summary>
        /// <param name="memberId">The ID of the member creating the mission.</param>
        /// <param name="planId">The ID of the plan to which the mission belongs.</param>
        /// <param name="mission">The mission details to be created.</param>
        /// <returns>A ServicesResult containing the created mission's details or an error message if the creation fails.</returns>
        public async Task<ServicesResult<DetailMission>> CreateMission(string memberId, string planId, AddMission mission)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(planId) || mission is null)
            {
                return ServicesResult<DetailMission>.Failure("MemberId or PlanId or Mission is null");
            }

            try
            {
                var owner = await GetOwnRole();
                if (owner.Status == false) return ServicesResult<DetailMission>.Failure(owner.Message);

                var leader = await GetLeaderRole();
                if (leader.Status == false) return ServicesResult<DetailMission>.Failure(leader.Message);

                var manager = await GetManagerRole();
                if (manager.Status == false) return ServicesResult<DetailMission>.Failure(manager.Message);

                // Check if the member exists
                var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!member.Status)
                {
                    return ServicesResult<DetailMission>.Failure(member.Message);
                }

                // Check if the plan exists
                var plan = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", planId);
                if (!plan.Status)
                {
                    return ServicesResult<DetailMission>.Failure(plan.Message);
                }

                // Get all members in the project associated with the plan
                var members = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", plan.Data.ProjectId);
                if (!members.Status)
                {
                    return ServicesResult<DetailMission>.Failure(members.Message);
                }

                // Check if the member has sufficient permissions (leader, owner, or manager roles)
                var hasPermission = members.Data.Any(x => x.Id == memberId &&
                    (x.RoleId == _leaderId || x.RoleId == _ownerId || x.RoleId == _mamagerId));
                if (!hasPermission)
                {
                    return ServicesResult<DetailMission>.Failure("You do not have permission to create a mission");
                }

                // Check if a mission with the same name already exists in the plan
                var missions = await _unitOfWork.MissionRepository.GetManyByKeyAndValue("PlanId", planId);
                if (!missions.Status)
                {
                    return ServicesResult<DetailMission>.Failure(missions.Message);
                }

                var missionExists = missions.Data.Any(x => x.Name == mission.TaskName);
                if (missionExists)
                {
                    return ServicesResult<DetailMission>.Failure("The mission name already exists");
                }

                // Create a new mission object
                var newMission = new Mission
                {
                    Id = Guid.NewGuid().ToString(),
                    PlanId = planId,
                    Name = mission.TaskName,
                    Description = mission.Description,
                    StartDate = new DateTime(mission.StartAt.Year, mission.StartAt.Month, mission.StartAt.Day, 0, 0, 0),
                    EndDate = new DateTime(mission.EndAt.Year, mission.EndAt.Month, mission.EndAt.Day, 23, 59, 59),
                    CreateDate = DateTime.Now,
                    IsCompleted = false
                };

                // Set the mission status based on its start date
                newMission.StatusId = DateTime.Now == newMission.StartDate
                    ? 3 // Ongoing
                    : (DateTime.Now < newMission.StartDate ? 2 : 1); // Upcoming or Overdue

                // Add the new mission to the repository
                var addResponse = await _unitOfWork.MissionRepository.AddAsync(newMission);
                if (!addResponse.Status)
                {
                    return ServicesResult<DetailMission>.Failure(addResponse.Message);
                }

                // Retrieve the detailed information of the newly created mission
                var response = await GetDetailMission(newMission.Id);
                if (!response.Status)
                {
                    return ServicesResult<DetailMission>.Failure(response.Message);
                }

                // Return the successful response with the new mission details
                return ServicesResult<DetailMission>.Success(response.Data);
            }
            catch (Exception e)
            {
                // Handle any exceptions by returning a failure result with the exception message
                return ServicesResult<DetailMission>.Failure(e.Message);
            }
        }
        #endregion

        #region Updates the specified mission with new detail
        /// <summary>
        /// Updates the specified mission with new details.
        /// </summary>
        /// <param name="memberId">The ID of the member attempting to update the mission.</param>
        /// <param name="missionId">The ID of the mission to update.</param>
        /// <param name="newMission">The updated mission details.</param>
        /// <returns>A ServicesResult containing the updated mission details or an error message if the update fails.</returns>
        public async Task<ServicesResult<DetailMission>> UpdateMission(string memberId, string missionId, UpdateMission newMission)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(missionId) || newMission is null)
            {
                return ServicesResult<DetailMission>.Failure("MemberId or MissionId or Mission is null");
            }

            try
            {
                var owner = await GetOwnRole();
                if (owner.Status == false) return ServicesResult<DetailMission>.Failure(owner.Message);

                var leader = await GetLeaderRole();
                if (leader.Status == false) return ServicesResult<DetailMission>.Failure(leader.Message);

                var manager = await GetManagerRole();
                if (manager.Status == false) return ServicesResult<DetailMission>.Failure(manager.Message);

                // Check if the member exists
                var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!member.Status)
                {
                    return ServicesResult<DetailMission>.Failure(member.Message);
                }

                // Check if the mission exists
                var mission = await _unitOfWork.MissionRepository.GetOneByKeyAndValue("Id", missionId);
                if (!mission.Status)
                {
                    return ServicesResult<DetailMission>.Failure(mission.Message);
                }

                // Check if the plan associated with the mission exists
                var plan = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", mission.Data.PlanId);
                if (!plan.Status)
                {
                    return ServicesResult<DetailMission>.Failure(plan.Message);
                }

                // Get all members in the project associated with the plan
                var members = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", plan.Data.ProjectId);
                if (!members.Status)
                {
                    return ServicesResult<DetailMission>.Failure(members.Message);
                }

                // Check if the member has sufficient permissions (leader, owner, or manager roles)
                var hasPermission = members.Data.Any(x => x.Id == memberId &&
                    (x.RoleId == _leaderId || x.RoleId == _ownerId || x.RoleId == _mamagerId));
                if (!hasPermission)
                {
                    return ServicesResult<DetailMission>.Failure("You do not have permission to update a mission");
                }

                // Check if a mission with the same name already exists in the plan
                var missions = await _unitOfWork.MissionRepository.GetManyByKeyAndValue("PlanId", plan.Data.Id);
                if (!missions.Status)
                {
                    return ServicesResult<DetailMission>.Failure(missions.Message);
                }

                var missionExists = missions.Data.Any(x => x.Name == newMission.TaskName);
                if (missionExists)
                {
                    return ServicesResult<DetailMission>.Failure("The mission name already exists");
                }

                // Update the mission details
                mission.Data.Name = newMission.TaskName ?? mission.Data.Name;
                mission.Data.Description = newMission.TaskDescription ?? mission.Data.Description;
                mission.Data.StartDate = new DateTime(newMission.StartAt.Year, newMission.StartAt.Month, newMission.StartAt.Day, 0, 0, 0);
                mission.Data.EndDate = new DateTime(newMission.EndAt.Year, newMission.EndAt.Month, newMission.EndAt.Day, 23, 59, 59);

                // Set the mission status based on its start and end dates
                mission.Data.StatusId = DateTime.Now == mission.Data.StartDate
                    ? 3 // Ongoing
                    : (DateTime.Now < mission.Data.EndDate ? 2 : 1); // Upcoming or Overdue

                // Update the mission in the repository
                var updateResponse = await _unitOfWork.MissionRepository.UpdateAsync(mission.Data);
                if (!updateResponse.Status)
                {
                    return ServicesResult<DetailMission>.Failure(updateResponse.Message);
                }

                // Retrieve the updated mission details
                var response = await GetDetailMission(missionId);
                if (!response.Status)
                {
                    return ServicesResult<DetailMission>.Failure(response.Message);
                }

                // Return the updated mission details
                return ServicesResult<DetailMission>.Success(response.Data);
            }
            catch (Exception e)
            {
                // Handle any exceptions by returning a failure result with the exception message
                return ServicesResult<DetailMission>.Failure(e.Message);
            }
        }
        #endregion

        #region
        /// <summary>
        /// Toggles the completion status of a mission.
        /// </summary>
        /// <param name="memberId">The ID of the member attempting to update the mission's completion status.</param>
        /// <param name="missionId">The ID of the mission to update.</param>
        /// <returns>A ServicesResult containing the updated mission details or an error message if the update fails.</returns>
        public async Task<ServicesResult<DetailMission>> UpdateIsDone(string memberId, string missionId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(missionId))
            {
                return ServicesResult<DetailMission>.Failure("MemberId or MissionId is null");
            }

            try
            {

                var owner = await GetOwnRole();
                if (owner.Status == false) return ServicesResult<DetailMission>.Failure(owner.Message);

                var leader = await GetLeaderRole();
                if (leader.Status == false) return ServicesResult<DetailMission>.Failure(leader.Message);

                var manager = await GetManagerRole();
                if (manager.Status == false) return ServicesResult<DetailMission>.Failure(manager.Message);

                // Check if the member exists
                var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!member.Status)
                {
                    return ServicesResult<DetailMission>.Failure(member.Message);
                }

                // Check if the mission exists
                var mission = await _unitOfWork.MissionRepository.GetOneByKeyAndValue("Id", missionId);
                if (!mission.Status)
                {
                    return ServicesResult<DetailMission>.Failure(mission.Message);
                }

                // Check if the plan associated with the mission exists
                var plan = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", mission.Data.PlanId);
                if (!plan.Status)
                {
                    return ServicesResult<DetailMission>.Failure(plan.Message);
                }

                // Get all members in the project associated with the plan
                var members = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", plan.Data.ProjectId);
                if (!members.Status)
                {
                    return ServicesResult<DetailMission>.Failure(members.Message);
                }

                // Check if the member has sufficient permissions (leader, owner, or manager roles)
                var hasPermission = members.Data.Any(x => x.Id == memberId &&
                    (x.RoleId == _leaderId || x.RoleId == _ownerId || x.RoleId == _mamagerId));
                if (!hasPermission)
                {
                    return ServicesResult<DetailMission>.Failure("You do not have permission to update a mission");
                }

                // Toggle the completion status of the mission
                mission.Data.IsCompleted = !mission.Data.IsCompleted;

                // Update the mission in the repository
                var updateResponse = await _unitOfWork.MissionRepository.UpdateAsync(mission.Data);
                if (!updateResponse.Status)
                {
                    return ServicesResult<DetailMission>.Failure(updateResponse.Message);
                }

                // Retrieve the updated mission details
                var response = await GetDetailMission(missionId);
                if (!response.Status)
                {
                    return ServicesResult<DetailMission>.Failure(response.Message);
                }

                // Return the updated mission details
                return ServicesResult<DetailMission>.Success(response.Data);
            }
            catch (Exception e)
            {
                // Handle any exceptions by returning a failure result with the exception message
                return ServicesResult<DetailMission>.Failure(e.Message);
            }
        }

        #endregion

        #region Deletes a mission along with its assignments
        /// <summary>
        /// Deletes a mission along with its assignments.
        /// </summary>
        /// <param name="memberId">The ID of the member attempting to delete the mission.</param>
        /// <param name="missionId">The ID of the mission to be deleted.</param>
        /// <returns>A ServicesResult containing the updated list of missions or an error message if the deletion fails.</returns>
        public async Task<ServicesResult<IEnumerable<IndexMission>>> DeleteMission(string memberId, string missionId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(missionId))
            {
                return ServicesResult<IEnumerable<IndexMission>>.Failure("MemberId or MissionId is null");
            }

            try
            {
                var owner = await GetOwnRole();
                if (owner.Status == false) return ServicesResult<IEnumerable<IndexMission>>.Failure(owner.Message);

                var leader = await GetLeaderRole();
                if (leader.Status == false) return ServicesResult<IEnumerable<IndexMission>>.Failure(leader.Message);

                var manager = await GetManagerRole();
                if (manager.Status == false) return ServicesResult<IEnumerable<IndexMission>>.Failure(manager.Message);

                // Check if the member exists
                var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!member.Status)
                {
                    return ServicesResult<IEnumerable<IndexMission>>.Failure(member.Message);
                }

                // Check if the mission exists
                var mission = await _unitOfWork.MissionRepository.GetOneByKeyAndValue("Id", missionId);
                if (!mission.Status)
                {
                    return ServicesResult<IEnumerable<IndexMission>>.Failure(mission.Message);
                }

                // Check if the plan associated with the mission exists
                var plan = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", mission.Data.PlanId);
                if (!plan.Status)
                {
                    return ServicesResult<IEnumerable<IndexMission>>.Failure(plan.Message);
                }

                // Get all members in the project associated with the plan
                var members = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", plan.Data.ProjectId);
                if (!members.Status)
                {
                    return ServicesResult<IEnumerable<IndexMission>>.Failure(members.Message);
                }

                // Check if the member has sufficient permissions (leader, owner, or manager roles)
                var hasPermission = members.Data.Any(x => x.Id == memberId && (x.RoleId == _leaderId || x.RoleId == _ownerId || x.RoleId == _mamagerId));
                if (!hasPermission)
                {
                    return ServicesResult<IEnumerable<IndexMission>>.Failure("You do not have permission to delete this mission");
                }

                // Retrieve all mission assignments
                var missionAssignments = await _unitOfWork.MissionAssignmentRepository.GetManyByKeyAndValue("MissionId", missionId);
                if (!missionAssignments.Status)
                {
                    return ServicesResult<IEnumerable<IndexMission>>.Failure(missionAssignments.Message);
                }

                // Delete all mission assignments if they exist
                foreach (var assignment in missionAssignments.Data)
                {
                    var deleteAssignmentResponse = await _unitOfWork.MissionAssignmentRepository.DeleteAsync(assignment.Id);
                    if (!deleteAssignmentResponse.Status)
                    {
                        return ServicesResult<IEnumerable<IndexMission>>.Failure(deleteAssignmentResponse.Message);
                    }
                }

                // Delete the mission
                var deleteMissionResponse = await _unitOfWork.MissionRepository.DeleteAsync(mission.Data.Id);
                if (!deleteMissionResponse.Status)
                {
                    return ServicesResult<IEnumerable<IndexMission>>.Failure(deleteMissionResponse.Message);
                }

                // Retrieve the updated list of missions in the plan
                var updatedMissionsResponse = await GetIndexMissionsInPlan(plan.Data.Id);
                if (!updatedMissionsResponse.Status)
                {
                    return ServicesResult<IEnumerable<IndexMission>>.Failure(updatedMissionsResponse.Message);
                }

                // Return the updated mission list
                return ServicesResult<IEnumerable<IndexMission>>.Success(updatedMissionsResponse.Data);
            }
            catch (Exception e)
            {
                // Handle any exceptions by returning a failure result with the exception message
                return ServicesResult<IEnumerable<IndexMission>>.Failure(e.Message);
            }
        }

        #endregion

        #region Private method helper
        /// <summary>
        /// Gets the role ID by role name.
        /// </summary>
        /// <param name="roleName">The name of the role to fetch.</param>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetRoleByName(string roleName)
        {
            try
            {
                var role = await _unitOfWork.RoleInProjectRepository.GetOneByKeyAndValue("Name", roleName);
                if (!role.Status)
                    return ServicesResult<bool>.Failure(role.Message);

                // Assign the role ID to the appropriate variable
                switch (roleName)
                {
                    case "Owner":
                        _ownerId = role.Data.Id;
                        break;
                    case "Leader":
                        _leaderId = role.Data.Id;
                        break;
                    case "Manager":
                        _mamagerId = role.Data.Id;
                        break;
                    default:
                        return ServicesResult<bool>.Failure("Invalid role name");
                }

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure(ex.Message);
            }
            finally
            {
                // Remove this if you don’t want to dispose the unit of work here.
                _unitOfWork.Dispose();
            }
        }

        /// <summary>
        /// Gets the role ID for the "Owner" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetOwnRole()
        {
            return await GetRoleByName("Owner");
        }

        /// <summary>
        /// Gets the role ID for the "Leader" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetLeaderRole()
        {
            return await GetRoleByName("Leader");
        }

        /// <summary>
        /// Gets the role ID for the "Manager" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetManagerRole()
        {
            return await GetRoleByName("Manager");
        }


        #endregion
    }
}
