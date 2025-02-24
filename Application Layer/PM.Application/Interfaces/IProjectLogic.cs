using Microsoft.AspNetCore.Mvc;
using PM.Application.Models.projects;

namespace PM.Application.Interfaces
{
    public interface IProjectLogic
    {
        public Task<IActionResult> GetProductListUserHasJoined([FromQuery] string token);
        public Task<IActionResult> GetProjectListUserHasOwner([FromQuery] string token);
        public Task<IActionResult> GetDetailProject(string projectId);
        public Task<IActionResult> AddProject([FromBody] AddProjectModel model);
        public Task<IActionResult> UpdateProjectInfo([FromBody] UpdateProjectModel model);
        public Task<IActionResult> DeleteProject([FromBody] MutilProjectModel model);
        public Task<IActionResult> UpdateIsCompleted([FromBody] MutilProjectModel model);
        public Task<IActionResult> UpdateIsDelete([FromBody] MutilProjectModel model);
    }
}
