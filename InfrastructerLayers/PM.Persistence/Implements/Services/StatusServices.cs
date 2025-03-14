using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;

namespace PM.Persistence.Implements.Services
{
    public class StatusServices : IStatusServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StatusServices> _logger;
        public StatusServices(IUnitOfWork unitOfWork, ILogger<StatusServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region
        public async Task<ServicesResult<IEnumerable<Status>>> GetStatusesAsync()
        {
            var response = await _unitOfWork.StatusQueryRepository.GetAllAsync(1, 100);
            if (!response.Status || response.Data == null)
            {
                _logger.LogError("[StatusServices] Failed to get statuses");
                return ServicesResult<IEnumerable<Status>>.Failure("Failed to fetch statuses");
            }
            _logger.LogInformation("[StatusServices] Retrieved statuses successfully");
            return ServicesResult<IEnumerable<Status>>.Success(response.Data);
        }
        #endregion

        #region
        public async Task<ServicesResult<Status>> GetStatusByIdAsync(int id)
        {
            var response = await _unitOfWork.StatusQueryRepository.GetOneByKeyAndValue("Id", id);
            if (!response.Status)
            {
                _logger.LogError("[StatusServices] Failed to get status with ID: {Id}", id);
                return ServicesResult<Status>.Failure("Failed to fetch status");
            }
            _logger.LogInformation("[StatusServices] Retrieved status with ID: {Id}", id);
            return ServicesResult<Status>.Success(response.Data!);
        }
        #endregion

        #region
        public int StatusForCreateAsync(DateTime startDate)
        {
            return DateTime.Now < startDate ? 1 : 2; // Waiting or In Progress
        }
        #endregion

        #region
        public int StatusForUpdateAsync(DateTime startDate, DateTime endDate)
        {
            var now = DateTime.Now;
            if (now < startDate) return 1; // Waiting
            if (now >= startDate && now < endDate) return 2; // In Progress
            if (now == endDate) return 4; // Finished On Time
            if (now > endDate) return 5; // Behind Schedule
            return 0; // Not Selected
        }
        #endregion

        #region
        public int StatusForFinallyAsync(DateTime endDate)
        {
            var now = DateTime.Now;
            if (now < endDate) return 3; // Completed Early
            if (now == endDate) return 4; // Finished On Time
            return 6; // Finished Late
        }
        #endregion
    }
}
