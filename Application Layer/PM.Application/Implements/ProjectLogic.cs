using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Application.Interfaces;
using PM.Application.Models.projects;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.projects;
using PM.Infrastructers.Interfaces;

namespace PM.Application.Implements
{
    public class ProjectLogic : ControllerBase, IProjectLogic
    {
        private readonly IProjectServices _projectServices;
        private readonly ILogger<ProjectLogic> _logger;
        private readonly IJwtServices _jwtServices;
        public ProjectLogic(IProjectServices projectServices, ILogger<ProjectLogic> logger, IJwtServices jwtServices)
        {
            _projectServices = projectServices;
            _logger = logger;
            _jwtServices = jwtServices;
        }

        #region GetProductListUserHasJoined
        /// <summary>
        /// Retrieves the list of products that the user has joined.
        /// </summary>
        /// <param name="token">JWT token used to identify the user.</param>
        /// <returns>An IActionResult containing the list of products or an error message.</returns>

        public async Task<IActionResult> GetProductListUserHasJoined([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token is required.");

            try
            {
                var userInfoResponse = _jwtServices.ParseToken(token);
                if (!userInfoResponse.Status)
                    return BadRequest(userInfoResponse.Message);

                var projectResponse = await _projectServices.GetProductListUserHasJoined(userInfoResponse.Data.UserId);
                if (!projectResponse.Status)
                    return BadRequest(projectResponse.Message);

                return Ok(projectResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving joined products.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region GetProjectListUserHasOwner
        /// <summary>
        /// Retrieves the list of projects that the user owns.
        /// </summary>
        /// <param name="token">JWT token used to identify the user.</param>
        /// <returns>An IActionResult containing the list of owned projects or an error message.</returns>
 
        public async Task<IActionResult> GetProjectListUserHasOwner([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required.");

            try
            {
                var userInfoResponse = _jwtServices.ParseToken(token);
                if (!userInfoResponse.Status)
                    return BadRequest(userInfoResponse.Message);

                var projectResponse = await _projectServices.GetProjectListUserHasOwner(userInfoResponse.Data.UserId);
                if (!projectResponse.Status)
                    return BadRequest(projectResponse.Message);

                return Ok(projectResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving owned projects.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region GetDetailProject
        /// <summary>
        /// Retrieves detailed information about a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>An IActionResult containing the project details or an error message.</returns>
        
        public async Task<IActionResult> GetDetailProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return BadRequest("Project ID is required.");

            try
            {
                var projectResponse = await _projectServices.GetDetailProject(projectId);
                if (!projectResponse.Status)
                    return NotFound(projectResponse.Message);

                return Ok(projectResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving project details.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region AddProject
        /// <summary>
        /// Adds a new project.
        /// </summary>
        /// <param name="model">The project model containing project information and the JWT token.</param>
        /// <returns>An IActionResult containing the newly created project details or an error message.</returns>
   
        public async Task<IActionResult> AddProject([FromBody] AddProjectModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            try
            {
                var userInfo = _jwtServices.ParseToken(model.Token);
                if (!userInfo.Status)
                    return BadRequest(userInfo.Message);

                var newProject = new AddProject
                {
                    ProjectName = model.ProjectName,
                    EndAt = model.EndAt,
                    ProjectDescription = model.ProjectDescription,
                };

                var projectResponse = await _projectServices.Add(userInfo.Data.UserId, newProject);
                if (!projectResponse.Status)
                    return BadRequest(projectResponse.Message);

                return Ok(projectResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding project.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region UpdateProjectInfo
        /// <summary>
        /// Updates information about an existing project.
        /// </summary>
        /// <param name="model">The update model containing updated project info and the JWT token.</param>
        /// <returns>An IActionResult containing the updated project details or an error message.</returns>
       
        public async Task<IActionResult> UpdateProjectInfo([FromBody] UpdateProjectModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            try
            {
                var userInfo = _jwtServices.ParseToken(model.Token);
                if (!userInfo.Status)
                    return BadRequest(userInfo.Message);

                var updateProject = new UpdateProject
                {
                    EndDate = model.EndDate,
                    ProjectName = model.ProjectName,
                    ProjectDescription = model.ProjectDescription,
                    StartDate = model.StartDate,
                };

                var projectResponse = await _projectServices.UpdateInfo(userInfo.Data.UserId, model.ProjectId, updateProject);
                if (!projectResponse.Status)
                    return BadRequest(projectResponse.Message);

                return Ok(projectResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating project information.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region DeleteProject
        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="model">The model containing the project ID to delete and the JWT token.</param>
        /// <returns>An IActionResult containing the updated project list or an error message.</returns>
       
        public async Task<IActionResult> DeleteProject([FromBody] MutilProjectModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            try
            {
                var userInfo = _jwtServices.ParseToken(model.Token);
                if (!userInfo.Status)
                    return BadRequest(userInfo.Message);

                var projectResponse = await _projectServices.Delete(userInfo.Data.UserId, model.ProjectId);
                if (!projectResponse.Status)
                    return BadRequest(projectResponse.Message);

                return Ok(projectResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting project.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region UpdateIsCompleted
        /// <summary>
        /// Updates the 'completed' status of a project.
        /// </summary>
        /// <param name="model">The model containing the project ID and JWT token.</param>
        /// <returns>An IActionResult containing the updated project details or an error message.</returns>
        
        public async Task<IActionResult> UpdateIsCompleted([FromBody] MutilProjectModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            try
            {
                var userInfo = _jwtServices.ParseToken(model.Token);
                if (!userInfo.Status)
                    return BadRequest(userInfo.Message);

                var projectResponse = await _projectServices.UpdateIsCompleted(userInfo.Data.UserId, model.ProjectId);
                if (!projectResponse.Status)
                    return BadRequest(projectResponse.Message);

                return Ok(projectResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating project completion status.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region UpdateIsDelete
        /// <summary>
        /// Updates the 'deleted' status of a project.
        /// </summary>
        /// <param name="model">The model containing the project ID and JWT token.</param>
        /// <returns>An IActionResult containing the updated project details or an error message.</returns>
        
        public async Task<IActionResult> UpdateIsDelete([FromBody] MutilProjectModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            try
            {
                var userInfo = _jwtServices.ParseToken(model.Token);
                if (!userInfo.Status)
                    return BadRequest(userInfo.Message);

                var projectResponse = await _projectServices.UpdateIsDelete(userInfo.Data.UserId, model.ProjectId);
                if (!projectResponse.Status)
                    return BadRequest(projectResponse.Message);

                return Ok(projectResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating project deletion status.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion
    }
}
