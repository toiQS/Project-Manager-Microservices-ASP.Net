using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;

namespace PM.Persistence.Implements.Services
{
    public class ProjectMemberServices : IProjectMemberServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProjectMemberServices> _logger;

        public ProjectMemberServices(IUnitOfWork unitOfWork, ILogger<ProjectMemberServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region GET Methods
        public async Task<ServicesResult<IEnumerable<ProjectMember>>> GetMembers()
        {
            _logger.LogInformation("[Service] Fetching all ProjectMembers...");
            var response = await _unitOfWork.ProjectMemberQueryRepository.GetAllAsync(1, 100);

            if (!response.Status)
            {
                _logger.LogError("[Service] GetMembers failed: {Message}", response.Message);
                return ServicesResult<IEnumerable<ProjectMember>>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully fetched {Count} ProjectMembers", response.Data?.Count());
            return ServicesResult<IEnumerable<ProjectMember>>.Success(response.Data!.ToList());
        }

        public async Task<ServicesResult<IEnumerable<ProjectMember>>> GetProjectMembersInProject(string projectId)
        {
            _logger.LogInformation("[Service] Fetching ProjectMembers for ProjectId={ProjectId}", projectId);
            var response = await _unitOfWork.ProjectMemberQueryRepository.GetManyByKeyAndValue("ProjectId", projectId);

            if (!response.Status || response.Data == null)
            {
                _logger.LogError("[Service] GetProjectMembersInProject failed for ProjectId={ProjectId}: {Message}", projectId, response.Message);
                return ServicesResult<IEnumerable<ProjectMember>>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Found {Count} members for ProjectId={ProjectId}", response.Data.Count(), projectId);
            return ServicesResult<IEnumerable<ProjectMember>>.Success(response.Data);
        }

        public async Task<ServicesResult<IEnumerable<ProjectMember>>> GetMemberOtherRolesIsNotOwner(string projectId, string roleId)
        {
            _logger.LogInformation("[Service] Fetching ProjectMembers with RoleId != {RoleId} for ProjectId={ProjectId}", roleId, projectId);
            var valuePairs = new Dictionary<string, string> { { "ProjectId", projectId }, { "RoleId", roleId } };
            var response = await _unitOfWork.ProjectMemberQueryRepository.GetManyByKeyAndValue(valuePairs, true, 1, 100);

            if (!response.Status || response.Data == null)
            {
                _logger.LogError("[Service] GetMemberOtherRolesIsNotOwner failed for ProjectId={ProjectId}: {Message}", projectId, response.Message);
                return ServicesResult<IEnumerable<ProjectMember>>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Found {Count} members for ProjectId={ProjectId}", response.Data.Count(), projectId);
            return ServicesResult<IEnumerable<ProjectMember>>.Success(response.Data);
        }

        public async Task<ServicesResult<ProjectMember>> GetOwnerProject(string projectId, string roleId)
        {
            var valuePairs = new Dictionary<string, string> { { "ProjectId", projectId }, { "RoleId", roleId } };
            var response = await _unitOfWork.ProjectMemberQueryRepository.GetOneByKeyAndValue(valuePairs, true);

            if (!response.Status)
            {
                _logger.LogError("[Service] GetOwnerProject failed for ProjectId={ProjectId}: {Message}", projectId, response.Message);
                return ServicesResult<ProjectMember>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully fetched ProjectMember: Id={MemberId}", response.Data?.Id);
            return ServicesResult<ProjectMember>.Success(response.Data!);
        }

        public async Task<ServicesResult<ProjectMember>> GetDetailMember(string memberId)
        {
            _logger.LogInformation("[Service] Fetching ProjectMember details: Id={MemberId}", memberId);
            var response = await _unitOfWork.ProjectMemberQueryRepository.GetOneByKeyAndValue("Id", memberId);

            if (!response.Status)
            {
                _logger.LogError("[Service] GetDetailMember failed for Id={MemberId}: {Message}", memberId, response.Message);
                return ServicesResult<ProjectMember>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully fetched ProjectMember: Id={MemberId}", memberId);
            return ServicesResult<ProjectMember>.Success(response.Data!);
        }
        #endregion

        #region CREATE/UPDATE Methods
        public async Task<ServicesResult<bool>> AddMember(ProjectMember member)
        {
            _logger.LogInformation("[Service] Adding ProjectMember: UserId={UserId}, ProjectId={ProjectId}", member.UserId, member.ProjectId);
            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.ProjectMemberCommandRepository.AddAsync(new List<ProjectMember> { member }, member));

            return response.Status ? ServicesResult<bool>.Success(true) : ServicesResult<bool>.Failure(response.Message);
        }

        public async Task<ServicesResult<bool>> UpdateMember(ProjectMember member)
        {
            _logger.LogInformation("[Service] Updating ProjectMember: Id={MemberId}", member.Id);
            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.ProjectMemberCommandRepository.UpdateAsync(new List<ProjectMember> { member }, member));

            return response.Status ? ServicesResult<bool>.Success(true) : ServicesResult<bool>.Failure(response.Message);
        }

        public async Task<ServicesResult<bool>> PatchMember(string memberId, ProjectMember member)
        {
            _logger.LogInformation("[Service] Patching ProjectMember: Id={MemberId}", memberId);
            var keyValuePairs = new Dictionary<string, object> {
                { "ProjectId", member.ProjectId },
                { "UserId", member.UserId },
                { "RoleId", member.RoleId },
                { "PositionWork", member.PositionWork },
                { "RoleInProject", member.RoleInProject }
            };

            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.ProjectMemberCommandRepository.PatchAsync(new List<ProjectMember> { member }, memberId, keyValuePairs));

            return response.Status ? ServicesResult<bool>.Success(true) : ServicesResult<bool>.Failure(response.Message);
        }
        #endregion

        #region DELETE Methods
        public async Task<ServicesResult<bool>> DeleteMember(string memberId)
        {
            _logger.LogInformation("[Service] Deleting ProjectMember: Id={MemberId}", memberId);
            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.ProjectMemberCommandRepository.DeleteAsync(memberId));

            return response.Status ? ServicesResult<bool>.Success(true) : ServicesResult<bool>.Failure(response.Message);
        }

        public async Task<ServicesResult<bool>> DeleteMembersInProject(string projectId)
        {
            _logger.LogInformation("[Service] Deleting all ProjectMembers in ProjectId={ProjectId}", projectId);
            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.ProjectMemberCommandRepository.DeleteManyAsync("ProjectId", projectId));

            return response.Status ? ServicesResult<bool>.Success(true) : ServicesResult<bool>.Failure(response.Message);
        }
        #endregion
    }
}
