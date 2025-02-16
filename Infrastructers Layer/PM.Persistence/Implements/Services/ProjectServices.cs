using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.members;
using PM.Domain.Models.plans;
using PM.Domain.Models.projects;

namespace PM.Persistence.Implements.Services
{
    public class ProjectServices : IProjectServices
    {
        #region Contructor
        private readonly IUnitOfWork _unitOfWork;
        private string _ownRoleId;
        private readonly IPlanServices _planServices;
        private readonly IMemberServices _memberServices;
        public ProjectServices(IUnitOfWork unitOfWork, IPlanServices planServices, IMemberServices memberServices)
        {
            _unitOfWork = unitOfWork;
            _planServices = planServices;
            _memberServices = memberServices;
        }
        #endregion

        #region Retrive project list user has joined

        /// <summary>
        /// Retrieves a list of projects that a user has joined.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A service result containing a list of projects.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProductListUserHasJoined(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<IEnumerable<IndexProject>>.Failure("User ID cannot be null or empty.");

            var response = new List<IndexProject>();

            try
            {
                // Verify the current user's role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(ownRoleResult.Message);

                // Check if the user exists in the database
                bool userExists = await _unitOfWork.UserRepository.ExistsAsync(userId);
                if (!userExists)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("User not found in database.");

                // Retrieve the list of projects the user has joined
                var projectsResult = await _unitOfWork.ProjectRepository.GetManyByKeyAndValue("UserId", userId);
                if (!projectsResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(projectsResult.Message);

                if (projectsResult.Data == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Success(response);

                foreach (var project in projectsResult.Data)
                {
                    // Retrieve project members
                    var membersResult = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", project.Id);
                    if (!membersResult.Status)
                        return ServicesResult<IEnumerable<IndexProject>>.Failure("Failed to retrieve project members.");

                    // Identify the project owner
                    var owner = membersResult.Data.FirstOrDefault(x => x.RoleId == _ownRoleId);
                    if (owner == null)
                        return ServicesResult<IEnumerable<IndexProject>>.Failure("Project owner not found.");

                    // Retrieve user information
                    var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                    if (!userResult.Status)
                        return ServicesResult<IEnumerable<IndexProject>>.Failure(userResult.Message);

                    // Map project details to response object
                    var indexProject = new IndexProject
                    {
                        ProjectId = project.Id,
                        ProjectName = project.Name,
                        OwnerAvata = userResult.Data.AvatarPath,
                        OwnerName = userResult.Data.UserName,
                    };

                    response.Add(indexProject);
                }

                return ServicesResult<IEnumerable<IndexProject>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("An unexpected error occurred.");
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Retrive project list user has owner

        /// <summary>
        /// Retrieves a list of projects where the user is the owner.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A service result containing a list of owned projects.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectListUserHasOwner(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<IEnumerable<IndexProject>>.Failure("User ID cannot be null or empty.");

            var response = new List<IndexProject>();

            try
            {
                // Verify the current user's role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(ownRoleResult.Message);

                // Retrieve projects where the user is a member
                var projectsResult = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (!projectsResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(projectsResult.Message);

                // Filter projects where the user is the owner
                var ownerProjects = projectsResult.Data.Where(x => x.RoleId == _ownRoleId).ToList();
                if (!ownerProjects.Any())
                    return ServicesResult<IEnumerable<IndexProject>>.Success(response);

                // Retrieve user information
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!userResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(userResult.Message);

                foreach (var item in ownerProjects)
                {
                    // Retrieve project details
                    var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", item.ProjectId);
                    if (!projectResult.Status)
                        return ServicesResult<IEnumerable<IndexProject>>.Failure(projectResult.Message);

                    // Map project details to response object
                    var indexProject = new IndexProject
                    {
                        ProjectId = item.ProjectId,
                        OwnerName = userResult.Data.UserName,
                        OwnerAvata = userResult.Data.AvatarPath,
                        ProjectName = projectResult.Data.Name
                    };

                    response.Add(indexProject);
                }

                return ServicesResult<IEnumerable<IndexProject>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("An unexpected error occurred.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Retrieve project details

        /// <summary>
        /// Retrieves the details of a specific project.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A service result containing the project details.</returns>
        public async Task<ServicesResult<DetailProject>> GetDetailProject(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<DetailProject>.Failure("User ID or Project ID cannot be null or empty.");

            try
            {
                // Verify the current user's role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<DetailProject>.Failure(ownRoleResult.Message);

                // Check if the user exists
                var isUserExists = await _unitOfWork.UserRepository.ExistAsync("Id", userId);
                if (!isUserExists)
                    return ServicesResult<DetailProject>.Failure("User not found.");

                // Retrieve project information
                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!projectResult.Status)
                    return ServicesResult<DetailProject>.Failure(projectResult.Message);

                // Retrieve project members
                var memberProjectResult = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!memberProjectResult.Status)
                    return ServicesResult<DetailProject>.Failure(memberProjectResult.Message);

                // Identify the project owner
                var ownerProject = memberProjectResult.Data.FirstOrDefault(x => x.RoleId == _ownRoleId);
                if (ownerProject == null)
                    return ServicesResult<DetailProject>.Failure("No owner found for this project.");

                // Retrieve owner details
                var ownerInfoResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", ownerProject.UserId);
                if (!ownerInfoResult.Status)
                    return ServicesResult<DetailProject>.Failure(ownerInfoResult.Message);

                // Construct response
                var response = new DetailProject
                {
                    OwnerName = ownerInfoResult.Data.UserName,
                    OwnerAvata = ownerInfoResult.Data.AvatarPath,
                    ProjectId = projectId,
                    ProjectName = projectResult.Data.Name,
                    ProjectDescription = projectResult.Data.Description,
                    CreateAt = projectResult.Data.CreatedDate,
                    StartAt = projectResult.Data.StartDate,
                    EndAt = projectResult.Data.EndDate,
                    IsCompleted = projectResult.Data.IsCompleted,
                    IsDeleted = projectResult.Data.IsDeleted,
                    QuantityMember = memberProjectResult.Data.Count(),
                    Members = new List<IndexMember>(),
                    Plans = new List<IndexPlan>()
                };

                // Retrieve project status
                var statusResult = await _unitOfWork.StatusRepository.GetOneByKeyAndValue("Id", projectResult.Data.StatusId);
                if (!statusResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to retrieve project status: {statusResult.Message}");
                response.Status = statusResult.Data.Name;

                // Retrieve and map project members
                foreach (var member in memberProjectResult.Data)
                {
                    var memberInfoResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.UserId);
                    if (!memberInfoResult.Status)
                        return ServicesResult<DetailProject>.Failure(memberInfoResult.Message);

                    response.Members.Add(new IndexMember
                    {
                        PositionWorkName = member.PositionWork,
                        UserName = memberInfoResult.Data.UserName,
                        RoleUserInProjectId = memberInfoResult.Data.Id,
                        UserAvata = memberInfoResult.Data.AvatarPath
                    });
                }

                // Retrieve and map project plans
                var planResult = await _unitOfWork.PlanRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!planResult.Status)
                    return ServicesResult<DetailProject>.Failure(planResult.Message);

                if (planResult.Data != null)
                {
                    response.Plans = planResult.Data.Select(plan => new IndexPlan
                    {
                        PlanName = plan.Name,
                        PlanId = plan.Id,
                        Description = plan.Description
                    }).ToList();
                }

                return ServicesResult<DetailProject>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure($"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Adds a new project for the user

        /// <summary>
        /// Adds a new project for the user.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="addProject">Project details</param>
        /// <returns>Service result containing project details</returns>
        public async Task<ServicesResult<DetailProject>> Add(string userId, AddProject addProject)
        {
            if (string.IsNullOrEmpty(userId) || addProject == null)
                return ServicesResult<DetailProject>.Failure("Invalid input");

            try
            {
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<DetailProject>.Failure(ownRoleResult.Message);

                var projectUser = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (!projectUser.Status)
                    return ServicesResult<DetailProject>.Failure(projectUser.Message);

                var projects = projectUser.Data.Where(x => x.RoleId == _ownRoleId).ToList();
                if (!projects.Any())
                    return await AddMethodSupport(userId, addProject);

                foreach (var item in projects)
                {
                    var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", item.ProjectId);
                    if (!project.Status)
                        return ServicesResult<DetailProject>.Failure(project.Message);

                    if (project.Data.Name == addProject.ProjectName)
                        return ServicesResult<DetailProject>.Failure("Project name already exists");
                }
                return await AddMethodSupport(userId, addProject);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure(ex.Message);
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }

        /// <summary>
        /// Supports adding a new project.
        /// </summary>
        private async Task<ServicesResult<DetailProject>> AddMethodSupport(string userId, AddProject addProject)
        {
            try
            {
                // add new project
                var project = new Project()
                {
                    Id = $"",
                    Name = addProject.ProjectName,
                    StartDate = addProject.StartAt,
                    EndDate = addProject.EndAt,
                    CreatedDate = DateTime.Now,
                    IsCompleted = false,
                    IsDeleted = false,
                    StatusId = DateTime.Now == addProject.StartAt
                    ? 3 // Ongoing
                    : (DateTime.Now < addProject.StartAt ? 2 : 1) // Upcoming or Overdue
                };
                var responseProject = await _unitOfWork.ProjectRepository.AddAsync(project);
                if (!responseProject.Status) return ServicesResult<DetailProject>.Failure(responseProject.Message);


                // set up owner role 
                var member = new ProjectMember()
                {
                    Id = "",
                    ProjectId = project.Id,
                    RoleId = _ownRoleId,
                    UserId = userId,
                    PositionWork = string.Empty
                };
                var projectMemberResponse = await _unitOfWork.ProjectMemberRepository.AddAsync(member);
                if (!projectMemberResponse.Status) return ServicesResult<DetailProject>.Failure(projectMemberResponse.Message);

                var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!user.Status) return ServicesResult<DetailProject>.Failure(user.Message);

                // create a activity log for project and owner role project
                var activityProject = new ActivityLog()
                {
                    Id = "",
                    ActionDate = DateTime.Now,
                    Action = $"A new project was created by {user.Data.UserName}",
                    ProjectId = project.Id,
                };
                var acvitity = await _unitOfWork.ActivityLogRepository.AddAsync(activityProject);
                if (!acvitity.Status) return ServicesResult<DetailProject>.Failure(acvitity.Message);

                // xác thực thành công

                var response = await GetDetailProject(userId, project.Id);
                if (response.Status == false) return ServicesResult<DetailProject>.Failure(response.Message);
                return ServicesResult<DetailProject>.Success(response.Data);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure(ex.Message);
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Update project information

        /// <summary>
        /// Updates project information.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="projectId">Project ID</param>
        /// <param name="updateProject">Updated project details</param>
        /// <returns>Service result containing updated project details</returns>
        public async Task<ServicesResult<DetailProject>> UpdateInfo(string userId, string projectId, UpdateProject updateProject)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId) || updateProject == null)
                return ServicesResult<DetailProject>.Failure("Invalid input");

            try
            {
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<DetailProject>.Failure(ownRoleResult.Message);

                var projectUser = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (!projectUser.Status || !projectUser.Data.Any())
                    return ServicesResult<DetailProject>.Failure(projectUser.Message ?? "No find any projects for update");

                bool isUserAuthorized = projectUser.Data.Any(x => x.RoleId == _ownRoleId && x.ProjectId == projectId);
                if (!isUserAuthorized)
                    return ServicesResult<DetailProject>.Failure("User does not have permission to update this project");

                return await UpdateMethodSupport(userId, projectId, updateProject);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure(ex.Message);
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }

        /// <summary>
        /// Supports updating an existing project.
        /// </summary>
        private async Task<ServicesResult<DetailProject>> UpdateMethodSupport(string userId, string projectId, UpdateProject updateProject)
        {
            try
            {
                var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (project.Status == false) return ServicesResult<DetailProject>.Failure(project.Message);
                project.Data.Name = updateProject.ProjectName ?? project.Data.Name;
                project.Data.Description = updateProject.ProjectDescription ?? project.Data.Description;
                project.Data.StartDate = updateProject.StartDate ?? project.Data.StartDate;
                project.Data.EndDate = updateProject.EndDate ?? project.Data.EndDate;
                project.Data.StatusId = DateTime.Now == updateProject.StartDate
                    ? 3 // Ongoing
                    : (DateTime.Now < updateProject.StartDate ? 2 : 1);// Upcoming or Overdue

                var update = await _unitOfWork.ProjectRepository.UpdateAsync(project.Data);
                if (!update.Status) return ServicesResult<DetailProject>.Failure($"{update.Message}");
                var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);

                if (!user.Status) return ServicesResult<DetailProject>.Failure(user.Message);
                // create a activity log for project and owner role project
                var activityProject = new ActivityLog()
                {
                    Id = "",
                    ActionDate = DateTime.Now,
                    Action = $"A new project was created by {user.Data.UserName}",
                    ProjectId = project.Data.Id,
                };
                var acvitity = await _unitOfWork.ActivityLogRepository.AddAsync(activityProject);
                if (!acvitity.Status) return ServicesResult<DetailProject>.Failure(acvitity.Message);

                // xác thực thành công


                // tạo kết quả trả về
                var response = await GetDetailProject(userId, projectId);
                if (response.Status == false) return ServicesResult<DetailProject>.Failure(response.Message);
                return ServicesResult<DetailProject>.Success(response.Data);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure(ex.Message);
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Helper Methods

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
        #endregion
        #region Toggle project status of a project
        /// <summary>
        /// Toggles the IsDeleted status of a project if the user has the required permissions.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the update.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <returns>The updated project details or an error message if the operation fails.</returns>
        public async Task<ServicesResult<DetailProject>> UpdateIsDelete(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<DetailProject>.Failure("Invalid input parameters.");

            try
            {
                // Validate user's ownership role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<DetailProject>.Failure(ownRoleResult.Message);

                var projectUser = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (!projectUser.Status || !projectUser.Data.Any())
                    return ServicesResult<DetailProject>.Failure("No projects found for the user.");

                bool isUserAuthorized = projectUser.Data.Any(x => x.RoleId == _ownRoleId && x.ProjectId == projectId);
                if (!isUserAuthorized)
                    return ServicesResult<DetailProject>.Failure("User does not have permission to update this project.");

                // Toggle IsDeleted status
                var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!project.Status) return ServicesResult<DetailProject>.Failure(project.Message);
                project.Data.IsDeleted = !project.Data.IsDeleted;

                var updateResult = await _unitOfWork.ProjectRepository.UpdateAsync(project.Data);
                if (!updateResult.Status) return ServicesResult<DetailProject>.Failure(updateResult.Message);

                // Log the action
                var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!user.Status) return ServicesResult<DetailProject>.Failure(user.Message);

                var activityLog = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    ActionDate = DateTime.Now,
                    Action = $"The project '{project.Data.Name}' was {(project.Data.IsDeleted ? "deleted" : "restored")} by {user.Data.UserName}.",
                    ProjectId = project.Data.Id,
                    UserId = userId
                };

                var logResult = await _unitOfWork.ActivityLogRepository.AddAsync(activityLog);
                if (!logResult.Status) return ServicesResult<DetailProject>.Failure(logResult.Message);

                // Return updated project details
                var response = await GetDetailProject(userId, projectId);
                return response.Status ? ServicesResult<DetailProject>.Success(response.Data) : ServicesResult<DetailProject>.Failure(response.Message);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }

        #endregion
        #region Toggle project status of a project
        /// <summary>
        /// Toggles the IsCompleted status of a project if the user has the required permissions.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the update.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <returns>The updated project details or an error message if the operation fails.</returns>
        public async Task<ServicesResult<DetailProject>> UpdateIsCompleted(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<DetailProject>.Failure("Invalid input parameters.");

            try
            {
                // Validate user's ownership role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<DetailProject>.Failure(ownRoleResult.Message);

                var projectUser = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (!projectUser.Status || !projectUser.Data.Any())
                    return ServicesResult<DetailProject>.Failure("No projects found for the user.");

                bool isUserAuthorized = projectUser.Data.Any(x => x.RoleId == _ownRoleId && x.ProjectId == projectId);
                if (!isUserAuthorized)
                    return ServicesResult<DetailProject>.Failure("User does not have permission to update this project.");

                // Toggle IsCompleted status
                var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!project.Status) return ServicesResult<DetailProject>.Failure(project.Message);
                project.Data.IsCompleted = !project.Data.IsCompleted;

                var updateResult = await _unitOfWork.ProjectRepository.UpdateAsync(project.Data);
                if (!updateResult.Status) return ServicesResult<DetailProject>.Failure(updateResult.Message);

                // Log the action
                var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!user.Status) return ServicesResult<DetailProject>.Failure(user.Message);

                var activityLog = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    ActionDate = DateTime.Now,
                    Action = $"The project '{project.Data.Name}' was marked as {(project.Data.IsCompleted ? "completed" : "incomplete")} by {user.Data.UserName}.",
                    ProjectId = project.Data.Id,
                    UserId = userId
                };

                var logResult = await _unitOfWork.ActivityLogRepository.AddAsync(activityLog);
                if (!logResult.Status) return ServicesResult<DetailProject>.Failure(logResult.Message);

                // Return updated project details
                var response = await GetDetailProject(userId, projectId);
                return response.Status ? ServicesResult<DetailProject>.Success(response.Data) : ServicesResult<DetailProject>.Failure(response.Message);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }
        #endregion

        #region Delete a project along with its accompanying plans, members, and activity logs
        /// <summary>
        /// Deletes a project along with its associated plans, members, and activity logs if the user has sufficient permissions.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to delete the project.</param>
        /// <param name="projectId">The ID of the project to be deleted.</param>
        /// <returns>A result containing the updated list of projects owned by the user or an error message if the deletion fails.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> Delete(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<IEnumerable<IndexProject>>.Failure("Invalid input parameters.");

            try
            {
                // Step 1: Verify user's role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(ownRoleResult.Message);

                // Step 2: Get project members and verify authorization
                var members = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!members.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(members.Message);

                var isUserAuthorized = members.Data.FirstOrDefault(x => x.RoleId == _ownRoleId && x.UserId == userId && x.ProjectId == projectId);
                if (isUserAuthorized == null)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("User does not have permission to delete this project.");

                // Step 3: Retrieve the project
                var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!project.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);

                // Step 4: Delete all associated plans
                var planProjects = await _unitOfWork.PlanRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!planProjects.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(planProjects.Message);

                foreach (var plan in planProjects.Data)
                {
                    var responsePlan = await _planServices.DeleteAsync(isUserAuthorized.Id, plan.Id);
                    if (!responsePlan.Status)
                        return ServicesResult<IEnumerable<IndexProject>>.Failure(responsePlan.Message);
                }

                // Step 5: Delete all associated members
                foreach (var member in members.Data)
                {
                    var responseMember = await _memberServices.DeleteMember(userId, member.Id);
                    if (!responseMember.Status)
                        return ServicesResult<IEnumerable<IndexProject>>.Failure(responseMember.Message);
                }

                // Step 6: Delete all activity logs for the project
                var logs = await _unitOfWork.ActivityLogRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!logs.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(logs.Message);

                foreach (var logEntry in logs.Data)
                {
                    var responseLog = await _unitOfWork.ActivityLogRepository.DeleteAsync(logEntry.Id);
                    if (!responseLog.Status)
                        return ServicesResult<IEnumerable<IndexProject>>.Failure(responseLog.Message);
                }

                // Step 7: Add a deletion activity log
                var deletionLog = new ActivityLog
                {
                    Id = "",
                    ActionDate = DateTime.Now,
                    Action = $"The project {project.Data.Name} was deleted by {isUserAuthorized.UserId}",
                    ProjectId = projectId
                };
                var logResponse = await _unitOfWork.ActivityLogRepository.AddAsync(deletionLog);
                if (!logResponse.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(logResponse.Message);

                // Step 8: Delete the project
                var projectDeletionResponse = await _unitOfWork.ProjectRepository.DeleteAsync(projectId);
                if (!projectDeletionResponse.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(projectDeletionResponse.Message);

                // Step 9: Return the updated project list
                return await GetProjectListUserHasOwner(userId);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
