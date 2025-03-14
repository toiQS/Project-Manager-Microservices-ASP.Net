using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;

namespace PM.Persistence.Implements.Services
{
    public class RoleInProjectServices : IRoleInProjectServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoleInProjectServices> _logger;

        public RoleInProjectServices(IUnitOfWork unitOfWork, ILogger<RoleInProjectServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Get by Name
        public async Task<ServicesResult<RoleInProject>> GetByName(string name)
        {
            _logger.LogInformation("[RoleInProjectServices] Fetching role by name: {Name}", name);

            var response = await _unitOfWork.RoleInProjectQueryRepository.GetOneByKeyAndValue("Name", name);
            if (!response.Status)
            {
                _logger.LogError("[RoleInProjectServices] Failed to fetch role by name: {Name}", name);
                return ServicesResult<RoleInProject>.Failure("Role not found");
            }

            _logger.LogInformation("[RoleInProjectServices] Successfully fetched role by name: {Name}", name);
            return ServicesResult<RoleInProject>.Success(response.Data!);
        }
        #endregion

        #region Get by Id
        public async Task<ServicesResult<RoleInProject>> GetById(string id)
        {
            _logger.LogInformation("[RoleInProjectServices] Fetching role by ID: {Id}", id);

            var response = await _unitOfWork.RoleInProjectQueryRepository.GetOneByKeyAndValue("Id", id);
            if (!response.Status)
            {
                _logger.LogError("[RoleInProjectServices] Failed to fetch role by ID: {Id}", id);
                return ServicesResult<RoleInProject>.Failure("Role not found");
            }

            _logger.LogInformation("[RoleInProjectServices] Successfully fetched role by ID: {Id}", id);
            return ServicesResult<RoleInProject>.Success(response.Data!);
        }
        #endregion

        #region Predefined Roles
        public Task<ServicesResult<RoleInProject>> GetOwnerRole() => GetByName("Owner");
        public Task<ServicesResult<RoleInProject>> GetLeaderRole() => GetByName("Leader");
        public Task<ServicesResult<RoleInProject>> GetManagerRole() => GetByName("Manager");
        public Task<ServicesResult<RoleInProject>> GetMemberRole() => GetByName("Member");
        #endregion

        #region Get Other Role
        public async Task<ServicesResult<RoleInProject>> GetOtherRole(string text)
        {
            var lowerText = text.ToLower();
            if (lowerText is "owner" or "leader" or "manager" or "member")
            {
                _logger.LogError("[RoleInProjectServices] Invalid request for predefined role: {Text}", text);
                return ServicesResult<RoleInProject>.Failure("Invalid role");
            }

            var byName = await GetByName(text);
            if (byName.Status) return byName;

            var byId = await GetById(text);
            return byId.Status ? byId : ServicesResult<RoleInProject>.Failure("Role not found");
        }
        #endregion
    }
}
