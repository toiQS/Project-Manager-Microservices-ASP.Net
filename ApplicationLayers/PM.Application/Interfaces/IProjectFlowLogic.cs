using Microsoft.AspNetCore.Mvc;
using PM.Application.Features.Projects.Command;
using PM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Interfaces
{
    public interface IProjectFlowLogic
    {
        public Task<IActionResult> GetProductListUserHasJoined([FromQuery] string token);
        public Task<IActionResult> GetProjectListUserHasOwner([FromQuery] string token);
        public Task<IActionResult> GetDetailProject(string projectId);
        public Task<IActionResult> AddProject([FromBody] AddProjectCommand model);
        public Task<IActionResult> UpdateProjectInfo([FromBody] PacthProjectCommand command  );
        //public Task<IActionResult> DeleteProject([FromBody] MutilProjectModel model);
        //public Task<IActionResult> UpdateIsCompleted([FromBody] MutilProjectModel model);
        //public Task<IActionResult> UpdateIsDelete([FromBody] MutilProjectModel model);
    }
}
