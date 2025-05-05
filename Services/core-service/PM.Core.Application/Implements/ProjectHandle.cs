using PM.Core.Application.Interfaces;
using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos;
using PM.Shared.Dtos.cores;
using PM.Shared.Dtos.cores.projects;
using PM.Shared.Dtos.users;
using PM.Shared.Handle.Implements;
using PM.Shared.Handle.Interfaces;

namespace PM.Core.Application.Implements
{
    public class ProjectHandle : IProjectHandle
    {
        private readonly IUnitOfWork<CoreDbContext> _unitOfWork;
        private readonly IAPIService<UserDetail> _userAPI;
        private readonly IPositionHandle _positionHandle;
        private readonly IProjectMemberHandle _projectMemberHandle;
        private readonly Position _ownerPosition;

        // Constructor: Inject dependencies and preload the "Product Owner" position.
        public ProjectHandle(
            IUnitOfWork<CoreDbContext> unitOfWork,
            IAPIService<UserDetail> userAPI,
            IProjectMemberHandle projectMemberHandle,
            IPositionHandle positionHandle)
        {
            _projectMemberHandle = projectMemberHandle;
            _unitOfWork = unitOfWork;
            _userAPI = userAPI;
            _positionHandle = positionHandle;

            // Synchronously get the "Product Owner" position once during construction.
            _ownerPosition = _positionHandle.GetPositionByName("Product Owner").GetAwaiter().GetResult().Data;
        }

        #region Get Projects User Has Joined

        /// <summary>
        /// Retrieves all projects that a user is participating in (regardless of role).
        /// </summary>
        /// <param name="userId">The ID of the user whose joined projects are to be retrieved.</param>
        /// <returns>A ServiceResult containing a list of projects the user has joined or an error message.</returns>
        public async Task<ServiceResult<IEnumerable<IndexProjectModel>>> ProjectsUserHasJoined(string userId)
        {
            // Validate the userId input
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.Error("UserId is required.");
            }

            List<IndexProjectModel> result = new();

            try
            {
                // Get all project members associated with the user
                var memberResult = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("UserId", userId);

                // Check if fetching the project members was successful
                if (!memberResult.IsSuccess())
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error(memberResult.Message);
                }

                // If the user has no project memberships, return an empty list
                if (memberResult.Data == null || !memberResult.Data.Any())
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Success(result);
                }

                // Iterate over each project member and gather the project details
                foreach (var member in memberResult.Data)
                {
                    // Retrieve project details based on the project ID
                    var projectResult = await _unitOfWork.Repository<Project>().GetOneAsync("Id", member.ProjectId);
                    if (!projectResult.IsSuccess())
                    {
                        return ServiceResult<IEnumerable<IndexProjectModel>>.Error(projectResult.Message);
                    }

                    // Retrieve the user details for the project member
                    var userResult = await _userAPI.APIsGetAsync($"api/user/get-user?userId={member.UserId}");
                    if (!userResult.IsSuccess())
                    {
                        return ServiceResult<IEnumerable<IndexProjectModel>>.Error(userResult.Message);
                    }

                    // Add the project to the result list
                    result.Add(new IndexProjectModel
                    {
                        Id = projectResult.Data.Id,
                        Name = projectResult.Data.Name,
                        Description = projectResult.Data.Description,
                        CreatedBy = userResult.Data.UserName
                    });
                }

                // Return the list of projects the user has joined
                return ServiceResult<IEnumerable<IndexProjectModel>>.Success(result);
            }
            catch (Exception ex)
            {
                // Return an exception result if something goes wrong
                return ServiceResult<IEnumerable<IndexProjectModel>>.FromException(ex);
            }
        }

        #endregion


        #region Get Projects Where User is Owner

        /// <summary>
        /// Retrieves all projects where the user has the "Product Owner" role.
        /// </summary>
        public async Task<ServiceResult<IEnumerable<IndexProjectModel>>> ProjectUserIsOwner(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult<IEnumerable<IndexProjectModel>>.Error("UserId is required.");

            List<IndexProjectModel> result = [];

            try
            {
                var userResult = await _userAPI.APIsGetAsync($"api/user/get-user?userId={userId}");
                if (!userResult.IsSuccess()) return ServiceResult<IEnumerable<IndexProjectModel>>.Error(userResult.Message);

                var memberResult = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("UserId", userId);
                if (!memberResult.IsSuccess()) return ServiceResult<IEnumerable<IndexProjectModel>>.Error(memberResult.Message);

                var ownerProjects = memberResult.Data?.Where(m => m.PositionId == _ownerPosition.Id).ToList();
                if (ownerProjects == null || !ownerProjects.Any())
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Success(result);

                foreach (var member in ownerProjects)
                {
                    var projectResult = await _unitOfWork.Repository<Project>().GetOneAsync("Id", member.ProjectId);
                    if (!projectResult.IsSuccess()) return ServiceResult<IEnumerable<IndexProjectModel>>.Error(projectResult.Message);

                    result.Add(new IndexProjectModel
                    {
                        Id = projectResult.Data.Id,
                        Name = projectResult.Data.Name,
                        Description = projectResult.Data.Description,
                        CreatedBy = userResult.Data.FullName
                    });
                }

                return ServiceResult<IEnumerable<IndexProjectModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.FromException(ex);
            }
        }
        #endregion

        #region Get Project Detail

        /// <summary>
        /// Fetches the full details of a project, including metadata and status.
        /// </summary>
        public async Task<ServiceResult<DetailProjectModel>> GetDetailProject(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
                return ServiceResult<DetailProjectModel>.Error("ProjectId is required.");

            try
            {
                var projectResult = await _unitOfWork.Repository<Project>().GetOneAsync("Id", projectId);
                if (!projectResult.IsSuccess()) return ServiceResult<DetailProjectModel>.Error("Project not found.");

                var ownerMember = (await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", projectId))
                    .Data.FirstOrDefault(m => m.PositionId == _ownerPosition.Id);

                return ownerMember == null
                    ? ServiceResult<DetailProjectModel>.Error("Owner not found.")
                    : ServiceResult<DetailProjectModel>.Success(new DetailProjectModel
                    {
                        Id = projectResult.Data.Id,
                        Name = projectResult.Data.Name,
                        Description = projectResult.Data.Description,
                        CreateAt = projectResult.Data.CreateAt,
                        UpdateAt = projectResult.Data.UpdateAt,
                        StartAt = projectResult.Data.StartAt,
                        EndAt = projectResult.Data.EndAt,
                        IsComplied = projectResult.Data.IsComplied,
                        IsDeleted = projectResult.Data.IsDeleted,
                        IsLocked = projectResult.Data.IsLocked,
                        IsModified = projectResult.Data.IsModified,
                        Status = nameof(projectResult.Data.Status)
                    });
            }
            catch (Exception ex)
            {
                return ServiceResult<DetailProjectModel>.FromException(ex);
            }
        }
        #endregion

        #region Search Projects By Text

        /// <summary>
        /// Searches for projects whose name or description contains the given text.
        /// </summary>
        public async Task<ServiceResult<IEnumerable<IndexProjectModel>>> GetProjectsByText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return ServiceResult<IEnumerable<IndexProjectModel>>.Error("Search text is required.");

            List<IndexProjectModel> result = [];

            try
            {
                var allProjects = (await _unitOfWork.Repository<Project>().GetAllAsync()).Data
                    .Where(p => p.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
                             || p.Description.Contains(text, StringComparison.OrdinalIgnoreCase));

                foreach (var project in allProjects)
                {
                    var ownerMember = (await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", project.Id))
                        .Data.FirstOrDefault(m => m.PositionId == _ownerPosition.Id);

                    if (ownerMember == null) continue;

                    var userResult = await _userAPI.APIsGetAsync($"api/user/get-user?userId={ownerMember.UserId}");
                    if (!userResult.IsSuccess()) continue;

                    result.Add(new IndexProjectModel
                    {
                        Id = project.Id,
                        Name = project.Name,
                        Description = project.Description,
                        CreatedBy = userResult.Data.UserName
                    });
                }

                return ServiceResult<IEnumerable<IndexProjectModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.FromException(ex);
            }
        }
        #endregion


        #region Add New Project

        /// <summary>
        /// Adds a new project for a user, ensuring uniqueness of name under the same ownership.
        /// </summary>
        public async Task<ServiceResult<DetailProjectModel>> AddAsync(string userId, AddProjectModel model)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(model.Name) ||
                string.IsNullOrWhiteSpace(model.Description))
            {
                return ServiceResult<DetailProjectModel>.Error("Invalid input data.");
            }

            try
            {
                // Get all project memberships of the user
                ServiceResult<IEnumerable<ProjectMember>> memberResult = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("UserId", userId);
                if (memberResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<DetailProjectModel>.Error(memberResult.Message);
                }

                // Filter projects where user is the owner
                List<ProjectMember>? ownerProjects = memberResult.Data?.Where(m => m.PositionId == _ownerPosition.Id).ToList();
                if (ownerProjects == null)
                {
                    return await AddAction(userId, model);
                }

                // Check for duplicate project name
                List<Project> existingProjects = [];
                foreach (ProjectMember? owner in ownerProjects)
                {
                    ServiceResult<Project> projectResult = await _unitOfWork.Repository<Project>().GetOneAsync("Id", owner.ProjectId);
                    if (projectResult.Status != ResultStatus.Success || projectResult.Data == null)
                    {
                        return ServiceResult<DetailProjectModel>.Error(projectResult.Message);
                    }

                    existingProjects.Add(projectResult.Data);
                }

                ServiceResult<bool> isNameAvailable = await _unitOfWork.Repository<Project>().IsExistName(existingProjects, model.Name);
                return isNameAvailable.Status != ResultStatus.Success || isNameAvailable.Data
                    ? ServiceResult<DetailProjectModel>.Error("Duplicate project name.")
                    : await AddAction(userId, model);
            }
            catch (Exception ex)
            {
                return ServiceResult<DetailProjectModel>.FromException(ex);
            }
        }

        /// <summary>
        /// Internal logic to create and save the project and assign ownership.
        /// </summary>
        private async Task<ServiceResult<DetailProjectModel>> AddAction(string userId, AddProjectModel model)
        {
            try
            {
                // Create new project entity
                Project project = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    Description = model.Description,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    StartAt = new DateTime(model.StartAt.Day, model.StartAt.Month, model.StartAt.Year),
                    EndAt = new DateTime(model.EndAt.Day, model.EndAt.Month, model.EndAt.Year),
                    IsModified = true,
                    IsLocked = false,
                    IsDeleted = false,
                    IsComplied = false,
                };
                project.Status = (TypeStatus)new StatusHandle(project.StartAt, project.EndAt, false).GetStatus();

                // Add project to DB
                ServiceResult<bool> addProjectResult = await _unitOfWork.ExecuteTransactionAsync(() =>
                    _unitOfWork.Repository<Project>().AddAsync(project));
                if (addProjectResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<DetailProjectModel>.Error("Failed to add project.");
                }

                // Add owner as project member
                ProjectMember member = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    PositionId = _ownerPosition.Id,
                    ProjectId = project.Id,
                    UserId = userId
                };
                ServiceResult<bool> addMemberResult = await _unitOfWork.ExecuteTransactionAsync(() =>
                    _unitOfWork.Repository<ProjectMember>().AddAsync(member));
                return addMemberResult.Status != ResultStatus.Success
                    ? ServiceResult<DetailProjectModel>.Error("Failed to add project member.")
                    : await GetDetailProject(project.Id);
            }
            catch (Exception ex)
            {
                return ServiceResult<DetailProjectModel>.FromException(ex);
            }
        }

        #endregion


        #region Patch Project

        /// <summary>
        /// Updates existing project information if the user is the project owner.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the patch.</param>
        /// <param name="projectId">The ID of the project to update.</param>
        /// <param name="model">The updated project model containing the new information.</param>
        /// <returns>A ServiceResult with the updated project details or an error message.</returns>
        public async Task<ServiceResult<DetailProjectModel>> PatchAsync(string userId, string projectId, PatchProjectModel model)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(projectId) ||
                string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Description))
            {
                return ServiceResult<DetailProjectModel>.Error("Invalid input.");
            }

            try
            {
                // Get all project members associated with the user
                ServiceResult<IEnumerable<ProjectMember>> memberResult = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("UserId", userId);
                if (memberResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<DetailProjectModel>.Error(memberResult.Message);
                }

                // Filter out the projects where the user is the owner
                List<ProjectMember>? ownerProjects = memberResult.Data?.Where(m => m.PositionId == _ownerPosition.Id).ToList();

                // If no owner projects are found, proceed with patching directly
                if (ownerProjects == null || ownerProjects.Count == 0)
                {
                    return await PatchAction(userId, projectId, model);
                }

                // Check if the user is the owner of the specified project
                bool userIsOwner = ownerProjects.Any(x => x.ProjectId == projectId);
                if (!userIsOwner)
                {
                    return ServiceResult<DetailProjectModel>.Error("Permission denied.");
                }

                // Retrieve all projects owned by the user and check if any project name conflicts with the new name
                List<Project> existingProjects = new();
                foreach (var owner in ownerProjects)
                {
                    ServiceResult<Project> projectResult = await _unitOfWork.Repository<Project>().GetOneAsync("Id", owner.ProjectId);
                    if (projectResult.Status != ResultStatus.Success || projectResult.Data == null)
                    {
                        return ServiceResult<DetailProjectModel>.Error(projectResult.Message);
                    }

                    existingProjects.Add(projectResult.Data);
                }

                // Check if the project name is already in use
                ServiceResult<bool> isNameAvailable = await _unitOfWork.Repository<Project>().IsExistName(existingProjects, model.Name);
                if (isNameAvailable.Status != ResultStatus.Success || isNameAvailable.Data)
                {
                    return ServiceResult<DetailProjectModel>.Error("Duplicate project name.");
                }

                // Proceed with patching the project
                return await PatchAction(userId, projectId, model);
            }
            catch (Exception ex)
            {
                return ServiceResult<DetailProjectModel>.FromException(ex);
            }
        }

        /// <summary>
        /// Internal method for patching project entity.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the patch.</param>
        /// <param name="projectId">The ID of the project to update.</param>
        /// <param name="model">The updated project model containing the new information.</param>
        /// <returns>A ServiceResult with the updated project details or an error message.</returns>
        private async Task<ServiceResult<DetailProjectModel>> PatchAction(string userId, string projectId, PatchProjectModel model)
        {
            try
            {
                // Retrieve the project by ID
                ServiceResult<Project> project = await _unitOfWork.Repository<Project>().GetOneAsync("Id", projectId);
                if (project.Status != ResultStatus.Success || project.Data == null)
                {
                    return ServiceResult<DetailProjectModel>.Error("Project not found.");
                }

                // Prepare a dictionary of updated project fields
                Dictionary<string, object> keyValuePairs = new()
                {
                    [nameof(project.Data.Name)] = model.Name,
                    [nameof(project.Data.Description)] = model.Description,
                    [nameof(project.Data.IsComplied)] = model.IsComplied,
                    [nameof(project.Data.IsModified)] = model.IsModified,
                    [nameof(project.Data.IsDeleted)] = model.IsDeleted,
                    [nameof(project.Data.IsLocked)] = model.IsLocked,
                    [nameof(project.Data.UpdateAt)] = DateTime.Now,
                    [nameof(project.Data.StartAt)] = new DateTime(model.StartAt.Day, model.StartAt.Month, model.StartAt.Year),
                    [nameof(project.Data.EndAt)] = new DateTime(model.EndAt.Day, model.EndAt.Month, model.EndAt.Year),
                    [nameof(project.Data.Status)] = (TypeStatus)new StatusHandle(
                        new DateTime(model.StartAt.Day, model.StartAt.Month, model.StartAt.Year),
                        new DateTime(model.EndAt.Day, model.EndAt.Month, model.EndAt.Year),
                        model.IsComplied).GetStatus()
                };

                // Execute the transaction to patch the project data
                ServiceResult<bool> patchResult = await _unitOfWork.ExecuteTransactionAsync(() =>
                    _unitOfWork.Repository<Project>().PatchAsync(project.Data, keyValuePairs));

                // Check if the patch was successful
                if (patchResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<DetailProjectModel>.Error("Update failed.");
                }

                // Return the updated project details
                return await GetDetailProject(projectId);
            }
            catch (Exception ex)
            {
                return ServiceResult<DetailProjectModel>.FromException(ex);
            }
        }

        #endregion

        #region Delete Project

        /// <summary>
        /// Asynchronously deletes a project by the given project ID if the user is the owner.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the deletion.</param>
        /// <param name="projectId">The ID of the project to be deleted.</param>
        /// <returns>A ServiceResult containing a collection of index project models or an error.</returns>
        public async Task<ServiceResult<IEnumerable<IndexProjectModel>>> DeleteAsync(string userId, string projectId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.Error("Invalid user or project ID.");
            }

            try
            {
                // Check if the user is the owner of the project
                bool isOwner = (await _unitOfWork.Repository<ProjectMember>().GetAllAsync()).Data
                    .Any(x => x.ProjectId == projectId && x.UserId == userId && x.PositionId == _ownerPosition.Id);

                // If user is not the owner, return an error
                if (!isOwner)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error("User is not the owner of the project.");
                }

                // Retrieve the project to be deleted
                ServiceResult<Project> projectResult = await _unitOfWork.Repository<Project>().GetOneAsync("Id", projectId);

                // If the project is not found or there was an error retrieving it, return an error
                if (projectResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error("Project not found or failed to retrieve.");
                }

                // Attempt to delete all members associated with the project
                ServiceResult<IEnumerable<Shared.Dtos.cores.members.projects.IndexProjectMemberModel>> deleteMemberResult = await _projectMemberHandle.DeteleManyAsync(projectId);

                // If member deletion failed, return an error
                if (!deleteMemberResult.IsSuccess())
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error("Failed to delete project members.");
                }

                // Attempt to delete the project itself
                ServiceResult<bool> deleteProjectResult = await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<Project>().DeleteAsync(projectResult.Data));

                // If project deletion failed, return an error
                if (deleteProjectResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error("Failed to delete the project.");
                }

                // Return the updated project list, excluding the deleted project
                return await ProjectUserIsOwner(userId);
            }
            catch (Exception ex)
            {
                // Return the exception details if any error occurs during the process
                return ServiceResult<IEnumerable<IndexProjectModel>>.FromException(ex);
            }
        }
        #endregion

    }
}
