using PM.Core.Application.Interfaces;
using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos.auths;
using PM.Shared.Dtos.cores.projects;
using PM.Shared.Dtos.users;
using PM.Shared.Handle.Interfaces;

namespace PM.Core.Application.Implements
{
    public class ProjectHandle : IProjectHandle
    {
        private readonly IUnitOfWork<CoreDbContext> _unitOfWork;
        private readonly IAPIService<UserDetail> _userAPI;

        public ProjectHandle(IUnitOfWork<CoreDbContext> unitOfWork, IAPIService<UserDetail> userAPI)
        {
            _unitOfWork = unitOfWork;
            _userAPI = userAPI;
        }

        public async Task<ServiceResult<IEnumerable<IndexProjectModel>>> ProjectsUserHasJoined(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.Error("UserId cannot be null or empty.");
            }

            var result = new List<IndexProjectModel>();

            try
            {
                var projectJoined = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("UserId", userId);

                if (projectJoined.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error(projectJoined.Message);
                }

                if (projectJoined.Data == null)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Success(result);
                }

                foreach (var item in projectJoined.Data)
                {
                    var project = await _unitOfWork.Repository<Project>().GetOneAsync("Id", item.ProjectId);

                    if (project.Status != ResultStatus.Success || project.Data == null)
                    {
                        return ServiceResult<IEnumerable<IndexProjectModel>>.Error(project.Message);
                    }

                    var userInfo = await _userAPI.APIsGetAsync($"api/user/get-user?userId={item.UserId}");

                    if (userInfo.Status != ResultStatus.Success || userInfo.Data == null)
                    {
                        return ServiceResult<IEnumerable<IndexProjectModel>>.Error(userInfo.Message);
                    }

                    var userData = userInfo.Data.UserName;

                    var index = new IndexProjectModel()
                    {
                        Id = project.Data.Id,
                        Name = project.Data.Name,
                        Description = project.Data.Description,
                        CreatedBy = userData 
                    };
                    result.Add(index); 
                }

                return ServiceResult<IEnumerable<IndexProjectModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.FromException(ex);
            }
        }


    }
}
