using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Application.Contracts.Interfaces;
using PM.Application.DTOs.Project;
using PM.Application.Features.Projects.Command;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces.Services;
using System.Data.Entity.Migrations.Design;

namespace PM.Application.Services
{
    public class ProjectFlowLogic : IProjectFlowLogic
    {
        private readonly IProjectServices _projectServices;
        private readonly IProjectMemberServices _projectMemberServices;
        private readonly IActivityLogServices _activityLogServices;
        private readonly IPlanServices _planServices;
        private readonly IStatusServices _statusServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IUserServices _userServices;
        private readonly ILogger<ProjectFlowLogic> _logger;

        public ProjectFlowLogic(IProjectServices projectServices,
                                IProjectMemberServices projectMemberServices,
                                IActivityLogServices activityLogServices,
                                IPlanServices planServices,
                                IStatusServices statusServices,
                                IRoleInProjectServices roleInProjectServices,
                                IUserServices userServices,
                                ILogger<ProjectFlowLogic> logger)
        {
            _projectServices = projectServices;
            _projectMemberServices = projectMemberServices;
            _activityLogServices = activityLogServices;
            _planServices = planServices;
            _statusServices = statusServices;
            _roleInProjectServices = roleInProjectServices;
            _userServices = userServices;
            _logger = logger;
        }

        public async Task<ServicesResult<IEnumerable<ProjectIndexDTO>>> GetProjectsUserHasJoined(string userId)
        {
            var userResponse = await _userServices.GetDetailUserAsync(userId);
            if (!userResponse.Status)
            {
                _logger.LogError("User not found: {UserId}", userId);
                return ServicesResult<IEnumerable<ProjectIndexDTO>>.Failure("User not found");
            }

            var projectsResponse = await _projectMemberServices.GetProjectMembersInProject(userId);
            if (!projectsResponse.Status)
            {
                _logger.LogError("Failed to retrieve projects for user: {UserId}", userId);
                return ServicesResult<IEnumerable<ProjectIndexDTO>>.Failure("Failed to retrieve projects");
            }

            var response = projectsResponse.Data!
                .Select(async item =>
                {
                    var project = await _projectServices.GetProjectAsync(item.ProjectId);
                    return project.Status && !project.Data!.IsDeleted
                        ? new ProjectIndexDTO { Id = item.Id, Name = project.Data.Name, CreatedDate = project.Data.CreatedDate }
                        : null;
                })
                .Where(task => task.Result != null)
                .Select(task => task.Result!);

            return ServicesResult<IEnumerable<ProjectIndexDTO>>.Success(response);
        }

        public async Task<ServicesResult<IEnumerable<ProjectIndexDTO>>> GetProjectsUserHasOwner(string userId)
        {
            var userResponse = await _userServices.GetDetailUserAsync(userId);
            if (!userResponse.Status)
            {
                _logger.LogError("User not found: {UserId}", userId);
                return ServicesResult<IEnumerable<ProjectIndexDTO>>.Failure("User not found");
            }

            var projectResponse = await GetProjectsUserHasOwnerMethod(userId);
            if (!projectResponse.Status)
            {
                _logger.LogError("Failed to retrieve projects for user: {UserId}", userId);
                return ServicesResult<IEnumerable<ProjectIndexDTO>>.Failure("Failed to retrieve projects");
            }

            var response = projectResponse.Data!
                .Select(item => new ProjectIndexDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    OwnerName = userResponse.Data!.UserName ?? "empty",
                    CreatedDate = item.CreatedDate
                });

            return ServicesResult<IEnumerable<ProjectIndexDTO>>.Success(response);
        }

        private async Task<ServicesResult<IEnumerable<Project>>> GetProjectsUserHasOwnerMethod(string userId)
        {
            var ownerRole = await _roleInProjectServices.GetOwnerRole();
            if (!ownerRole.Status)
            {
                _logger.LogError("Failed to retrieve owner role");
                return ServicesResult<IEnumerable<Project>>.Failure("Failed to retrieve owner role");
            }

            var projectsResponse = await _projectMemberServices.GetProjectMembersInProject(userId);
            if (!projectsResponse.Status)
            {
                _logger.LogError("Failed to retrieve projects for user: {UserId}", userId);
                return ServicesResult<IEnumerable<Project>>.Failure("Failed to retrieve projects");
            }

            var response = projectsResponse.Data!
                .Where(item => item.RoleId == ownerRole.Data!.Id)
                .Select(async item => await _projectServices.GetProjectAsync(item.ProjectId))
                .Where(task => task.Result.Status)
                .Select(task => task.Result.Data!);

            return ServicesResult<IEnumerable<Project>>.Success(response);
        }

        #region
        public async Task<ServicesResult<ProjectDetailDTO>> GetDetailProject(string projectId)
        {
            var projectResponse = await _projectServices.GetProjectAsync(projectId);
            if (!projectResponse.Status)
            {
                _logger.LogError("Project not found: {ProjectId}", projectId);
                return ServicesResult<ProjectDetailDTO>.Failure("Project not found");
            }

            var statusInfo = await _statusServices.GetStatusByIdAsync(projectResponse.Data!.StatusId);
            if (!statusInfo.Status)
            {
                _logger.LogError("Failed to retrieve project status for project: {ProjectId}", projectId);
                return ServicesResult<ProjectDetailDTO>.Failure("Failed to retrieve project status");
            }

            var response = new ProjectDetailDTO
            {
                Id = projectId,
                Name = projectResponse.Data.Name,
                Description = projectResponse.Data.Description,
                StartDate = projectResponse.Data.StartDate,
                EndDate = projectResponse.Data.EndDate,
                CreatedDate = projectResponse.Data.CreatedDate,
                StatusInfo = statusInfo.Data!.Name,
                CompleteInfo = projectResponse.Data.IsCompleted ? "Complete" : "Not Complete",
                DeleteInfo = projectResponse.Data.IsDeleted ? "Deleted" : "Active"
            };

            return ServicesResult<ProjectDetailDTO>.Success(response);
        }
        #endregion

        #region
        public async Task<ServicesResult<ProjectDetailDTO>> AddProject([FromBody] AddProjectCommand model)
        {
            var project = new Project()
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.ProjectName,
                Description = model.ProjectDescription,
                EndDate = new DateTime(model.EndAt.Year, model.EndAt.Month, model.EndAt.Day),
                StartDate = new DateTime(model.StartAt.Year, model.StartAt.Month, model.StartAt.Day),
                CreatedDate = DateTime.Now,
                IsCompleted = false,
                IsDeleted = false,
                StatusId = _statusServices.StatusForCreateAsync(new DateTime(model.StartAt.Year, model.StartAt.Month, model.StartAt.Day))
            };

            var projectsUserIsOwner = await GetProjectsUserHasOwnerMethod(model.UserId);
            var addProjectResponse = await _projectServices.CreateProjectAsync(projectsUserIsOwner.Data!, project);
            if (addProjectResponse.Status == false)
            {
                _logger.LogError("Failed to create project for user: {UserId}", model.UserId);
                return ServicesResult<ProjectDetailDTO>.Failure("Failed to create project");
            }

            var ownerRole = await _roleInProjectServices.GetOwnerRole();
            if (!ownerRole.Status)
            {
                _logger.LogError("Failed to retrieve owner role");
                return ServicesResult<ProjectDetailDTO>.Failure("Failed to retrieve owner role");
            }

            var member = new ProjectMember()
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = project.Id,
                RoleId = ownerRole.Data!.Id,
                UserId = model.UserId,
            };

            var addProjectMemberResponse = await _projectMemberServices.AddMember(member);
            if (addProjectMemberResponse.Status == false)
            {
                _logger.LogError("Failed to add member to project: {ProjectId}", project.Id);
                return ServicesResult<ProjectDetailDTO>.Failure("Failed to add member to project");
            }

            var log = new ActivityLog()
            {
                Id = Guid.NewGuid().ToString(),
                Action = "Project created",
                ActionDate = DateTime.Now,
                ProjectId = project.Id,
                UserId = model.UserId,
            };

            var addLogResponse = await _activityLogServices.AddAsync(log);
            if (addLogResponse.Status == false)
            {
                _logger.LogError("Failed to log activity for project creation: {ProjectId}", project.Id);
                return ServicesResult<ProjectDetailDTO>.Failure("Failed to log activity");
            }

            return await GetDetailProject(project.Id);
        }
        #endregion

        #region
        public async Task<ServicesResult<ProjectDetailDTO>> PatchProject(PatchProjectCommand command)
        {
            var ownerRole = await _roleInProjectServices.GetOwnerRole();
            if (!ownerRole.Status)
            {
                _logger.LogError("Failed to retrieve owner role");
                return ServicesResult<ProjectDetailDTO>.Failure("Failed to retrieve owner role");
            }

            var userResponse = await _userServices.GetDetailUserAsync(command.UserId);
            if (!userResponse.Status)
            {
                _logger.LogError("User not found: {UserId}", command.UserId);
                return ServicesResult<ProjectDetailDTO>.Failure("User not found");
            }

            var isOwner = await _projectMemberServices.GetOwnerProject(command.ProjectId, ownerRole.Data!.Id);
            if (!isOwner.Status)
            {
                _logger.LogError("Failed to retrieve owner information for project: {ProjectId}", command.ProjectId);
                return ServicesResult<ProjectDetailDTO>.Failure("Failed to retrieve owner information");
            }

            if (command.UserId != isOwner.Data!.UserId)
            {
                _logger.LogError("User {UserId} is not the owner of project: {ProjectId}", command.UserId, command.ProjectId);
                return ServicesResult<ProjectDetailDTO>.Failure("User is not the owner");
            }

            var projectResponse = await _projectServices.GetProjectAsync(command.ProjectId);
            if (!projectResponse.Status)
            {
                _logger.LogError("Project not found: {ProjectId}", command.ProjectId);
                return ServicesResult<ProjectDetailDTO>.Failure("Project not found");
            }

            projectResponse.Data!.Name = command.ProjectName ?? projectResponse.Data.Name;
            projectResponse.Data.Description = command.ProjectDescription ?? projectResponse.Data.Description;
            projectResponse.Data.StartDate = new DateTime(command.StartDate.Year, command.StartDate.Month, command.StartDate.Day);
            projectResponse.Data.EndDate = new DateTime(command.EndDate.Year, command.EndDate.Month, command.EndDate.Day);
            projectResponse.Data.IsCompleted = command.IsComplete;
            projectResponse.Data.IsDeleted = command.IsDelete;

            if (projectResponse.Data.IsCompleted)
            {
                projectResponse.Data.StatusId =  _statusServices.StatusForUpdateAsync(projectResponse.Data.StartDate, projectResponse.Data.EndDate);
            }
            projectResponse.Data.StatusId = _statusServices.StatusForFinallyAsync(projectResponse.Data.EndDate);

            var log = new ActivityLog()
            {
                Id = Guid.NewGuid().ToString(),
                Action = "Project updated",
                ActionDate = DateTime.Now,
                ProjectId = projectResponse.Data.Id,
                UserId = command.UserId,
            };

            var addLogResponse = await _activityLogServices.AddAsync(log);
            if (addLogResponse.Status == false)
            {
                _logger.LogError("Failed to log activity for project update: {ProjectId}", command.ProjectId);
                return ServicesResult<ProjectDetailDTO>.Failure("Failed to log activity");
            }

            return await GetDetailProject(command.ProjectId);
        }
        #endregion
        //#region
        //public async Task<ServicesResult<IEnumerable<ProjectIndexDTO>>> DeleteProject(DeleteProjectCommand command)
        //{
        //    var ownerRole = await _roleInProjectServices.GetOwnerRole();
        //    if (!ownerRole.Status)
        //    {
        //        _logger.LogError("Failed to retrieve owner role");
        //        return ServicesResult<IEnumerable<ProjectIndexDTO>>.Failure("Failed to retrieve owner role");
        //    }

        //    var userResponse = await _userServices.GetDetailUserAsync(command.UserId);
        //    if (!userResponse.Status)
        //    {
        //        _logger.LogError("User not found: {UserId}", command.UserId);
        //        return ServicesResult<IEnumerable<ProjectIndexDTO>>.Failure("User not found");
        //    }

        //    var isOwner = await _projectMemberServices.GetOwnerProject(command.ProjectId, ownerRole.Data!.Id);
        //    if (!isOwner.Status)
        //    {
        //        _logger.LogError("Failed to retrieve owner information for project: {ProjectId}", command.ProjectId);
        //        return ServicesResult<IEnumerable<ProjectIndexDTO>>.Failure("Failed to retrieve owner information");
        //    }
        //    var plans = await _planServices.GetPlansInProject(command.ProjectId);
        //    if (!plans.Status)
        //    {
        //        _logger.LogError("User not found: {UserId}", command.UserId);
        //        return ServicesResult<IEnumerable<ProjectIndexDTO>>.Failure("User not found");
        //    }
        //    foreach (var plan in plans.Data!.Select(x => x.Id))
        //    {
        //       var deletePlanResponse = await _planServices.DeleteAsync(plan);
        //        if (!deletePlanResponse.Status)
        //        {
        //            _logger.LogError("User not found: {UserId}", command.UserId);
        //            return ServicesResult<IEnumerable<ProjectIndexDTO>>.Failure("User not found");
        //        }
        //    }



        //}
        //#endregion
    }
}
