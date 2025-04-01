using Microsoft.AspNetCore.Mvc;
using PM.Application.DTOs.Project;
using PM.Application.Features.Projects.Command;
using PM.Domain;

namespace PM.Application.Contracts.Interfaces
{
    public interface IProjectFlowLogic
    {
        public Task<ServicesResult<IEnumerable<ProjectIndexDTO>>> GetProjectsUserHasJoined([FromQuery] string userId);
        public Task<ServicesResult<IEnumerable<ProjectIndexDTO>>> GetProjectsUserHasOwner([FromQuery] string userId);
        public Task<ServicesResult<ProjectDetailDTO>> GetDetailProject(string projectId);
        public Task<ServicesResult<ProjectDetailDTO>> AddProject([FromBody] AddProjectCommand model);
        public Task<ServicesResult<ProjectDetailDTO>> PatchProject(PatchProjectCommand command);
    }
}
