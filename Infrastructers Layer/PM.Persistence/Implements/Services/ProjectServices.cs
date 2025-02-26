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
        public async Task<ServicesResult<DetailProject>> GetDetailProject(string projectId)
        {
            if ( string.IsNullOrEmpty(projectId))
                return ServicesResult<DetailProject>.Failure("User ID or Project ID cannot be null or empty.");

            try
            {
                // Verify the current user's role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<DetailProject>.Failure(ownRoleResult.Message);

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
                        MemberId = memberInfoResult.Data.Id,
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
        }

        /// <summary>
        /// Supports adding a new project.
        /// </summary>
        private async Task<ServicesResult<DetailProject>> AddMethodSupport(string memberId, AddProject addProject)
        {
            try
            {
                // add new project
                var project = new Project()
                {
                    Id = $"{Guid.NewGuid()}",
                    Name = addProject.ProjectName,
                    StartDate = new DateTime(addProject.StartAt.Year, addProject.StartAt.Month, addProject.StartAt.Day),
                    EndDate = new DateTime(addProject.EndAt.Year, addProject.EndAt.Month, addProject.EndAt.Day),
                    CreatedDate = DateTime.Now,
                    IsCompleted = false,
                    IsDeleted = false,
                    
                };
                project.StatusId = DateTime.Now == project.StartDate
                    ? 3 // Ongoing
                    : (DateTime.Now < project.EndDate ? 2 : 1); // Upcoming or Overdue
                var responseProject = await _unitOfWork.ProjectRepository.AddAsync(project);
                if (!responseProject.Status) return ServicesResult<DetailProject>.Failure(responseProject.Message);

                var memberInfo = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (memberInfo.Status == false)
                    return ServicesResult<DetailProject>.Failure(memberInfo.Message);
                // set up owner role 
                var member = new ProjectMember()
                {
                    Id = $"{Guid.NewGuid()}",
                    ProjectId = project.Id,
                    RoleId = _ownRoleId,
                    UserId = memberInfo.Data.UserId,
                    PositionWork = string.Empty
                };
                var projectMemberResponse = await _unitOfWork.ProjectMemberRepository.AddAsync(member);
                if (!projectMemberResponse.Status) return ServicesResult<DetailProject>.Failure(projectMemberResponse.Message);

                var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", memberInfo.Data.UserId);
                if (!user.Status) return ServicesResult<DetailProject>.Failure(user.Message);

                // create a activity log for project and owner role project
                var activityProject = new ActivityLog()
                {
                    Id = $"{Guid.NewGuid()}",
                    ActionDate = DateTime.Now,
                    Action = $"A new project was created by {user.Data.UserName}",
                    ProjectId = project.Id,
                };
                var acvitity = await _unitOfWork.ActivityLogRepository.AddAsync(activityProject);
                if (!acvitity.Status) return ServicesResult<DetailProject>.Failure(acvitity.Message);

                // xác thực thành công

                var response = await GetDetailProject( project.Id);
                if (response.Status == false) return ServicesResult<DetailProject>.Failure(response.Message);
                return ServicesResult<DetailProject>.Success(response.Data);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure(ex.Message);
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
        public async Task<ServicesResult<DetailProject>> UpdateInfo(string MemberCurrentId, string projectId, UpdateProject updateProject)
        {
            if (string.IsNullOrEmpty(MemberCurrentId) || string.IsNullOrEmpty(projectId) || updateProject == null)
                return ServicesResult<DetailProject>.Failure("Invalid input");

            try
            {
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<DetailProject>.Failure(ownRoleResult.Message);

                var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", MemberCurrentId);
                if(!member.Status)
                    return ServicesResult<DetailProject>.Failure(member.Message);

                if (member.Data.RoleId != _ownRoleId)
                    return ServicesResult<DetailProject>.Failure("");
                var userInfo = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.Data.UserId);
                if(userInfo.Status == false)
                    return ServicesResult<DetailProject>.Failure(userInfo.Message);
               return await UpdateMethodSupport(userInfo.Data.Id, projectId, updateProject);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure(ex.Message);
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
                project.Data.Name = updateProject.ProjectName is null ? project.Data.Name : updateProject.ProjectName;
                project.Data.Description = updateProject.ProjectDescription is null ? project.Data.Description : updateProject.ProjectDescription;
                project.Data.StartDate = new DateTime(updateProject.StartDate.Year, updateProject.StartDate.Month, updateProject.StartDate.Day);
                project.Data.EndDate = new DateTime(updateProject.EndDate.Year, updateProject.EndDate.Month, updateProject.EndDate.Day);
                project.Data.StatusId = DateTime.Now == new DateTime(updateProject.StartDate.Year, updateProject.StartDate.Month, updateProject.StartDate.Day)
                    ? 3 // Ongoing
                    : (DateTime.Now < new DateTime(updateProject.StartDate.Year, updateProject.StartDate.Month, updateProject.StartDate.Day) ? 2 : 1);// Upcoming or Overdue

                var update = await _unitOfWork.ProjectRepository.UpdateAsync(project.Data);
                if (!update.Status) return ServicesResult<DetailProject>.Failure($"{update.Message}");
                var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);

                if (!user.Status) return ServicesResult<DetailProject>.Failure(user.Message);
                // create a activity log for project and owner role project
                var activityProject = new ActivityLog()
                {
                    Id = $"{Guid.NewGuid()}",
                    ActionDate = DateTime.Now,
                    Action = $"{user.Data.UserName} updated project {project.Data.Name} ",
                    ProjectId = project.Data.Id,
                    UserId = userId
                };
                var acvitity = await _unitOfWork.ActivityLogRepository.AddAsync(activityProject);
                if (!acvitity.Status) return ServicesResult<DetailProject>.Failure(acvitity.Message);

                // xác thực thành công


                // tạo kết quả trả về
                var response = await GetDetailProject( projectId);
                if (response.Status == false) return ServicesResult<DetailProject>.Failure(response.Message);
                return ServicesResult<DetailProject>.Success(response.Data);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure(ex.Message);
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
        /// Toggles the deletion status of a project (IsDeleted) and logs the action.
        /// </summary>
        /// <param name="memberCurrentId">The ID of the member making the request.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <returns>A service result containing the updated project details or an error message.</returns>
        public async Task<ServicesResult<DetailProject>> UpdateIsDelete(string memberCurrentId, string projectId)
        {
            if (string.IsNullOrEmpty(memberCurrentId))
                return ServicesResult<DetailProject>.Failure("Member ID cannot be null or empty.");

            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<DetailProject>.Failure("Project ID cannot be null or empty.");

            try
            {
                // Validate user's ownership role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to validate role: {ownRoleResult.Message}");

                // Fetch the member data
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberCurrentId);
                if (!memberResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to retrieve member: {memberResult.Message}");

                var member = memberResult.Data;

                // Check if the member has the required ownership role and is associated with the project
                if (member.RoleId != _ownRoleId || member.ProjectId != projectId)
                    return ServicesResult<DetailProject>.Failure("User does not have permission to update this project.");

                // Fetch the project data
                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!projectResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to retrieve project: {projectResult.Message}");

                var project = projectResult.Data;

                // Toggle the IsDeleted status
                project.IsDeleted = !project.IsDeleted;

                // Update the project status in the database
                var updateResult = await _unitOfWork.ProjectRepository.UpdateAsync(project);
                if (!updateResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to update project: {updateResult.Message}");

                // Log the action
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.UserId);
                if (!userResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to retrieve user information: {userResult.Message}");

                var user = userResult.Data;

                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    ActionDate = DateTime.Now,
                    Action = $"The project '{project.Name}' was {(project.IsDeleted ? "deleted" : "restored")} by {user.UserName}.",
                    ProjectId = project.Id,
                    UserId = member.UserId,
                };

                var logResult = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!logResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to create activity log: {logResult.Message}");

                // Return the updated project details
                var detailProjectResult = await GetDetailProject(projectId);
                if (!detailProjectResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to retrieve updated project details: {detailProjectResult.Message}");

                return ServicesResult<DetailProject>.Success(detailProjectResult.Data);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
        #endregion

        #region Toggle project status of a project
        /// <summary>
        /// Toggles the completion status (IsCompleted) of a project and logs the action.
        /// </summary>
        /// <param name="memberCurrentId">The ID of the member making the request.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <returns>A service result containing the updated project details or an error message.</returns>
        public async Task<ServicesResult<DetailProject>> UpdateIsCompleted(string memberCurrentId, string projectId)
        {
            if (string.IsNullOrEmpty(memberCurrentId))
                return ServicesResult<DetailProject>.Failure("Member ID cannot be null or empty.");

            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<DetailProject>.Failure("Project ID cannot be null or empty.");

            try
            {
                // Validate user's ownership role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to validate role: {ownRoleResult.Message}");

                // Fetch the member data
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberCurrentId);
                if (!memberResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to retrieve member: {memberResult.Message}");

                var member = memberResult.Data;

                // Check if the member has the required ownership role and is associated with the project
                if (member.RoleId != _ownRoleId || member.ProjectId != projectId)
                    return ServicesResult<DetailProject>.Failure("User does not have permission to update this project.");

                // Fetch the project data
                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!projectResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to retrieve project: {projectResult.Message}");

                var project = projectResult.Data;

                // Toggle the IsCompleted status
                project.IsCompleted = !project.IsCompleted;

                // Update the project status in the database
                var updateResult = await _unitOfWork.ProjectRepository.UpdateAsync(project);
                if (!updateResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to update project: {updateResult.Message}");

                // Log the action
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.UserId);
                if (!userResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to retrieve user information: {userResult.Message}");

                var user = userResult.Data;

                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    ActionDate = DateTime.Now,
                    Action = $"The project '{project.Name}' was marked as {(project.IsCompleted ? "completed" : "incomplete")} by {user.UserName}.",
                    ProjectId = project.Id,
                    UserId = member.UserId,
                };

                var logResult = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!logResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to create activity log: {logResult.Message}");

                // Return the updated project details
                var detailProjectResult = await GetDetailProject(projectId);
                if (!detailProjectResult.Status)
                    return ServicesResult<DetailProject>.Failure($"Failed to retrieve updated project details: {detailProjectResult.Message}");

                return ServicesResult<DetailProject>.Success(detailProjectResult.Data);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
        #endregion

        #region Deletes a project and its associated plans, members, and logs if the user has ownership permissions.
        /// <summary>
        /// Deletes a project and its associated plans, members, and logs if the user has ownership permissions.
        /// </summary>
        /// <param name="memberId">The ID of the member initiating the deletion.</param>
        /// <param name="projectId">The ID of the project to be deleted.</param>
        /// <returns>A service result containing the list of projects or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> Delete(string memberId, string projectId)
        {
            if (string.IsNullOrEmpty(memberId))
                return ServicesResult<IEnumerable<IndexProject>>.Failure("Member ID cannot be null or empty.");

            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<IEnumerable<IndexProject>>.Failure("Project ID cannot be null or empty.");

            try
            {
                // Validate user ownership role
                var ownRoleResult = await GetOwnRole();
                if (!ownRoleResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure($"Failed to validate role: {ownRoleResult.Message}");

                // Fetch member data
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!memberResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure($"Failed to retrieve member: {memberResult.Message}");

                var member = memberResult.Data;

                // Fetch project data
                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!projectResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure($"Failed to retrieve project: {projectResult.Message}");

                // Check if the member has ownership and is associated with the project
                if (member.RoleId != _ownRoleId || member.ProjectId != projectId)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure("User does not have permission to delete this project.");

                // Delete all associated plans
                var plansResult = await _unitOfWork.PlanRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!plansResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure($"Failed to retrieve plans: {plansResult.Message}");

                foreach (var plan in plansResult.Data)
                {
                    var deletePlanResponse = await _planServices.DeleteAsync(memberId, plan.Id);
                    if (!deletePlanResponse.Status)
                        return ServicesResult<IEnumerable<IndexProject>>.Failure($"Failed to delete plan '{plan.Name}': {deletePlanResponse.Message}");
                }

                // Delete all project members
                var membersResult = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!membersResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure($"Failed to retrieve project members: {membersResult.Message}");

                foreach (var memberToDelete in membersResult.Data)
                {
                    var deleteMemberResponse = await _memberServices.DeleteMember(memberId, memberToDelete.Id);
                    if (!deleteMemberResponse.Status)
                        return ServicesResult<IEnumerable<IndexProject>>.Failure($"Failed to delete project member: {deleteMemberResponse.Message}");
                }

                // Delete all activity logs for the project
                var logsResult = await _unitOfWork.ActivityLogRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!logsResult.Status)
                    return ServicesResult<IEnumerable<IndexProject>>.Failure($"Failed to retrieve logs: {logsResult.Message}");

                foreach (var log in logsResult.Data)
                {
                    var deleteLogResponse = await _unitOfWork.ActivityLogRepository.DeleteAsync(log.Id);
                    if (!deleteLogResponse.Status)
                        return ServicesResult<IEnumerable<IndexProject>>.Failure($"Failed to delete log: {deleteLogResponse.Message}");
                }

                // Return the updated project list
                return await GetProjectListUserHasOwner(projectId);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        #endregion
    }
}
