using System.Globalization;
using Azure.Core;
using Microsoft.Identity.Client;
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
        private string _roleCurrent;
        public ProjectServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region public method

        #region 
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsAsync()
        {
            try
            {
                var response = new List<IndexProject>();
                var projectResponse = await _unitOfWork.ProjectRepository.GetAllAsync();
                if (projectResponse.Status == false)
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(projectResponse.Message);
                }
                if (projectResponse.Data == null)
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Success(response);
                }
                foreach (var item in projectResponse.Data)
                {
                    if (item.IsDeleted == true)
                    {
                        continue;
                    }
                    var ownerMember = await GetOwnerInProject(item.Id);
                    var index = new IndexProject
                    {
                        Id = item.Id,
                        Name = item.Name,
                        OwnerName = ownerMember.Data.UserName,
                        OwnerAvata = ownerMember.Data.AvatarPath
                    };
                    response.Add(index);
                }
                return ServicesResult<IEnumerable<IndexProject>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure(ex.Message);
            }
        }
        #endregion

        #region 
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsOwnerAsync(string userId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("UserId is null or empty");
            }
            try
            {
                return await GetProjectsOfUserWithRole(userId, "Owner");
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure(ex.Message);
            }
        }
        #endregion

        #region
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsLeaderAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("UserId is null or empty");
            }
            try
            {
                return await GetProjectsOfUserWithRole(userId, "Leader");
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure(ex.Message);
            }
        }
        #endregion

        #region 
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsManagerAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("UserId is null or empty");
            }
            try
            {
                return await GetProjectsOfUserWithRole(userId, "Manager");
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure(ex.Message);
            }
        }
        #endregion

        #region 
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserIsMemberAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("UserId is null or empty");
            }
            try
            {
                return await GetProjectsOfUserWithRole(userId, "Member");
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure(ex.Message);
            }
        }
        #endregion

        #region
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsUserHasJoined(string userId)
        {
            if (!string.IsNullOrEmpty(userId)) return ServicesResult<IEnumerable<IndexProject>>.Failure("UserId is null or empty");
            try
            {
                var response = new List<IndexProject>();
                var projectJoined = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (projectJoined.Status == false)
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(projectJoined.Message);
                }
                if (projectJoined.Data == null)
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Success(response);
                }
                foreach (var item in projectJoined.Data)
                {
                    var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", item.ProjectId);
                    if (project.Status == false)
                    {
                        return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);
                    }
                    if (project.Data == null)
                    {
                        return ServicesResult<IEnumerable<IndexProject>>.Success(response);
                    }
                    if (project.Data.IsDeleted == true)
                    {
                        continue;
                    }
                    var ownerMember = await GetOwnerInProject(item.ProjectId);
                    var index = new IndexProject
                    {
                        Id = item.ProjectId,
                        Name = project.Data.Name,
                        OwnerName = ownerMember.Data.UserName,
                        OwnerAvata = ownerMember.Data.AvatarPath
                    };
                    response.Add(index);
                }
                return ServicesResult<IEnumerable<IndexProject>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure(ex.Message);
            }

        }
        #endregion

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
        private async Task<ServicesResult<User>> GetOwnerInProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return ServicesResult<User>.Failure("ProjectId is null or empty");
            }
            try
            {
                await GetOwnRole();

                var membersResponse = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (membersResponse.Status == false)
                {
                    return ServicesResult<User>.Failure(membersResponse.Message);
                }

                if (membersResponse.Data == null)
                {
                    return ServicesResult<User>.Failure("Members is null");
                }

                var ownerMember = membersResponse.Data.FirstOrDefault(x => x.RoleId == _roleCurrent);
                if (ownerMember == null)
                {
                    return ServicesResult<User>.Failure("Owner is null");
                }

                return await GetInfoUser(ownerMember.UserId);
            }
            catch (Exception ex)
            {
                return ServicesResult<User>.Failure(ex.Message);
            }
        }
        #endregion

        #region 
        private async Task<ServicesResult<User>> GetInfoUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<User>.Failure("UserId is null or empty");
            }
            try
            {
                var userResponse = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (userResponse.Status == false)
                {
                    return ServicesResult<User>.Failure(userResponse.Message);
                }
                return ServicesResult<User>.Success(userResponse.Data);
            }
            catch (Exception ex)
            {
                return ServicesResult<User>.Failure(ex.Message);
            }
        }
        #endregion

        #region 
        private async Task<ServicesResult<IEnumerable<IndexProject>>> GetProjectsOfUserWithRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("UserId is null or empty");
            }
            if (string.IsNullOrEmpty(roleName))
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("RoleName is null or empty");
            }

            if (roleName == "Owner")
            {
                await GetOwnRole();
            }
            else if (roleName == "Leader")
            {
                await GetLeaderRole();
            }
            else if (roleName == "Manager")
            {
                await GetManagerRole();
            }
            else if (roleName == "Member")
            {
                await GetMemberRole();
            }
            else
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("Invalid role name");
            }

            try
            {
                var response = new List<IndexProject>();
                var projectJoined = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (projectJoined.Status == false)
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Failure(projectJoined.Message);
                }
                if (projectJoined.Data == null)
                {
                    return ServicesResult<IEnumerable<IndexProject>>.Success(response);

                }
                foreach (var item in projectJoined.Data)
                {
                    if (item.RoleId != _roleCurrent) continue;

                    var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", item.ProjectId);
                    if (project.Status == false)
                    {
                        return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);
                    }
                    if (project.Data == null)
                    {
                        return ServicesResult<IEnumerable<IndexProject>>.Success(response);
                    }
                    if (project.Data.IsDeleted == true)
                    {
                        continue;
                    }
                    var ownerMember = await GetOwnerInProject(item.ProjectId);
                    var index = new IndexProject
                    {
                        Id = item.ProjectId,
                        Name = project.Data.Name,
                        OwnerName = ownerMember.Data.UserName,
                        OwnerAvata = ownerMember.Data.AvatarPath
                    };
                    response.Add(index);


                }
                return ServicesResult<IEnumerable<IndexProject>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure(ex.Message);
            }
        }
        #endregion

        #region Private role method helper
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
                        _roleCurrent = role.Data.Id;
                        break;
                    case "Leader":
                        _roleCurrent = role.Data.Id;
                        break;
                    case "Manager":
                        _roleCurrent = role.Data.Id;
                        break;
                    case "Member":
                        _roleCurrent = role.Data.Id;
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
