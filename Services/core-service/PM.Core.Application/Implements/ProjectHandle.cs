using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos.auths;
using PM.Shared.Dtos.cores.projects;
using PM.Shared.Dtos.users;
using PM.Shared.Handle.Interfaces;
using System.Diagnostics;

namespace PM.Core.Application.Implements
{
    public class ProjectHandle
    {
        private readonly IUnitOfWork<CoreDbContext> _unitOfWork;
        private readonly IAPIService<UserDetail> _userAPI;


        public async Task<ServiceResult<IEnumerable<IndexProjectModel>>> ProjectsUserHasJoined(string userId)
        {
            // Check if the userId is null or empty
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<IEnumerable<IndexProjectModel>>.Error("UserId cannot be null or empty.");
            }

            // Initialize an empty list to store the project details
            var result = new List<IndexProjectModel>();

            try
            {
                // Fetch the list of projects the user has joined based on the userId
                var projectJoined = await _unitOfWork.Repository<ProjectMember>().GetManyAsync("UserId", userId);

                // Handle if fetching the project members fails
                if (projectJoined.Status != ResultStatus.Success)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Error(projectJoined.Message);
                }

                // If the user has not joined any projects, return an empty list
                if (projectJoined.Data == null)
                {
                    return ServiceResult<IEnumerable<IndexProjectModel>>.Success(result);
                }

                // Loop through each project the user has joined
                foreach (var item in projectJoined.Data)
                {
                    // Fetch the project details based on the projectId from ProjectMember
                    var project = await _unitOfWork.Repository<Project>().GetOneAsync("Id", item.ProjectId);

                    // Handle if fetching the project details fails
                    if (project.Status != ResultStatus.Success || project.Data == null)
                    {
                        return ServiceResult<IEnumerable<IndexProjectModel>>.Error(project.Message);
                    }

                    // Fetch the user info (username) for the user who created the project
                    var userInfo = await _userAPI.APIsGetAsync($"api/user/get-user?userId={item.UserId}");

                    // Handle if fetching the user info fails
                    if (userInfo.Status != ResultStatus.Success || userInfo.Data == null)
                    {
                        return ServiceResult<IEnumerable<IndexProjectModel>>.Error(userInfo.Message);
                    }

                    // Extract the username of the project creator
                    var userData = userInfo.Data.UserName;

                    // Map the project and user data to an IndexProjectModel and add to the result list
                    var index = new IndexProjectModel()
                    {
                        Id = project.Data.Id,
                        Name = project.Data.Name,
                        Description = project.Data.Description,
                        CreatedBy = userData // Set the username of the project creator
                    };
                    result.Add(index); // Add the mapped project data to the result list
                }

                // Return the list of projects the user has joined
                return ServiceResult<IEnumerable<IndexProjectModel>>.Success(result);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return the error result
                return ServiceResult<IEnumerable<IndexProjectModel>>.FromException(ex);
            }
        }


    }
}
