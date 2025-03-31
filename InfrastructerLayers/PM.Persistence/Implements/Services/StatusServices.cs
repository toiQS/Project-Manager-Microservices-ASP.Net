using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;

namespace PM.Persistence.Implements.Services
{
    public class StatusServices : IStatusServices
    {
        private readonly IProjectManagerUnitOfWork _unitOfWork;
        private readonly ILogger<StatusServices> _logger;

        public StatusServices(IProjectManagerUnitOfWork unitOfWork, ILogger<StatusServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Get All Statuses
        public async Task<ServicesResult<IEnumerable<Status>>> GetStatusesAsync()
        {
            _logger.LogInformation("[Service] Fetching all Status records...");
            var response = await _unitOfWork.StatusQueryRepository.GetAllAsync(1, 100);

            if (!response.Status || response.Data == null)
            {
                _logger.LogError("[Service] Failed to fetch statuses from database.");
                return ServicesResult<IEnumerable<Status>>.Failure("Database error: Unable to fetch statuses.");
            }

            _logger.LogInformation("[Service] Successfully retrieved {Count} statuses.", response.Data.Count());
            return ServicesResult<IEnumerable<Status>>.Success(response.Data);
        }
        #endregion

        #region Get Status By ID
        public async Task<ServicesResult<Status>> GetStatusByIdAsync(int id)
        {
            _logger.LogInformation("[Service] Fetching Status with Id={Id}", id);
            var response = await _unitOfWork.StatusQueryRepository.GetOneByKeyAndValue("Id", id);

            if (!response.Status || response.Data == null)
            {
                _logger.LogError("[Service] No Status found for Id={Id}", id);
                return ServicesResult<Status>.Failure("Status not found.");
            }

            _logger.LogInformation("[Service] Successfully retrieved Status with Id={Id}", id);
            return ServicesResult<Status>.Success(response.Data);
        }
        #endregion

        #region Determine Status for Creation
        public int StatusForCreateAsync(DateTime startDate)
        {
            _logger.LogInformation("[Service] Determining initial status for StartDate={StartDate}", startDate);
            return DateTime.Now < startDate ? 1 : 2; // 1: Waiting, 2: In Progress
        }
        #endregion

        #region Determine Status for Update
        public int StatusForUpdateAsync(DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("[Service] Determining update status for StartDate={StartDate}, EndDate={EndDate}", startDate, endDate);
            var now = DateTime.Now;

            if (now < startDate) return 1; // Waiting
            if (now >= startDate && now < endDate) return 2; // In Progress
            if (now == endDate) return 4; // Finished On Time
            if (now > endDate) return 5; // Behind Schedule

            return 0; // Not Selected
        }
        #endregion

        #region Determine Final Status
        public int StatusForFinallyAsync(DateTime endDate)
        {
            _logger.LogInformation("[Service] Determining final status for EndDate={EndDate}", endDate);
            var now = DateTime.Now;

            if (now < endDate) return 3; // Completed Early
            if (now == endDate) return 4; // Finished On Time
            return 6; // Finished Late
        }
        #endregion
    }
}