using Microsoft.Extensions.Logging;
using PM.Domain.Entities;
using PM.Domain;
using PM.Domain.Interfaces;
using System.Data.Entity.Core.Metadata.Edm;

namespace PM.Infrastructer.Implements.Services
{
    public class ActivityLogServices
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly ILogger<ActivityLogServices> _logger;
        public ActivityLogServices(IAuthUnitOfWork authUnitOfWork, ILogger<ActivityLogServices> logger)
        {
            _authUnitOfWork = authUnitOfWork;
            _logger = logger;
        }
        public async Task<ServicesResult<bool>> AddAsync(ActivityLog log)
        {
            if (log == null)
            {
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
            try
            {
                var logs = await _authUnitOfWork.ActivityLogQueryRepository.GetManyByKeyAndValue("UserId", log.UserId);
                if (logs.Status == false)
                {
                    _logger.LogError("");
                    return ServicesResult<bool>.Failure("");
                }
                var addResponse = await _authUnitOfWork.ActivityLogCommandRepository.AddAsync(logs.Data!.ToList(), log);
                if (addResponse.Status == false)
                {
                    _logger.LogError("");
                    return ServicesResult<bool>.Failure("");
                }
                _logger.LogInformation("");
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");
            }
        }
    }
}
