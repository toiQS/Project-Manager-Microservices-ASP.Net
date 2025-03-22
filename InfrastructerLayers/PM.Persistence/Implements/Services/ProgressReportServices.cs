using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;

namespace PM.Persistence.Implements.Services
{
    public class ProgressReportServices : IProgressReportServices
    {
        private readonly IProjectManagerUnitOfWork _unitOfWork;
        private readonly ILogger<ProgressReportServices> _logger;

        public ProgressReportServices(IProjectManagerUnitOfWork unitOfWork, ILogger<ProgressReportServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region GET Methods
        public async Task<ServicesResult<IEnumerable<ProgressReport>>> GetReports()
        {
            _logger.LogInformation("[Service] Fetching all Progress Reports...");
            var response = await _unitOfWork.ProgressReportQueryRepository.GetAllAsync(1, 100);

            if (!response.Status)
            {
                _logger.LogError("[Service] GetReports failed: {Message}", response.Message);
                return ServicesResult<IEnumerable<ProgressReport>>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully fetched {Count} Progress Reports", response.Data?.Count());
            return ServicesResult<IEnumerable<ProgressReport>>.Success(response.Data!);
        }

        public async Task<ServicesResult<IEnumerable<ProgressReport>>> GetReportsInPlan(string planId)
        {
            _logger.LogInformation("[Service] Fetching Progress Reports for PlanId={PlanId}", planId);
            var response = await _unitOfWork.ProgressReportQueryRepository.GetManyByKeyAndValue("PlanId", planId);

            if (!response.Status)
            {
                _logger.LogError("[Service] GetReportsInPlan failed for PlanId={PlanId}: {Message}", planId, response.Message);
                return ServicesResult<IEnumerable<ProgressReport>>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Found {Count} Progress Reports for PlanId={PlanId}", response.Data?.Count(), planId);
            return ServicesResult<IEnumerable<ProgressReport>>.Success(response.Data!);
        }
        #endregion

        #region CREATE/UPDATE Methods
        public async Task<ServicesResult<bool>> AddAsync(ProgressReport progressReport)
        {
            _logger.LogInformation("[Service] Adding new Progress Report: PlanId={PlanId}", progressReport.PlanId);
            var reports = await GetReportsInPlan(progressReport.PlanId);
            if (!reports.Status)
            {
                _logger.LogError("[Service] AddAsync failed: {Message}", reports.Message);
                return ServicesResult<bool>.Failure(reports.Message);
            }

            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.ProgressReportCommandRepository.AddAsync(reports.Data!.ToList(), progressReport)
            );

            if (!response.Status)
            {
                _logger.LogError("[Service] AddAsync failed: {Message}", response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully added Progress Report: Id={Id}", progressReport.Id);
            return ServicesResult<bool>.Success(response.Data);
        }

        public async Task<ServicesResult<bool>> UpdateAsync(ProgressReport progressReport)
        {
            _logger.LogInformation("[Service] Updating Progress Report: Id={Id}", progressReport.Id);
            var reports = await GetReportsInPlan(progressReport.PlanId);
            if (!reports.Status)
            {
                _logger.LogError("[Service] UpdateAsync failed: {Message}", reports.Message);
                return ServicesResult<bool>.Failure(reports.Message);
            }

            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.ProgressReportCommandRepository.UpdateAsync(reports.Data!.ToList(), progressReport)
            );

            if (!response.Status)
            {
                _logger.LogError("[Service] UpdateAsync failed: {Message}", response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully updated Progress Report: Id={Id}", progressReport.Id);
            return ServicesResult<bool>.Success(response.Data);
        }

        public async Task<ServicesResult<bool>> PatchAsync(string progressReportId, ProgressReport progressReport)
        {
            _logger.LogInformation("[Service] Patching Progress Report: Id={Id}", progressReportId);
            var keyValuePairs = new Dictionary<string, object>
            {
                {"PlanId", progressReport.PlanId},
                {"ReportDetails", progressReport.ReportDetails}
            };

            var reports = await GetReportsInPlan(progressReport.PlanId);
            if (!reports.Status)
            {
                _logger.LogError("[Service] PatchAsync failed: {Message}", reports.Message);
                return ServicesResult<bool>.Failure(reports.Message);
            }

            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.ProgressReportCommandRepository.PatchAsync(reports.Data!.ToList(), progressReportId, keyValuePairs)
            );

            if (!response.Status)
            {
                _logger.LogError("[Service] PatchAsync failed: {Message}", response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully patched Progress Report: Id={Id}", progressReportId);
            return ServicesResult<bool>.Success(response.Data);
        }
        #endregion

        #region DELETE Methods
        public async Task<ServicesResult<bool>> DeleteAsync(string progressReportId)
        {
            _logger.LogInformation("[Service] Deleting Progress Report: Id={Id}", progressReportId);
            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.ProgressReportCommandRepository.DeleteAsync(progressReportId)
            );

            if (!response.Status)
            {
                _logger.LogError("[Service] DeleteAsync failed: {Message}", response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully deleted Progress Report: Id={Id}", progressReportId);
            return ServicesResult<bool>.Success(response.Data);
        }
        #endregion
    }
}
