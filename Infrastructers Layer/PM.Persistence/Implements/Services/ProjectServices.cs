using Azure.Core;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.members;
using PM.Domain.Models.plans;
using PM.Domain.Models.projects;

namespace PM.Persistence.Implements.Services
{
    public class ProjectServices
    {
        #region Constructor
        private readonly IUnitOfWork _unitOfWork;
        private string _ownerId;
        private string _leaderId;
        private string _managerId;
        private string _memberId;
        public ProjectServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region public method
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsAsync()
        {
            try
            {
                var projectResponse = await _unitOfWork.ProjectRepository.GetAllAsync();
                if (projectResponse.Status == false)
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(projectResponse.Message);
                }
                foreach (var item in projectResponse.Data)
                {
                    var ownerResponse = await GetOwnerInProject(item.Id);
                    if (ownerResponse.Status == false)
                    {
                        return ServicesResult<IEnumerable<IndexProject>>.Failure(ownerResponse.Message);
                    }
                    var owner = ownerResponse.Data;
                    var membersResponse = await GetMembersInProject(item.Id);
                    if (membersResponse.Status == false)
                    {
                        return ServicesResult<IEnumerable<IndexProject>>.Failure(membersResponse.Message);
                    }
                    var members = membersResponse.Data;

                }

            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure(ex.Message);
            }
        }
        public Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsOwnerAsync(string userId);
        public Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsLeaderAsync(string userId);
        public Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsManagerAsync(string userId);
        public Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsMemberAsync(string userId);
        public Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserHasJoined(string userId);

        #endregion

        #region  private method
        #region 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private async Task<ServicesResult<IEnumerable<ProjectMember>>> GetMembersInProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return ServicesResult<IEnumerable<ProjectMember>>.Failure("ProjectId is null or empty");
            }
            try
            {
                var membersResponse = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (membersResponse.Status == false)
                {
                    return ServicesResult<IEnumerable<ProjectMember>>.Failure(membersResponse.Message);
                }
                if (membersResponse.Data == null)
                {
                    return ServicesResult<IEnumerable<ProjectMember>>.Failure("Members is null");
                }
                return ServicesResult<IEnumerable<ProjectMember>>.Success(membersResponse.Data);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<ProjectMember>>.Failure(ex.Message);
            }
        }
        #endregion

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private async Task<ServicesResult<ProjectMember>> GetOwnerInProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return ServicesResult<ProjectMember>.Failure("ProjectId is null or empty");
            }
            try
            {
                await GetOwnRole();

                var membersResponse = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (membersResponse.Status == false)
                {
                    return ServicesResult<ProjectMember>.Failure(membersResponse.Message);
                }

                if (membersResponse.Data == null)
                {
                    return ServicesResult<ProjectMember>.Failure("Members is null");
                }

                var ownerMember = membersResponse.Data.FirstOrDefault(x => x.RoleId == _ownerId);
                if (ownerMember == null)
                {
                    return ServicesResult<ProjectMember>.Failure("Owner is null");
                }

                return ServicesResult<ProjectMember>.Success(ownerMember);
            }
            catch (Exception ex)
            {
                return ServicesResult<ProjectMember>.Failure(ex.Message);
            }
        }
        #endregion

        #region Private method helper
        /// <summary>
        /// Gets the role ID by role name.
        /// </summary>
        /// <param name="roleName">The name of the role to fetch.</param>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetRoleByName(string roleName)
        {
            try
            {
                var role = await _unitOfWork.RoleInProjectRepository.GetOneByKeyAndValue("Name", roleName);
                if (!role.Status)
                    return ServicesResult<bool>.Failure(role.Message);

                // Assign the role ID to the appropriate variable
                switch (roleName)
                {
                    case "Owner":
                        _ownerId = role.Data.Id;
                        break;
                    case "Leader":
                        _leaderId = role.Data.Id;
                        break;
                    case "Manager":
                        _managerId = role.Data.Id;
                        break;
                    case "Member":
                        _memberId = role.Data.Id;
                        break;
                    default:
                        return ServicesResult<bool>.Failure("Invalid role name");
                }

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure(ex.Message);
            }

        }

        /// <summary>
        /// Gets the role ID for the "Owner" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetOwnRole()
        {
            return await GetRoleByName("Owner");
        }

        /// <summary>
        /// Gets the role ID for the "Leader" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetLeaderRole()
        {
            return await GetRoleByName("Leader");
        }

        /// <summary>
        /// Gets the role ID for the "Manager" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetManagerRole()
        {
            return await GetRoleByName("Manager");
        }

        /// <summary>
        /// Gets the role ID for the "Member" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetMemberRole()
        {
            return await GetRoleByName("Member");
        }


        #endregion
        #endregion
    }
}
