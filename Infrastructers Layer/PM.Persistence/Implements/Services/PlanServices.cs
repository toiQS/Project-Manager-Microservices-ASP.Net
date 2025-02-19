using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.missions;
using PM.Domain.Models.plans;
using PM.Domain.Models.progressReports;
using System.Data.Entity.Core.Metadata.Edm;

namespace PM.Persistence.Implements.Services
{
    public class PlanServices : IPlanServices
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMissionServices _missionServices;
        private string _ownerId;
        private string _leaderId;
        private string _managerId;
        private string _memberId;
        public PlanServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //eveyone can create, update, and delete when your role is owner, leader, manager

        #region Retrieves all plans in the system
        /// <summary>
        /// Retrieves all plans in the system.
        /// </summary>
        /// <returns>A list of plans, or an error if the operation fails.</returns>
        public async Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlans()
        {
            try
            {
                var plans = await _unitOfWork.PlanRepository.GetAllAsync();
                if (!plans.Status)
                    return ServicesResult<IEnumerable<IndexPlan>>.Failure(plans.Message);

                var response = plans.Data.Select(x => new IndexPlan
                {
                    PlanName = x.Name,
                    Description = x.Description,
                    PlanId = x.Id
                }).ToList();

                return ServicesResult<IEnumerable<IndexPlan>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexPlan>>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        #endregion

        #region Retrieves all plans associated with a specific project
        /// <summary>
        /// Retrieves all plans associated with a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A list of plans in the project, or an error if the operation fails.</returns>
        public async Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlansInProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<IEnumerable<IndexPlan>>.Failure("Invalid project ID");

            try
            {
                var planProjects = await _unitOfWork.PlanRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!planProjects.Status)
                    return ServicesResult<IEnumerable<IndexPlan>>.Failure(planProjects.Message);

                var response = planProjects.Data.Select(x => new IndexPlan
                {
                    PlanName = x.Name,
                    Description = x.Description,
                    PlanId = x.Id
                }).ToList();

                return ServicesResult<IEnumerable<IndexPlan>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexPlan>>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        #endregion

        #region Retrieves detailed information about a specific plan

        /// <summary>
        /// Retrieves detailed information about a specific plan.
        /// </summary>
        /// <param name="planId">The ID of the plan to retrieve.</param>
        /// <returns>Detailed plan information, including related missions and progress reports.</returns>
        public async Task<ServicesResult<DetailPlan>> GetDetailPlan(string planId)
        {
            if (string.IsNullOrEmpty(planId))
                return ServicesResult<DetailPlan>.Failure("Invalid plan ID");

            try
            {
                // Retrieve plan details
                var plan = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", planId);
                if (!plan.Status)
                    return ServicesResult<DetailPlan>.Failure(plan.Message);

                var response = new DetailPlan
                {
                    PlanId = planId,
                    PlanName = plan.Data.Name,
                    Description = plan.Data.Description,
                    StartDate = new DateOnly(plan.Data.StartDate.Year, plan.Data.StartDate.Month, plan.Data.StartDate.Day),
                    EndDate = new DateOnly(plan.Data.EndDate.Year, plan.Data.EndDate.Month, plan.Data.EndDate.Day),
                    IsCompleted = plan.Data.IsCompleted,
                    Missions = new List<IndexMission>()
                };

                // Retrieve plan status
                var statusResponse = await _unitOfWork.StatusRepository.GetOneByKeyAndValue("Id", plan.Data.StatusId);
                if (!statusResponse.Status)
                    return ServicesResult<DetailPlan>.Failure(statusResponse.Message);

                response.Status = statusResponse.Data.Name;

                // Retrieve associated missions
                var missions = await _unitOfWork.MissionRepository.GetManyByKeyAndValue("PlanId", planId);
                if (!missions.Status)
                    return ServicesResult<DetailPlan>.Failure(missions.Message);

                foreach (var mission in missions.Data)
                {
                    var missionDetail = new IndexMission
                    {
                        MissionId = mission.Id,
                        MissionName = mission.Name
                    };

                    // Retrieve mission status
                    var missionStatus = await _unitOfWork.StatusRepository.GetOneByKeyAndValue("Id", mission.StatusId);
                    if (!missionStatus.Status)
                        return ServicesResult<DetailPlan>.Failure(missionStatus.Message);

                    missionDetail.Status = missionStatus.Data.Name;
                    response.Missions.Add(missionDetail);
                }

                // Retrieve progress reports
                var progressReports = await _unitOfWork.ProgressReportRepository.GetManyByKeyAndValue("PlanId", planId);
                if (!progressReports.Status)
                    return ServicesResult<DetailPlan>.Failure(progressReports.Message);

                response.ProgressReports = progressReports.Data.Select(x => new IndexRepost
                {
                    Id = x.Id,
                    RepostDate = new DateOnly(x.ReportDate.Year, x.ReportDate.Month, x.ReportDate.Day),
                    RepostDetail = x.ReportDetails
                }).ToList();

                return ServicesResult<DetailPlan>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailPlan>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        #endregion

        #region add a new plan to a project if the member has the required permissions
        /// <summary>
        /// Adds a new plan to a project if the member has the required permissions.
        /// </summary>
        /// <param name="memberId">The ID of the member creating the plan.</param>
        /// <param name="projectId">The ID of the project to add the plan to.</param>
        /// <param name="addPlan">The details of the plan to be added.</param>
        /// <returns>The details of the newly created plan, or an error message if the operation fails.</returns>
        public async Task<ServicesResult<DetailPlan>> AddAsync(string memberId, string projectId, AddPlan addPlan)
        {
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(projectId) || addPlan == null)
                return ServicesResult<DetailPlan>.Failure("Invalid input parameters.");

            try
            {
                var ownerRole = await GetOwnRole();
                if (ownerRole.Status == false)
                    return ServicesResult<DetailPlan>.Failure(ownerRole.Message);
                var leaderRole = await GetLeaderRole();
                if (leaderRole.Status == false)
                    return ServicesResult<DetailPlan>.Failure(leaderRole.Message);
                var managerRole = await GetManagerRole();
                if (managerRole.Status == false)
                    return ServicesResult<DetailPlan>.Failure(managerRole.Message);
                // Verify member and their permissions
                var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!member.Status) return ServicesResult<DetailPlan>.Failure(member.Message);

                var memberInfo = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.Data.UserId);
                if (!memberInfo.Status) return ServicesResult<DetailPlan>.Failure(memberInfo.Message);

                var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!project.Status) return ServicesResult<DetailPlan>.Failure(project.Message);

                var members = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!members.Status) return ServicesResult<DetailPlan>.Failure(members.Message);

                var hasPermission = members.Data.Any(x => x.Id == memberId && (x.RoleId == _leaderId || x.RoleId == _memberId || x.RoleId == _ownerId));
                if (!hasPermission) return ServicesResult<DetailPlan>.Failure("You do not have permission to create a plan in this project.");

                // Check for duplicate plan names
                var plans = await GetPlansInProject(projectId);
                if (!plans.Status) return ServicesResult<DetailPlan>.Failure(plans.Message);

                if (plans.Data.Any(x => x.PlanName == addPlan.PlanName))
                    return ServicesResult<DetailPlan>.Failure("This plan name already exists in the project.");

                // Create the new plan
                var plan = new Plan
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = addPlan.PlanName,
                    Description = addPlan.Description,
                    StartDate = new DateTime(addPlan.StartAt.Year, addPlan.StartAt.Month, addPlan.StartAt.Day),
                    EndDate = new DateTime(addPlan.EndAt.Year, addPlan.EndAt.Month, addPlan.EndAt.Day),
                    IsCompleted = false,
                    ProjectId = projectId,

                };
                plan.StatusId = DateTime.Now == plan.StartDate ? 3 : (DateTime.Now < plan.StartDate ? 2 : 1); // Ongoing, Upcoming, or Overdue
                var responseAdd = await _unitOfWork.PlanRepository.AddAsync(plan);
                if (!responseAdd.Status) return ServicesResult<DetailPlan>.Failure(responseAdd.Message);

                // Log the action
                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = $"{memberInfo.Data.UserName} created a plan {plan.Name} in project {project.Data.Name}.",
                    ProjectId = projectId,
                    ActionDate = DateTime.Now,
                    UserId = memberInfo.Data.Id
                };

                var responseLog = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!responseLog.Status) return ServicesResult<DetailPlan>.Failure(responseLog.Message);

                return await GetDetailPlan(responseAdd.Data.Id);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailPlan>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }
        #endregion

        #region Updates an existing plan if the member has the required permissions

        /// <summary>
        /// Updates an existing plan if the member has the required permissions.
        /// </summary>
        /// <param name="memberId">The ID of the member updating the plan.</param>
        /// <param name="planId">The ID of the plan to update.</param>
        /// <param name="updatePlan">The new details for the plan.</param>
        /// <returns>The updated plan details, or an error message if the operation fails.</returns>
        public async Task<ServicesResult<DetailPlan>> UpdateAsync(string memberId, string planId, UpdatePlan updatePlan)
        {
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(planId) || updatePlan == null)
                return ServicesResult<DetailPlan>.Failure("Invalid input parameters.");

            try
            {
                var ownerRole = await GetOwnRole();
                if (ownerRole.Status == false)
                    return ServicesResult<DetailPlan>.Failure(ownerRole.Message);
                var leaderRole = await GetLeaderRole();
                if (leaderRole.Status == false)
                    return ServicesResult<DetailPlan>.Failure(leaderRole.Message);
                var managerRole = await GetManagerRole();
                if (managerRole.Status == false)
                    return ServicesResult<DetailPlan>.Failure(managerRole.Message);
                // Verify member and their permissions
                var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!member.Status) return ServicesResult<DetailPlan>.Failure(member.Message);

                var memberInfo = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.Data.UserId);
                if (!memberInfo.Status) return ServicesResult<DetailPlan>.Failure(memberInfo.Message);

                var plan = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", planId);
                if (!plan.Status) return ServicesResult<DetailPlan>.Failure(plan.Message);

                var members = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", plan.Data.ProjectId);
                if (!members.Status) return ServicesResult<DetailPlan>.Failure(members.Message);

                var hasPermission = members.Data.Any(x => x.Id == memberId && (x.RoleId == _leaderId || x.RoleId == _managerId || x.RoleId == _ownerId));
                if (!hasPermission) return ServicesResult<DetailPlan>.Failure("You do not have permission to update this plan.");

                // Check for duplicate plan names
                var plans = await GetPlansInProject(plan.Data.ProjectId);
                if (!plans.Status) return ServicesResult<DetailPlan>.Failure(plans.Message);

                if (plans.Data.Any(x => x.PlanName == updatePlan.PlanName && x.PlanId != planId))
                    return ServicesResult<DetailPlan>.Failure("This plan name already exists in the project.");

                // Update the plan
                plan.Data.Name = updatePlan.PlanName ?? plan.Data.Name;
                plan.Data.Description = updatePlan.Description ?? plan.Data.Description;
                plan.Data.StartDate = new DateTime(updatePlan.StartAt.Year, updatePlan.StartAt.Month, updatePlan.StartAt.Day);
                plan.Data.EndDate = new DateTime(updatePlan.EndAt.Year, updatePlan.EndAt.Month, updatePlan.EndAt.Day);
                plan.Data.StatusId = DateTime.Now == plan.Data.StartDate ? 3 : (DateTime.Now < plan.Data.StartDate ? 2 : 1); // Ongoing, Upcoming, or Overdue

                var responseUpdate = await _unitOfWork.PlanRepository.UpdateAsync(plan.Data);
                if (!responseUpdate.Status) return ServicesResult<DetailPlan>.Failure(responseUpdate.Message);

                // Log the action
                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = $"{memberInfo.Data.UserName} updated the plan {plan.Data.Name} in project {plan.Data.Project.Name}.",
                    ActionDate = DateTime.Now,
                    ProjectId = plan.Data.ProjectId,
                    UserId = memberInfo.Data.Id
                };

                var responseLog = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!responseLog.Status) return ServicesResult<DetailPlan>.Failure(responseLog.Message);

                return await GetDetailPlan(planId);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailPlan>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }
        #endregion

        #region Deletes a plan and its associated missions if the user has sufficient permissions.
        /// <summary>
        /// Deletes a plan and its associated missions if the user has sufficient permissions.
        /// </summary>
        /// <param name="memberId">The ID of the member requesting the deletion.</param>
        /// <param name="planId">The ID of the plan to delete.</param>
        /// <returns>A service result containing the updated list of plans or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexPlan>>> DeleteAsync(string memberId, string planId)
        {
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(planId))
                return ServicesResult<IEnumerable<IndexPlan>>.Failure("Invalid input parameters.");

            try
            {
                var memberRole = await GetMemberRole();
                if (memberRole.Status == false)
                    return ServicesResult<IEnumerable<IndexPlan>>.Failure(memberRole.Message);
                // Fetch the plan
                var plan = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", planId);
                if (!plan.Status)
                    return ServicesResult<IEnumerable<IndexPlan>>.Failure($"Failed to retrieve plan: {plan.Message}");

                // Fetch all members in the project
                var members = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", plan.Data.ProjectId);
                if (!members.Status)
                    return ServicesResult<IEnumerable<IndexPlan>>.Failure($"Failed to retrieve project members: {members.Message}");

                // Fetch the current member
                var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!member.Status)
                    return ServicesResult<IEnumerable<IndexPlan>>.Failure($"Failed to retrieve member: {member.Message}");

                // Check if the member has permission to delete the plan
                var hasPermission = members.Data.Any(x => x.Id == memberId && x.ProjectId == plan.Data.ProjectId && x.RoleId != _memberId);
                if (!hasPermission)
                    return ServicesResult<IEnumerable<IndexPlan>>.Failure("You do not have permission to delete this plan.");

                // Fetch all missions associated with the plan
                var planMissions = await _unitOfWork.MissionRepository.GetManyByKeyAndValue("PlanId", planId);
                if (!planMissions.Status)
                    return ServicesResult<IEnumerable<IndexPlan>>.Failure($"Failed to retrieve missions: {planMissions.Message}");

                // Delete all missions in the plan
                foreach (var mission in planMissions.Data)
                {
                    var deleteMissionResponse = await _missionServices.DeleteMission(memberId, mission.Id);
                    if (!deleteMissionResponse.Status)
                        return ServicesResult<IEnumerable<IndexPlan>>.Failure($"Failed to delete mission '{mission.Id}': {deleteMissionResponse.Message}");
                }

                // Delete the plan and log the action
                return await DeletePlanSupport(memberId, planId, plan, member);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexPlan>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes the specified plan and logs the action.
        /// </summary>
        /// <param name="memberId">The ID of the member performing the deletion.</param>
        /// <param name="planId">The ID of the plan to delete.</param>
        /// <param name="plan">The plan to be deleted.</param>
        /// <param name="member">The member performing the action.</param>
        /// <returns>A service result containing the updated list of plans or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexPlan>>> DeletePlanSupport(string memberId, string planId, ServicesResult<Plan> plan, ServicesResult<ProjectMember> member)
        {
            // Delete the plan
            var deletePlanResponse = await _unitOfWork.PlanRepository.DeleteAsync(planId);
            if (!deletePlanResponse.Status)
                return ServicesResult<IEnumerable<IndexPlan>>.Failure($"Failed to delete plan: {deletePlanResponse.Message}");

            // Log the deletion action
            var infoMember = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.Data.UserId);
            if (!infoMember.Status)
                return ServicesResult<IEnumerable<IndexPlan>>.Failure($"Failed to retrieve user information: {infoMember.Message}");

            var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", plan.Data.ProjectId);
            if (!project.Status)
                return ServicesResult<IEnumerable<IndexPlan>>.Failure("Failed to retrieve project details.");

            var log = new ActivityLog
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = plan.Data.ProjectId,
                UserId = member.Data.Id,
                ActionDate = DateTime.Now,
                Action = $"Plan '{plan.Data.Name}' was deleted by {infoMember.Data.UserName} in project '{project.Data.Name}'.",
            };

            var logResponse = await _unitOfWork.ActivityLogRepository.AddAsync(log);
            if (!logResponse.Status)
                return ServicesResult<IEnumerable<IndexPlan>>.Failure($"Failed to create activity log: {logResponse.Message}");

            // Return the updated list of plans
            return await GetPlansInProject(project.Data.Id);
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
                        _managerId = role.Data.Id;
                        break;
                    case "Member":
                        _memberId = role.Data.Id;
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

        /// <summary>
        /// Gets the role ID for the "Member" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetMemberRole()
        {
            return await GetRoleByName("Member");
        }


        #endregion
    }
}

