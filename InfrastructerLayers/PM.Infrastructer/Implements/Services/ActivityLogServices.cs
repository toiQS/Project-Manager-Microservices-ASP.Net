using Microsoft.Extensions.Logging;
using PM.Domain.Entities;
using PM.Domain;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;

namespace PM.Infrastructure.Implements.Services
{
    public class ActivityLogServices : IActivityLogServices
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly ILogger<ActivityLogServices> _logger;

        public ActivityLogServices(IAuthUnitOfWork authUnitOfWork, ILogger<ActivityLogServices> logger)
        {
            _authUnitOfWork = authUnitOfWork;
            _logger = logger;
        }

        #region Add Activity Log
        public async Task<ServicesResult<bool>> AddAsync(ActivityLog log)
        {
            if (log == null)
            {
                _logger.LogWarning("[Service] AddAsync failed: ActivityLog object is null.");
                return ServicesResult<bool>.Failure("ActivityLog cannot be null.");
            }

            _logger.LogInformation("[Service] Adding ActivityLog for UserId={UserId}", log.UserId);
            var logs = await _authUnitOfWork.ActivityLogQueryRepository.GetManyByKeyAndValue("UserId", log.UserId);

            if (!logs.Status)
            {
                _logger.LogError("[Service] AddAsync failed: Database error while fetching logs for UserId={UserId}", log.UserId);
                return ServicesResult<bool>.Failure("Database error while fetching logs.");
            }

            var addResponse = await _authUnitOfWork.ActivityLogCommandRepository.AddAsync(logs.Data?.ToList() ?? new List<ActivityLog>(), log);

            if (!addResponse.Status)
            {
                _logger.LogError("[Service] AddAsync failed: Database error while adding ActivityLog for UserId={UserId}", log.UserId);
                return ServicesResult<bool>.Failure("Database error while adding log.");
            }

            _logger.LogInformation("[Service] Successfully added ActivityLog for UserId={UserId}", log.UserId);
            return ServicesResult<bool>.Success(true);
        }
        #endregion
    }
}
