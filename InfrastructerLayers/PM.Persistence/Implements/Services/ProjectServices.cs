using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;

namespace PM.Persistence.Implements.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProjectServices> _logger;
        public ProjectServices(IUnitOfWork unitOfWork, ILogger<ProjectServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #region

        public async Task<ServicesResult<IEnumerable<Project>>> GetProjectsAsync()
        {
            var response = await _unitOfWork.ProjectQueryRepository.GetAllAsync(1, 100);
            if (response.Status == false)
            {
                _logger.LogError("[ProjectServices] Failed to retrieve projects");
                return ServicesResult<IEnumerable<Project>>.Failure("Failed to retrieve projects");
            }

            _logger.LogInformation("[ProjectServices] Successfully retrieved projects");
            return ServicesResult<IEnumerable<Project>>.Success(response.Data!);
        }
        #endregion
        #region
        public async Task<ServicesResult<Project>> GetProjectAsync(string projectId)
        {
            var response = await _unitOfWork.ProjectQueryRepository.GetOneByKeyAndValue("Id", projectId);
            if (response.Status == false)
            {
                _logger.LogError("[ProjectServices] Failed to retrieve project with Id: {ProjectId}", projectId);
                return ServicesResult<Project>.Failure("Failed to retrieve project");
            }

            _logger.LogInformation("[ProjectServices] Successfully retrieved project with Id: {ProjectId}", projectId);
            return ServicesResult<Project>.Success(response.Data!);
        }
        #endregion
        #region
        public async Task<ServicesResult<bool>> CreateProjectAsync(IEnumerable<Project> arr, Project project)
        {
            var response = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.ProjectCommandRepository.AddAsync(arr.ToList(), project));
            if (response.Status == false)
            {
                _logger.LogError("[ProjectServices] Failed to create project");
                return ServicesResult<bool>.Failure("Failed to create project");
            }
            _logger.LogInformation("[ProjectServices] Successfully created project");
            return ServicesResult<bool>.Success(response.Data!);
        }
        #endregion
        #region
        public async Task<ServicesResult<bool>> UpdateProjectAsync(IEnumerable<Project> arr, Project project)
        {
            var response = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.ProjectCommandRepository.UpdateAsync(arr.ToList(), project));
            if (response.Status == false)
            {
                _logger.LogError("[ProjectServices] Failed to update project");
                return ServicesResult<bool>.Failure("Failed to update project");
            }
            _logger.LogInformation("[ProjectServices] Successfully updated project");
            return ServicesResult<bool>.Success(response.Data!);
        }
        public async Task<ServicesResult<bool>> PatchProjectAsync(IEnumerable<Project> arr, string projectId, Project project)
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>()
            {
                {"Name", project.Name },
                {"Description", project.Description },
                {"IsComplete", project.IsCompleted },
                {"IsDelete", project.IsDeleted },
                {"StartDate", project.StartDate },
                {"EndDate",project.EndDate },
                {"StatusId", project.StatusId},
            };
            var response = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.ProjectCommandRepository.PatchAsync(arr.ToList(), projectId, keyValuePairs));
            if (response.Status == false)
            {
                _logger.LogError("[ProjectServices] Failed to patch project with Id: {ProjectId}", projectId);
                return ServicesResult<bool>.Failure("Failed to patch project");
            }
            _logger.LogInformation("[ProjectServices] Successfully patched project with Id: {ProjectId}", projectId);
            return ServicesResult<bool>.Success(response.Data!);
        }
        public async Task<ServicesResult<bool>> DeleteProjectAsync(string projectId)
        {
            var response = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.ProjectCommandRepository.DeleteAsync(projectId));
            if (response.Status == false)
            {
                _logger.LogError("[ProjectServices] Failed to delete project with Id: {ProjectId}", projectId);
                return ServicesResult<bool>.Failure("Failed to delete project");
            }
            _logger.LogInformation("[ProjectServices] Successfully deleted project with Id: {ProjectId}", projectId);
            return ServicesResult<bool>.Success(response.Data!);
        }
        #endregion
    }
}
