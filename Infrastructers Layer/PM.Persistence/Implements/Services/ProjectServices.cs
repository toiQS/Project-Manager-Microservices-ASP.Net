using Azure.Core;
using Microsoft.IdentityModel.Tokens;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Models.members;
using PM.Domain.Models.plans;
using PM.Domain.Models.projects;
using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;

namespace PM.Persistence.Implements.Services
{
    public class ProjectServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private string _ownRoleId;
        private string? _projectId = "";
        #region Get projects user has joined

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
                        OwnerName = userResult.Data.NickName,
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

        #region Get projects user has owner

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
                        OwnerName = userResult.Data.NickName,
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

        #region Detail Project

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
                    OwnerName = ownerInfoResult.Data.NickName,
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
                        UserName = memberInfoResult.Data.NickName,
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

        #region add new project, setup owner and create a acvitity log

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

        #endregion

        #region Update Project

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
                    return await UpdateMethodSupport(userId, projectId, updateProject);

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
                    Action = $"A new project was created by {user.Data.NickName}",
                    ProjectId = project.Id,
                };
                var acvitity = await _unitOfWork.ActivityLogRepository.AddAsync(activityProject);
                if (!acvitity.Status) return ServicesResult<DetailProject>.Failure(acvitity.Message);

                // xác thực thành công

                var indexMember = new IndexMember()
                {
                    PositionWorkName = member.PositionWork,
                    UserName = user.Data.NickName,
                    RoleUserInProjectId = member.Id,
                    UserAvata = user.Data.AvatarPath
                };

                // tạo kết quả trả về
                var response = new DetailProject()
                {
                    ProjectId = project.Id,
                    OwnerName = user.Data.NickName,
                    OwnerAvata = user.Data.AvatarPath,
                    CreateAt = project.CreatedDate,
                    EndAt = project.EndDate,
                    ProjectName = project.Name,
                    IsCompleted = project.IsCompleted,
                    IsDeleted = project.IsDeleted,
                };
                response.Members.Add(indexMember);

                return ServicesResult<DetailProject>.Success(response);
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
                var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id",projectId);
                if (project.Status == false) return ServicesResult<DetailProject>.Failure(project.Message);
                project.Data.Name = updateProject.ProjectName ?? project.Data.Name;
                project.Data.Description = updateProject.ProjectDescription??project.Data.Description;
                project.Data.StartDate = updateProject.StartDate?? project.Data.StartDate;
                project.Data.EndDate = updateProject.EndDate?? project.Data.EndDate;
                project.Data.StatusId = DateTime.Now == updateProject.StartDate
                    ? 3 // Ongoing
                    : (DateTime.Now < updateProject.StartDate ? 2 : 1);// Upcoming or Overdue

                var update = await _unitOfWork.ProjectRepository.UpdateAsync(project.Data);
                if(!update.Status) return ServicesResult<DetailProject>.Failure($"{update.Message}");
                var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);

                if (!user.Status) return ServicesResult<DetailProject>.Failure(user.Message);
                // create a activity log for project and owner role project
                var activityProject = new ActivityLog()
                {
                    Id = "",
                    ActionDate = DateTime.Now,
                    Action = $"A new project was created by {user.Data.NickName}",
                    ProjectId = project.Data.Id,
                };
                var acvitity = await _unitOfWork.ActivityLogRepository.AddAsync(activityProject);
                if (!acvitity.Status) return ServicesResult<DetailProject>.Failure(acvitity.Message);

                // xác thực thành công

                
                // tạo kết quả trả về
                var response = new DetailProject()
                {
                    ProjectId = project.Data.Id,
                    OwnerName = user.Data.NickName,
                    OwnerAvata = user.Data.AvatarPath,
                    CreateAt = project.Data.CreatedDate,
                    EndAt = project.Data.EndDate,
                    ProjectName = project.Data.Name,
                    IsCompleted = project.Data.IsCompleted,
                    IsDeleted = project.Data.IsDeleted,
                };
                var memberProject = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!memberProject.Status) return ServicesResult<DetailProject>.Failure(memberProject.Message);
                foreach (var member in memberProject.Data)
                {
                    var infoMember = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.UserId);
                    if (!infoMember.Status) return ServicesResult<DetailProject>.Failure(infoMember.Message);
                    var index = new IndexMember()
                    {
                        PositionWorkName = member.PositionWork,
                        UserName = infoMember.Data.NickName,
                        RoleUserInProjectId = member.Id,
                        UserAvata = infoMember.Data.AvatarPath,
                    };
                    response.Members.Add(index);
                }
                return ServicesResult<DetailProject>.Success(response);
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
    }
    #endregion
}
