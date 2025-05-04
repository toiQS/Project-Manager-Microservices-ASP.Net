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
        private readonly Position _ownerPosition;

        public ProjectHandle(
            IUnitOfWork<CoreDbContext> unitOfWork,
            IAPIService<UserDetail> userAPI,
            IPositionHandle positionHandle)
        {
            _unitOfWork = unitOfWork;
            _userAPI = userAPI;
            _positionHandle = positionHandle;
            _ownerPosition = _positionHandle.GetPositionByName("Product Owner").GetAwaiter().GetResult().Data;
        }

        #region Get Projects User Has Joined
        public async Task<ServiceResult<IEnumerable<IndexProjectModel>>> ProjectsUserHasJoined(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.Error("UserId is required.");
            }

            List<IndexProjectModel> result = new List<IndexProjectModel>();

            try
            {
                ServiceResult<IEnumerable<ProjectMember>> memberResult = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("UserId", userId);
                if (memberResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error(memberResult.Message);
                }

                if (memberResult.Data == null)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Success(result);
                }

                foreach (ProjectMember member in memberResult.Data)
                {
                    ServiceResult<Project> projectResult = await _unitOfWork.Repository<Project>().GetOneAsync("Id", member.ProjectId);
                    if (projectResult.Status != ResultStatus.Success || projectResult.Data == null)
                    {
                        return ServiceResult<IEnumerable<IndexProjectModel>>.Error(projectResult.Message);
                    }

                    ServiceResult<UserDetail> userResult = await _userAPI.APIsGetAsync($"api/user/get-user?userId={member.UserId}");
                    if (userResult.Status != ResultStatus.Success || userResult.Data == null)
                    {
                        return ServiceResult<IEnumerable<IndexProjectModel>>.Error(userResult.Message);
                    }

                    result.Add(new IndexProjectModel
                    {
                        Id = projectResult.Data.Id,
                        Name = projectResult.Data.Name,
                        Description = projectResult.Data.Description,
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

        #region Get Projects Where User is Owner
        public async Task<ServiceResult<IEnumerable<IndexProjectModel>>> ProjectUserIsOwner(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.Error("UserId is required.");
            }

            List<IndexProjectModel> result = new List<IndexProjectModel>();

            try
            {
                ServiceResult<UserDetail> userResult = await _userAPI.APIsGetAsync($"api/user/get-user?userId={userId}");
                if (userResult.Status != ResultStatus.Success || userResult.Data == null)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error(userResult.Message);
                }

                ServiceResult<IEnumerable<ProjectMember>> memberResult = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("UserId", userId);
                if (memberResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error(memberResult.Message);
                }

                List<ProjectMember>? ownerProjects = memberResult.Data?.Where(m => m.PositionId == _ownerPosition.Id).ToList();
                if (ownerProjects == null || !ownerProjects.Any())
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Success(result);
                }

                foreach (ProjectMember? member in ownerProjects)
                {
                    ServiceResult<Project> projectResult = await _unitOfWork.Repository<Project>().GetOneAsync("Id", member.ProjectId);
                    if (projectResult.Status != ResultStatus.Success || projectResult.Data == null)
                    {
                        return ServiceResult<IEnumerable<IndexProjectModel>>.Error(projectResult.Message);
                    }

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
        public async Task<ServiceResult<DetailProjectModel>> GetDetailProject(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return ServiceResult<DetailProjectModel>.Error("ProjectId is required.");
            }

            try
            {
                ServiceResult<Project> projectResult = await _unitOfWork.Repository<Project>().GetOneAsync("Id", projectId);
                if (projectResult.Status != ResultStatus.Success || projectResult.Data == null)
                {
                    return ServiceResult<DetailProjectModel>.Error("Project not found.");
                }

                ProjectMember? ownerMember = (await _unitOfWork.Repository<ProjectMember>()
                    .GetManyAsync("ProjectId", projectId))
                    .Data
                    .FirstOrDefault(m => m.PositionId == _ownerPosition.Id);

                if (ownerMember == null)
                {
                    return ServiceResult<DetailProjectModel>.Error("Owner not found.");
                }

                return ServiceResult<DetailProjectModel>.Success(new DetailProjectModel
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
        public async Task<ServiceResult<IEnumerable<IndexProjectModel>>> GetProjectsByText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.Error("Search text is required.");
            }

            List<IndexProjectModel> result = new List<IndexProjectModel>();

            try
            {
                IEnumerable<Project> allProjects = (await _unitOfWork.Repository<Project>().GetAllAsync()).Data
                    .Where(p => p.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
                             || p.Description.Contains(text, StringComparison.OrdinalIgnoreCase));

                foreach (Project? project in allProjects)
                {
                    ProjectMember? ownerMember = (await _unitOfWork.Repository<ProjectMember>().GetManyAsync("ProjectId", project.Id))
                        .Data.FirstOrDefault(m => m.PositionId == _ownerPosition.Id);

                    if (ownerMember == null)
                    {
                        continue;
                    }

                    ServiceResult<UserDetail> userResult = await _userAPI.APIsGetAsync($"api/user/get-user?userId={ownerMember.UserId}");
                    if (userResult.Status != ResultStatus.Success || userResult.Data == null)
                    {
                        continue;
                    }

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
                List<Project> existingProjects = new List<Project>();
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
                if (isNameAvailable.Status != ResultStatus.Success || isNameAvailable.Data)
                {
                    return ServiceResult<DetailProjectModel>.Error("Duplicate project name.");
                }

                return await AddAction(userId, model);
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
                Project project = new Project
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
                ProjectMember member = new ProjectMember
                {
                    Id = Guid.NewGuid().ToString(),
                    PositionId = _ownerPosition.Id,
                    ProjectId = project.Id,
                    UserId = userId
                };
                ServiceResult<bool> addMemberResult = await _unitOfWork.ExecuteTransactionAsync(() =>
                    _unitOfWork.Repository<ProjectMember>().AddAsync(member));
                if (addMemberResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<DetailProjectModel>.Error("Failed to add project member.");
                }

                return await GetDetailProject(project.Id);
            }
            catch (Exception ex)
            {
                return ServiceResult<DetailProjectModel>.FromException(ex);
            }
        }

        #endregion


        #region Patch Project
        /// <summary>
        /// Updates existing project information if user is the project owner.
        /// </summary>
        public async Task<ServiceResult<DetailProjectModel>> PacthAsync(string userId, string projectId, PatchProjectModel model)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(projectId) ||
                string.IsNullOrEmpty(model.Name) ||
                string.IsNullOrEmpty(model.Description))
            {
                return ServiceResult<DetailProjectModel>.Error("Invalid input.");
            }

            try
            {
                ServiceResult<IEnumerable<ProjectMember>> memberResult = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("UserId", userId);
                if (memberResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<DetailProjectModel>.Error(memberResult.Message);
                }

                List<ProjectMember>? ownerProjects = memberResult.Data?.Where(m => m.PositionId == _ownerPosition.Id).ToList();
                if (ownerProjects == null)
                {
                    return await PatchAction(userId, projectId, model);
                }

                bool userIsOwner = ownerProjects.Any(x => x.ProjectId == projectId);
                if (!userIsOwner)
                {
                    return ServiceResult<DetailProjectModel>.Error("Permission denied.");
                }

                List<Project> existingProjects = new List<Project>();
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
                if (isNameAvailable.Status != ResultStatus.Success || isNameAvailable.Data)
                {
                    return ServiceResult<DetailProjectModel>.Error("Duplicate project name.");
                }

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
        private async Task<ServiceResult<DetailProjectModel>> PatchAction(string userId, string projectId, PatchProjectModel model)
        {
            try
            {
                ServiceResult<Project> project = await _unitOfWork.Repository<Project>().GetOneAsync("Id", projectId);
                if (project.Status != ResultStatus.Success || project.Data == null)
                {
                    return ServiceResult<DetailProjectModel>.Error("Project not found.");
                }

                Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
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
                    [nameof(project.Data.Status)] = (TypeStatus)new StatusHandle(new DateTime(model.StartAt.Day, model.StartAt.Month, model.StartAt.Year), new DateTime(model.EndAt.Day, model.EndAt.Month, model.EndAt.Year), model.IsComplied).GetStatus()
                };

                ServiceResult<bool> patchResult = await _unitOfWork.ExecuteTransactionAsync(() =>
                    _unitOfWork.Repository<Project>().PatchAsync(project.Data, keyValuePairs));
                if (patchResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<DetailProjectModel>.Error("Update failed.");
                }

                return await GetDetailProject(projectId);
            }
            catch (Exception ex)
            {
                return ServiceResult<DetailProjectModel>.FromException(ex);
            }
        }
        #endregion
        #region Delete Project
        public async Task<ServiceResult<IEnumerable<IndexProjectModel>>> DeleteAsync(string userId, string projectId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.Error("");
            }

            if (string.IsNullOrEmpty(projectId))
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.Error("");
            }

            try
            {

                bool isCheckIsOwner = (await _unitOfWork.Repository<ProjectMember>().GetAllAsync()).Data.Any(x => x.ProjectId == projectId && x.UserId == userId && x.PositionId == _ownerPosition.Id);
                if (!isCheckIsOwner)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error("");
                }

                ServiceResult<Project> project = await _unitOfWork.Repository<Project>().GetOneAsync("Id", projectId);
                if (project.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error("");
                }
                ServiceResult<bool> deleteProjectResult = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.Repository<Project>().DeleteAsync(project.Data));

                if (deleteProjectResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error("");
                }
                return await ProjectUserIsOwner(userId);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.FromException(ex);
            }
        }
        #endregion
    }
}
