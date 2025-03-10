using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Models.members;
using PM.Domain.Models.plans;
using PM.Domain.Models.projects;

namespace PM.Persistence.Implements.Services
{
    public class ProjectServices
    {
        #region Constructor
        private readonly IUnitOfWork _unitOfWork;
        public ProjectServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region public method

        #region Get All Active Projects
        /// <summary>
        /// Retrieves a list of all active projects.
        /// </summary>
        /// <returns>A ServicesResult containing a list of projects or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsAsync()
        {
            try
            {
                var response = new List<IndexProject>();

                // Fetch all projects from the repository
                var projectResponse = await _unitOfWork.ProjectRepository.GetAllAsync();
                if (!projectResponse.Status)
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(projectResponse.Message);
                }

                // Return an empty list if there are no projects
                if (projectResponse.Data == null || !projectResponse.Data.Any())
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Success(response);
                }

                // Iterate through projects
                foreach (var project in projectResponse.Data)
                {
                    // Skip deleted projects
                    if (project.IsDeleted)
                    {
                        continue;
                    }

                    // Retrieve project owner information
                    var ownerMember = await GetOwnerInProject(project.Id);
                    if (!ownerMember.Status || ownerMember.Data == null)
                    {
                        return ServicesResult<IEnumerable<IndexProject>>.Failure("Failed to retrieve project owner.");
                    }

                    // Create an IndexProject instance and add to the response list
                    response.Add(new IndexProject
                    {
                        Id = project.Id,
                        Name = project.Name,
                        OwnerName = ownerMember.Data.UserName,
                        OwnerAvata = ownerMember.Data.AvatarPath
                    });
                }

                return ServicesResult<IEnumerable<IndexProject>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure($"An error occurred: {ex.Message}");
            }
        }
        #endregion


        #region Get Projects Where User Is Owner
        /// <summary>
        /// Retrieves a list of projects where the specified user is the owner.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A ServicesResult containing a list of projects or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsOwnerAsync(string userId)
        {
            // Validate input
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("UserId cannot be null or empty.");
            }

            try
            {
                // Retrieve projects where the user has the "Owner" role
                return await GetProjectsOfUserWithRole(userId, "Owner");
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure($"An error occurred: {ex.Message}");
            }
        }
        #endregion


        #region Get Projects Where User Is Leader
        /// <summary>
        /// Retrieves a list of projects where the specified user is a leader.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A ServicesResult containing a list of projects or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsLeaderAsync(string userId)
        {
            // Validate input
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("UserId cannot be null or empty.");
            }

            try
            {
                // Retrieve projects where the user has the "Leader" role
                return await GetProjectsOfUserWithRole(userId, "Leader");
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure($"An error occurred: {ex.Message}");
            }
        }
        #endregion


        #region Get Projects Where User Is Manager
        /// <summary>
        /// Retrieves a list of projects where the specified user is a manager.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A ServicesResult containing a list of projects or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsManagerAsync(string userId)
        {
            // Validate input
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("UserId cannot be null or empty.");
            }

            try
            {
                // Retrieve projects where the user has the "Manager" role
                return await GetProjectsOfUserWithRole(userId, "Manager");
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure($"An error occurred: {ex.Message}");
            }
        }
        #endregion


        #region Get Projects Where User Is Member
        /// <summary>
        /// Retrieves a list of projects where the specified user is a member.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A ServicesResult containing a list of projects or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsMemberAsync(string userId)
        {
            // Validate input
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("UserId cannot be null or empty.");
            }

            try
            {
                // Retrieve projects where the user has the "Member" role
                return await GetProjectsOfUserWithRole(userId, "Member");
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure($"An error occurred: {ex.Message}");
            }
        }
        #endregion


        #region Get Projects User Has Joined
        /// <summary>
        /// Retrieves a list of projects that the specified user has joined.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A ServicesResult containing a list of projects or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserHasJoinedAsync(string userId)
        {
            // Validate input
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("UserId cannot be null or empty.");
            }

            try
            {
                var response = new List<IndexProject>();

                // Get all projects where the user is a member
                var projectJoined = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (!projectJoined.Status)
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(projectJoined.Message);
                }

                if (projectJoined.Data == null)
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Success(response);
                }

                // Iterate through joined projects
                foreach (var item in projectJoined.Data)
                {
                    var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", item.ProjectId);
                    if (!project.Status)
                    {
                        return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);
                    }

                    if (project.Data == null || project.Data.IsDeleted)
                    {
                        continue; // Skip if the project is deleted or not found
                    }

                    var ownerMember = await GetOwnerInProject(item.ProjectId);
                    if (!ownerMember.Status || ownerMember.Data == null)
                    {
                        return ServicesResult<IEnumerable<IndexProject>>.Failure("Failed to retrieve project owner.");
                    }

                    var index = new IndexProject
                    {
                        Id = item.ProjectId,
                        Name = project.Data.Name,
                        OwnerName = ownerMember.Data.UserName,
                        OwnerAvata = ownerMember.Data.AvatarPath
                    };
                    response.Add(index);
                }

                return ServicesResult<IEnumerable<IndexProject>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure($"An error occurred: {ex.Message}");
            }
        }
        #endregion

        #region Get Project Details
        /// <summary>
        /// Retrieves detailed information about a specific project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project.</param>
        /// <returns>A ServicesResult containing detailed project information or an error message.</returns>
        public async Task<ServicesResult<DetailProject>> GetDetailProjectAsync(string projectId)
        {
            // Validate input
            if (string.IsNullOrEmpty(projectId))
            {
                return ServicesResult<DetailProject>.Failure("ProjectId cannot be null or empty.");
            }

            try
            {
                // Fetch project data
                var projectResponse = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!projectResponse.Status || projectResponse.Data == null)
                {
                    return ServicesResult<DetailProject>.Failure("Project not found.");
                }

                // Fetch related data in parallel to optimize performance
                var memberTask = GetMembersInProject(projectId);
                var ownerTask = GetOwnerInProject(projectId);
                var statusTask = GetStatusAsync(projectResponse.Data.StatusId);
                var plansTask = GetPlansInProjectAsync(projectId);

                await Task.WhenAll(memberTask, ownerTask, statusTask, plansTask);

                var memberResponse = memberTask.Result;
                var ownerResponse = ownerTask.Result;
                var statusResponse = statusTask.Result;
                var plansResponse = plansTask.Result;

                // Validate responses
                if (!ownerResponse.Status || ownerResponse.Data == null)
                {
                    return ServicesResult<DetailProject>.Failure("Failed to retrieve project owner.");
                }

                if (!statusResponse.Status || statusResponse.Data == null)
                {
                    return ServicesResult<DetailProject>.Failure("Failed to retrieve project status.");
                }

                // Construct project details
                var projectDetail = new DetailProject
                {
                    Id = projectResponse.Data.Id,
                    Name = projectResponse.Data.Name,
                    OwnerName = ownerResponse.Data.UserName,
                    OwnerAvata = ownerResponse.Data.AvatarPath,
                    IsCompleted = projectResponse.Data.IsCompleted,
                    IsDeleted = projectResponse.Data.IsDeleted,
                    StartAt = projectResponse.Data.StartDate,
                    EndAt = projectResponse.Data.EndDate,
                    CreateAt = projectResponse.Data.CreatedDate,
                    ProjectDescription = projectResponse.Data.Description,
                    QuantityMember = memberResponse.Data?.Count() ?? 0,
                    Members = new List<IndexMember>(),
                    Plans = new List<IndexPlan>(),
                    Status = statusResponse.Data.Name
                };

                // Populate members list
                if (memberResponse.Data != null)
                {
                    foreach (var member in memberResponse.Data)
                    {
                        var infoUser = await GetInfoUser(member.UserId);
                        if (infoUser.Status && infoUser.Data != null)
                        {
                            projectDetail.Members.Add(new IndexMember
                            {
                                MemberId = member.Id,
                                PositionWorkName = member.PositionWork,
                                UserName = infoUser.Data.UserName,
                                UserAvata = infoUser.Data.AvatarPath
                            });
                        }
                    }
                }

                // Populate plans list
                if (plansResponse.Data != null)
                {
                    projectDetail.Plans = plansResponse.Data.Select(plan => new IndexPlan
                    {
                        PlanId = plan.Id,
                        PlanName = plan.Name,
                        Description = plan.Description
                    }).ToList();
                }

                return ServicesResult<DetailProject>.Success(projectDetail);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure($"An error occurred while retrieving project details: {ex.Message}");
            }
        }
        #endregion


        #region Create Project
        /// <summary>
        /// Creates a new project and assigns the user as the owner.
        /// </summary>
        /// <param name="userId">The ID of the user creating the project.</param>
        /// <param name="addProject">The project details provided by the user.</param>
        /// <returns>A boolean result indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> CreateProjectAsync(string userId, AddProject addProject)
        {
            if (string.IsNullOrEmpty(userId) || addProject == null)
            {
                return ServicesResult<bool>.Failure("UserId or project cannot be null or empty.");
            }
            try
            {
                var ownerRoleResult = await GetRoleId(RoleType.Owner);
                if (!ownerRoleResult.Status)
                {
                    return ServicesResult<bool>.Failure(ownerRoleResult.Message);
                }
                var ownerRoleId = ownerRoleResult.Data;

                var projectJoined = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (!projectJoined.Status)
                {
                    return ServicesResult<bool>.Failure(projectJoined.Message);
                }

                var projectUserAreOwner = projectJoined.Data.Where(x => x.RoleId == ownerRoleId).ToList();

                var projects = new List<Project>();

                foreach (var projectIndex in projectUserAreOwner)
                {
                    var projectResponse = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectIndex.ProjectId);
                    if (!projectResponse.Status)
                    {
                        return ServicesResult<bool>.Failure(projectResponse.Message);
                    }
                    if (projectResponse.Data.IsDeleted)
                    {
                        continue;
                    }
                    projects.Add(projectResponse.Data);
                }

                var project = new Project()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = addProject.ProjectName,
                    CreatedDate = DateTime.Now,
                    StartDate = new DateTime(addProject.StartAt.Year, addProject.StartAt.Month, addProject.StartAt.Day),
                    EndDate = new DateTime(addProject.EndAt.Year, addProject.EndAt.Month, addProject.EndAt.Day),
                    Description = addProject.ProjectDescription,
                    IsCompleted = false,
                    IsDeleted = false,
                };
                project.StatusId = DateTime.Now == project.StartDate
                   ? 3 // Ongoing
                   : (DateTime.Now < project.EndDate ? 2 : 1); // Upcoming or Overdue

                var addResponse = await _unitOfWork.ProjectRepository.AddAsync(projects, project);
                if (!addResponse.Status)
                {
                    return ServicesResult<bool>.Failure(addResponse.Message);
                }
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure($"An error occurred while creating project: {ex.Message}");
            }
        }
        #endregion

        #region Update Project
        /// <summary>
        /// Updates an existing project if the user is the owner.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to update the project.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <param name="updateProject">The updated project details.</param>
        /// <returns>A boolean result indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> UpdateProjectAsync(string userId, string projectId, UpdateProject updateProject)
        {
            // Validate input
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId) || updateProject == null)
            {
                return ServicesResult<bool>.Failure("UserId, projectId, or project details cannot be null or empty.");
            }

            try
            {
                // Retrieve the user's owner role
                var ownerRoleResult = await GetRoleId(RoleType.Owner);
                if (!ownerRoleResult.Status)
                {
                    return ServicesResult<bool>.Failure(ownerRoleResult.Message);
                }
                var ownerRoleId = ownerRoleResult.Data;

                

                // Fetch projects the user is part of
                var projectJoined = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (!projectJoined.Status || projectJoined.Data == null)
                {
                    return ServicesResult<bool>.Failure(projectJoined.Message ?? "Failed to retrieve user's projects.");
                }

                // Get projects where the user is the owner
                var userOwnedProjectIds = projectJoined.Data
                    .Where(x => x.RoleId == ownerRoleId)
                    .Select(x => x.ProjectId)
                    .ToList();

                var activeProjects = new List<Project>();
                foreach (var projectIdOwned in userOwnedProjectIds)
                {
                    var projectResponse = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectIdOwned);
                    if (projectResponse.Status && projectResponse.Data != null && !projectResponse.Data.IsDeleted)
                    {
                        activeProjects.Add(projectResponse.Data);
                    }
                }

                // Find the project to update
                var project = activeProjects.FirstOrDefault(x => x.Id == projectId);
                if (project == null)
                {
                    return ServicesResult<bool>.Failure("Project not found or user is not the owner.");
                }

                // Validate project start and end dates
                if (updateProject.StartDate > updateProject.EndDate)
                {
                    return ServicesResult<bool>.Failure("Project start date cannot be later than the end date.");
                }

                // Update project details
                project.Name = updateProject.ProjectName;
                project.Description = updateProject.ProjectDescription;
                project.StartDate = new DateTime(updateProject.StartDate.Year, updateProject.StartDate.Month, updateProject.StartDate.Day);
                project.EndDate = new DateTime(updateProject.EndDate.Year, updateProject.EndDate.Month, updateProject.EndDate.Day);
                project.StatusId = DateTime.Now == project.StartDate
                    ? 3 // Ongoing
                    : (DateTime.Now < project.EndDate ? 2 : 1); // Upcoming or Overdue
                                                                // Save updated project
                var updateResponse = await _unitOfWork.ProjectRepository.UpdateAsync(activeProjects, project);
                if (!updateResponse.Status)
                {
                    return ServicesResult<bool>.Failure(updateResponse.Message);
                }

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure($"An error occurred while updating the project: {ex.Message}");
            }
        }
        #endregion

        #region Delete Project
        /// <summary>
        /// Deletes a project if the user is the owner.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to delete the project.</param>
        /// <param name="projectId">The ID of the project to be deleted.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> DeleteProjectAsync(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
            {
                return ServicesResult<bool>.Failure("UserId or projectId cannot be null or empty.");
            }
            try
            {
                // Get the role ID for the "Owner" role
                var ownerRoleResult = await GetRoleId(RoleType.Owner);
                if (!ownerRoleResult.Status)
                {
                    return ServicesResult<bool>.Failure(ownerRoleResult.Message);
                }
                var ownerRoleId = ownerRoleResult.Data;

                // Check if the user is the owner of the project
                var projectMember = await _unitOfWork.ProjectMemberRepository
                    .GetOneByKeyAndValue("ProjectId", projectId, "RoleId", ownerRoleId, "UserId", userId);

                if (!projectMember.Status || projectMember.Data == null)
                {
                    return ServicesResult<bool>.Failure("User is not the owner of the project.");
                }

                // Proceed with project deletion
                var deleteResponse = await _unitOfWork.ProjectRepository.DeleteAsync(projectMember.Data.ProjectId);
                if (!deleteResponse.Status)
                {
                    return ServicesResult<bool>.Failure(deleteResponse.Message);
                }

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure($"An error occurred while deleting the project: {ex.Message}");
            }
        }
        #endregion

        #region Update Project Deletion Status
        /// <summary>
        /// Toggles the IsDeleted status of a project if the user is the owner.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>Result indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> UpdateIsDeleteAsync(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
            {
                return ServicesResult<bool>.Failure("UserId or projectId cannot be null or empty.");
            }

            return await UpdateProjectAsync(userId, projectId, async project =>
            {
                // Fetch latest project state to avoid stale data
                var latestProject = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!latestProject.Status || latestProject.Data == null)
                {
                    throw new Exception("Project not found or cannot be retrieved.");
                }

                // Toggle IsDeleted safely
                project.IsDeleted = !latestProject.Data.IsDeleted;
            });
        }
        #endregion

        #region Update Project Completion Status
        /// <summary>
        /// Toggles the IsCompleted status of a project if the user is the owner.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>Result indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> UpdateIsCompletedAsync(string userId, string projectId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ServicesResult<bool>.Failure("User ID cannot be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return ServicesResult<bool>.Failure("Project ID cannot be null or empty.");
            }

            return await UpdateProjectAsync(userId, projectId, async project =>
            {
                // Ensure we work with the latest state
                var latestProject = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!latestProject.Status || latestProject.Data == null)
                {
                    throw new Exception("Project not found or cannot be retrieved.");
                }

                // Toggle IsCompleted safely
                project.IsCompleted = !latestProject.Data.IsCompleted;
            });
        }
        #endregion


        #endregion

        #region  private method

        #region Retrieves the members of a specific project.
        /// <summary>
        /// Retrieves the members of a specific project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project.</param>
        /// <returns>A ServicesResult containing the list of project members or an error message.</returns>
        private async Task<ServicesResult<IEnumerable<ProjectMember>>> GetMembersInProject(string projectId)
        {
            // Validate input
            if (string.IsNullOrEmpty(projectId))
            {
                return ServicesResult<IEnumerable<ProjectMember>>.Failure("ProjectId cannot be null or empty.");
            }

            try
            {
                // Fetch project members based on the given projectId
                var membersResponse = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);

                // Check if the repository call was unsuccessful
                if (!membersResponse.Status)
                {
                    return ServicesResult<IEnumerable<ProjectMember>>.Failure(membersResponse.Message);
                }

                // Ensure the response data is not null
                if (membersResponse.Data == null)
                {
                    return ServicesResult<IEnumerable<ProjectMember>>.Failure("No members found for the specified project.");
                }

                // Return the retrieved project members
                return ServicesResult<IEnumerable<ProjectMember>>.Success(membersResponse.Data);
            }
            catch (Exception ex)
            {
                // Handle any unexpected exceptions
                return ServicesResult<IEnumerable<ProjectMember>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        #endregion

        #region Get Project Owner
        /// <summary>
        /// Retrieves the owner of a specific project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project.</param>
        /// <returns>A ServicesResult containing the owner user or an error message.</returns>
        private async Task<ServicesResult<User>> GetOwnerInProject(string projectId)
        {
            // Validate input
            if (string.IsNullOrEmpty(projectId))
            {
                return ServicesResult<User>.Failure("ProjectId cannot be null or empty.");
            }

            try
            {
                // Retrieve the current role of the user
                var ownerRoleResult = await GetRoleId(RoleType.Owner);
                if (!ownerRoleResult.Status)
                {
                    return ServicesResult<User>.Failure(ownerRoleResult.Message);
                }
                var ownerRoleId = ownerRoleResult.Data;


                // Fetch all members of the project based on the projectId
                var membersResponse = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);

                // Check if the repository call was unsuccessful
                if (!membersResponse.Status)
                {
                    return ServicesResult<User>.Failure(membersResponse.Message);
                }

                // Ensure the response data is not null
                if (membersResponse.Data == null)
                {
                    return ServicesResult<User>.Failure("No members found for the specified project.");
                }

                // Find the owner of the project based on the current role
                var ownerMember = membersResponse.Data.FirstOrDefault(x => x.RoleId == ownerRoleId);
                if (ownerMember == null)
                {
                    return ServicesResult<User>.Failure("Project owner not found.");
                }

                // Retrieve and return the user information of the project owner
                return await GetInfoUser(ownerMember.UserId);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return ServicesResult<User>.Failure($"An error occurred: {ex.Message}");
            }
        }
        #endregion

        #region Get User Information
        /// <summary>
        /// Retrieves user information based on the given user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A ServicesResult containing the user data or an error message.</returns>
        private async Task<ServicesResult<User>> GetInfoUser(string userId)
        {
            // Validate input
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<User>.Failure("UserId cannot be null or empty.");
            }

            try
            {
                // Fetch user details from the repository based on userId
                var userResponse = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);

                // Check if the repository call was unsuccessful
                if (!userResponse.Status)
                {
                    return ServicesResult<User>.Failure(userResponse.Message);
                }

                // Return the retrieved user information
                return ServicesResult<User>.Success(userResponse.Data);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return ServicesResult<User>.Failure($"An error occurred: {ex.Message}");
            }
        }
        #endregion

        #region Get User Projects by Role
        /// <summary>
        /// Retrieves a list of projects that a user is involved in based on their role.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="roleName">The role of the user in the projects (Owner, Leader, Manager, Member).</param>
        /// <returns>A ServicesResult containing a list of projects or an error message.</returns>
        private async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsOfUserWithRole(string userId, string roleName)
        {
            // Validate input
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("UserId cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(roleName))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("RoleName cannot be null or empty.");
            }

            // Assign the current role based on roleName
            

            try
            {
                var roleResult = await GetRoleIdByName(roleName);
                if (!roleResult.Status)
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(roleResult.Message);
                }

                var roleId = roleResult.Data;

                var response = new List<IndexProject>();

                // Fetch the projects that the user has joined
                var projectJoined = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (!projectJoined.Status)
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(projectJoined.Message);
                }
                if (projectJoined.Data == null || !projectJoined.Data.Any())
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Success(response); // Return empty list
                }

                // Iterate through the projects the user has joined
                foreach (var member in projectJoined.Data)
                {
                    // Skip projects where the role does not match
                    if (member.RoleId != roleId) continue;

                    // Retrieve project details
                    var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", member.ProjectId);
                    if (!project.Status)
                    {
                        return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);
                    }
                    if (project.Data == null || project.Data.IsDeleted)
                    {
                        continue; // Skip if project does not exist or is deleted
                    }

                    // Retrieve project owner information
                    var ownerMember = await GetOwnerInProject(member.ProjectId);
                    if (!ownerMember.Status || ownerMember.Data == null)
                    {
                        return ServicesResult<IEnumerable<IndexProject>>.Failure("Failed to retrieve project owner.");
                    }

                    // Create an IndexProject instance and add to the response list
                    response.Add(new IndexProject
                    {
                        Id = member.ProjectId,
                        Name = project.Data.Name,
                        OwnerName = ownerMember.Data.UserName,
                        OwnerAvata = ownerMember.Data.AvatarPath
                    });
                }

                return ServicesResult<IEnumerable<IndexProject>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure($"An error occurred: {ex.Message}");
            }
        }
        #endregion

        #region Private role method helper

        /// <summary>
        /// Gets the role ID by role name.
        /// </summary>
        /// <param name="roleName">The name of the role to fetch.</param>
        /// <returns>Role ID if found, otherwise null.</returns>
        private async Task<ServicesResult<string>> GetRoleIdByName(string roleName)
        {
            try
            {
                var role = await _unitOfWork.RoleInProjectRepository.GetOneByKeyAndValue("Name", roleName);
                if (!role.Status || role.Data == null)
                {
                    return ServicesResult<string>.Failure($"Role '{roleName}' not found.");
                }

                return ServicesResult<string>.Success(role.Data.Id);
            }
            catch (Exception ex)
            {
                return ServicesResult<string>.Failure($"An error occurred while fetching role '{roleName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the role ID for a given role type.
        /// </summary>
        /// <param name="roleType">The role type (Owner, Leader, Manager, Member).</param>
        /// <returns>Role ID if found, otherwise null.</returns>
        private Task<ServicesResult<string>> GetRoleId(RoleType roleType)
        {
            return GetRoleIdByName(roleType.ToString());
        }

        /// <summary>
        /// Enum for role types.
        /// </summary>
        private enum RoleType
        {
            Owner,
            Leader,
            Manager,
            Member
        }

        #endregion


        #region Get Status by ID
        /// <summary>
        /// Retrieves a status entity based on the given status ID.
        /// </summary>
        /// <param name="statusId">The unique identifier of the status.</param>
        /// <returns>A ServicesResult containing the status entity or an error message.</returns>
        private async Task<ServicesResult<Status>> GetStatusAsync(int statusId)
        {
            try
            {
                // Fetch status from the repository
                var statusResponse = await _unitOfWork.StatusRepository.GetOneByKeyAndValue("Id", statusId);

                // Check if retrieval was unsuccessful
                if (!statusResponse.Status)
                {
                    return ServicesResult<Status>.Failure(statusResponse.Message);
                }

                // Return success with the retrieved status
                return ServicesResult<Status>.Success(statusResponse.Data);
            }
            catch (Exception ex)
            {
                return ServicesResult<Status>.Failure($"An error occurred while fetching status: {ex.Message}");
            }
        }
        #endregion

        #region Get Plans in Project
        /// <summary>
        /// Retrieves all plans associated with a specific project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project.</param>
        /// <returns>A ServicesResult containing a list of plans or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<Plan>>> GetPlansInProjectAsync(string projectId)
        {
            // Validate input
            if (string.IsNullOrEmpty(projectId))
            {
                return ServicesResult<IEnumerable<Plan>>.Failure("ProjectId cannot be null or empty.");
            }

            try
            {
                // Fetch plans associated with the project
                var projectResponse = await _unitOfWork.PlanRepository.GetManyByKeyAndValue("ProjectId", projectId);

                // Check if retrieval was unsuccessful
                if (!projectResponse.Status)
                {
                    return ServicesResult<IEnumerable<Plan>>.Failure(projectResponse.Message);
                }

                // If no plans exist, return an empty success response
                var plans = projectResponse.Data ?? Enumerable.Empty<Plan>();
                return ServicesResult<IEnumerable<Plan>>.Success(plans);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<Plan>>.Failure($"An error occurred while fetching plans: {ex.Message}");
            }
        }
        #endregion

        #region Update Project
        /// <summary>
        /// Updates a project if the user is the owner.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to update the project.</param>
        /// <param name="projectId">The ID of the project to be updated.</param>
        /// <param name="updateProject">A function to update the project details.</param>
        /// <returns>A boolean indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> UpdateProjectAsync(string userId, string projectId, Func<Project, Task> updateProject)
        {
            // Validate input
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
            {
                return ServicesResult<bool>.Failure("UserId or projectId cannot be null or empty.");
            }

            try
            {
                var ownerRoleResult = await GetRoleId(RoleType.Owner);
                if (!ownerRoleResult.Status)
                {
                    return ServicesResult<bool>.Failure(ownerRoleResult.Message);
                }
                var ownerRoleId = ownerRoleResult.Data;

                // Check if the user is the owner of the project
                var projectMember = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("ProjectId", projectId, "RoleId", ownerRoleId, "UserId", userId);

                if (!projectMember.Status || projectMember.Data == null)
                {
                    return ServicesResult<bool>.Failure("User is not the owner of the project.");
                }

                // Retrieve the project
                var projectResponse = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!projectResponse.Status || projectResponse.Data == null)
                {
                    return ServicesResult<bool>.Failure("Project not found.");
                }

                var project = projectResponse.Data;

                // Apply update logic
                await updateProject(project);

                // Update project status dynamically based on the dates
                project.StatusId = DateTime.Now == project.StartDate
                   ? 3 // Ongoing
                   : (DateTime.Now < project.EndDate ? 2 : 1); // Upcoming or Overdue

                // Save changes
                var updateResponse = await _unitOfWork.ProjectRepository.UpdateAsync(project);
                if (!updateResponse.Status)
                {
                    return ServicesResult<bool>.Failure(updateResponse.Message);
                }

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure($"An error occurred while updating the project: {ex.Message}");
            }
        }
        #endregion


        #endregion
    }
}
