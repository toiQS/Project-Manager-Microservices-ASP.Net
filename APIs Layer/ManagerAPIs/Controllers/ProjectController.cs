using Microsoft.AspNetCore.Mvc;
using PM.Application.Models.projects;
using PM.Application.Interfaces;

namespace ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectLogic _projectLogic;

        public ProjectController(IProjectLogic projectLogic)
        {
            _projectLogic = projectLogic;
        }

        /// <summary>
        /// Retrieves the list of products that the user has joined.
        /// </summary>
        /// <param name="token">JWT token used to identify the user.</param>
        /// <returns>An IActionResult containing the list of products or an error message.</returns>
        [HttpGet("joined")]
        public async Task<IActionResult> GetProductListUserHasJoined([FromQuery] string token)
        {
            return await _projectLogic.GetProductListUserHasJoined(token);
        }

        /// <summary>
        /// Retrieves the list of projects that the user owns.
        /// </summary>
        /// <param name="token">JWT token used to identify the user.</param>
        /// <returns>An IActionResult containing the list of owned projects or an error message.</returns>
        [HttpGet("owned")]
        public async Task<IActionResult> GetProjectListUserHasOwner([FromQuery] string token)
        {
            return await _projectLogic.GetProjectListUserHasOwner(token);
        }

        /// <summary>
        /// Retrieves detailed information about a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>An IActionResult containing the project details or an error message.</returns>
        [HttpGet("get-detail-project")]
        public async Task<IActionResult> GetDetailProject(string projectId)
        {
            return await _projectLogic.GetDetailProject(projectId);
        }

        /// <summary>
        /// Adds a new project.
        /// </summary>
        /// <param name="model">The model containing project information and the JWT token.</param>
        /// <returns>An IActionResult containing the newly created project details or an error message.</returns>
        [HttpPost("add-project")]
        public async Task<IActionResult> AddProject([FromBody] AddProjectModel model)
        {
            return await _projectLogic.AddProject(model);
        }

        /// <summary>
        /// Updates information about an existing project.
        /// </summary>
        /// <param name="model">The model containing updated project info and the JWT token.</param>
        /// <returns>An IActionResult containing the updated project details or an error message.</returns>
        [HttpPut("update-project")]
        public async Task<IActionResult> UpdateProjectInfo([FromBody] UpdateProjectModel model)
        {
            return await _projectLogic.UpdateProjectInfo(model);
        }

        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="model">The model containing the project ID to delete and the JWT token.</param>
        /// <returns>An IActionResult containing the updated project list or an error message.</returns>
        [HttpDelete("delete-project")]
        public async Task<IActionResult> DeleteProject([FromBody] MutilProjectModel model)
        {
            return await _projectLogic.DeleteProject(model);
        }

        /// <summary>
        /// Updates the 'completed' status of a project.
        /// </summary>
        /// <param name="model">The model containing the project ID and JWT token.</param>
        /// <returns>An IActionResult containing the updated project details or an error message.</returns>
        [HttpPut("complete")]
        public async Task<IActionResult> UpdateIsCompleted([FromBody] MutilProjectModel model)
        {
            return await _projectLogic.UpdateIsCompleted(model);
        }

        /// <summary>
        /// Updates the 'deleted' status of a project.
        /// </summary>
        /// <param name="model">The model containing the project ID and JWT token.</param>
        /// <returns>An IActionResult containing the updated project details or an error message.</returns>
        [HttpPut("delete-status")]
        public async Task<IActionResult> UpdateIsDelete([FromBody] MutilProjectModel model)
        {
            return await _projectLogic.UpdateIsDelete(model);
        }
    }
}
