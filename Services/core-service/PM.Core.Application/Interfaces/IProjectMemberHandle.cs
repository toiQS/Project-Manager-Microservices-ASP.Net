using PM.Shared.Dtos.cores.members.projects;
using PM.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Core.Application.Interfaces
{
    public  interface IProjectMemberHandle
    {
        Task<ServiceResult<IEnumerable<IndexProjectMemberModel>>> GetMembersInProject(string projectId);
        Task<ServiceResult<IEnumerable<IndexProjectMemberModel>>> AddAsync(string userId, AddProjectMemberModel model);
        Task<ServiceResult<IEnumerable<IndexProjectMemberModel>>> PatchAsync(string userId, string memberId, PacthProjectMemberModel model);
        Task<ServiceResult<IEnumerable<IndexProjectMemberModel>>> DeleteAsync(string userId, string memberId); 
        Task<ServiceResult<IEnumerable<IndexProjectMemberModel>>> DeteleManyAsync(string userId, string projectId);
    }
}
