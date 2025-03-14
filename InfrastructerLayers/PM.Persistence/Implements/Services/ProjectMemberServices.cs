using Azure.Core;
using EasyNetQ.Events;
using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;

namespace PM.Persistence.Implements.Services
{
    public class ProjectMemberServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private ILogger<ProjectMemberServices> _logger;
        public ProjectMemberServices(IUnitOfWork unitOfWork, ILogger<ProjectMemberServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #region
        public async Task<ServicesResult<IEnumerable<ProjectMember>>> GetMembers()
        {
            var response = await _unitOfWork.ProjectMemberQueryRepository.GetAllAsync(1, 100);
            if (response.Status == false)
            {
                _logger.LogError("");
                return ServicesResult<IEnumerable<ProjectMember>>.Failure("");
            }
            _logger.LogInformation("");
            return ServicesResult<IEnumerable<ProjectMember>>.Success(response.Data!.ToList());
        }
        #endregion
        #region
        public async Task<ServicesResult<IEnumerable<ProjectMember>>> GetMembersInProject(string projectId)
        {
            var response = await _unitOfWork.ProjectMemberQueryRepository.GetManyByKeyAndValue("ProjectId", projectId);
            if (response.Status == false)
            {
                _logger.LogError("");
                return ServicesResult<IEnumerable<ProjectMember>>.Failure("");
            }
            if (response.Data == null)
            {
                _logger.LogError("");
                return ServicesResult<IEnumerable<ProjectMember>>.Failure("");
            }
            _logger.LogInformation("");
            return ServicesResult<IEnumerable<ProjectMember>>.Success(response.Data);
        }
        #endregion
        #region
        public async Task<ServicesResult<ProjectMember>> GetDetailMember(string memberId)
        {
            var response = await _unitOfWork.ProjectMemberQueryRepository.GetOneByKeyAndValue("Id", memberId);
            if (response.Status == false)
            {
                _logger.LogError("");
                return ServicesResult<ProjectMember>.Failure("");
            }
            _logger.LogInformation("");
            return ServicesResult<ProjectMember>.Success(response.Data!);
        }
        #endregion
        #region
        public async Task<ServicesResult<bool>> AddMember(ProjectMember member)
        {
            var membersResponse = await GetMembersInProject(projectId: member.ProjectId);
            if (membersResponse.Status == false)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");
            }
            var response = await _unitOfWork.ExecuteTransactionAsync(async() => await _unitOfWork.ProjectMemberCommandRepository.AddAsync(membersResponse.Data!.ToList(),member));
            if (response.Status == false)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");
            }
            _logger.LogInformation("");
            return ServicesResult<bool>.Success(response.Data);
        }
        #endregion
        #region

        public async Task<ServicesResult<bool>> UpdateMember(ProjectMember member)
        {
            var membersResponse = await GetMembersInProject(projectId: member.ProjectId);
            if (membersResponse.Status == false)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");
            }
            var response = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.ProjectMemberCommandRepository.UpdateAsync(membersResponse.Data!.ToList(), member));
            if (response.Status == false)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");
            }
            _logger.LogInformation("");
            return ServicesResult<bool>.Success(response.Data);
        }
        #endregion
        #region
        public async Task<ServicesResult<bool>> PatchMember(string memberId, ProjectMember member)
        {
            var membersResponse = await GetMembersInProject(projectId: member.ProjectId);
            if (membersResponse.Status == false)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");
            }
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>()
            {
                {"ProjectId",member.ProjectId },
                {"UserId", member.UserId},
                {"RoleId", member.RoleId},
                {"PositionWork",member.PositionWork},
                {"RoleInProject", member.RoleInProject }
            };
            var response = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.ProjectMemberCommandRepository.PatchAsync(membersResponse.Data!.ToList(), memberId,keyValuePairs));
            if (response.Status == false)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");
            }
            _logger.LogInformation("");
            return ServicesResult<bool>.Success(response.Data);
        }
        #endregion
        #region
        public async Task<ServicesResult<bool>> DeleteMember(string memberId)
        {
            var response = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.ProjectMemberCommandRepository.DeleteAsync(memberId));
            if (response.Status == false)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");
            }
            _logger.LogInformation("");
            return ServicesResult<bool>.Success(response.Data);
        }
        #endregion
        #region
        public async Task<ServicesResult<bool>> DeteleMembersInProject(string projectId)
        {
            
            var response = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.ProjectMemberCommandRepository.DeleteManyAsync("ProjectId", projectId));
            if (response.Status == false)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");
            }
            _logger.LogInformation("");
            return ServicesResult<bool>.Success(response.Data);
        }
        #endregion
    }
}
