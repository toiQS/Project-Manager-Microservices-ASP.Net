using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace PM.Persistence.Implements.Services
{
    public class MissionServices : IMissionServices
    {
        private readonly IProjectManagerUnitOfWork _unitOfWork;
        private readonly ILogger<MissionServices> _logger;

        public MissionServices(IProjectManagerUnitOfWork unitOfWork, ILogger<MissionServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region GET Methods
        public async Task<ServicesResult<IEnumerable<Mission>>> GetMissions()
        {
            _logger.LogInformation("[Service] Fetching all Missions...");
            var response = await _unitOfWork.MissionQueryRepository.GetAllAsync(1, 100);

            if (!response.Status)
            {
                _logger.LogError("[Service] GetMissions failed: {Message}", response.Message);
                return ServicesResult<IEnumerable<Mission>>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully fetched {Count} Missions", response.Data?.Count());
            return ServicesResult<IEnumerable<Mission>>.Success(response.Data!);
        }

        public async Task<ServicesResult<IEnumerable<Mission>>> GetMissionsInPlan(string planId)
        {
            _logger.LogInformation("[Service] Fetching Missions for PlanId={PlanId}", planId);
            var response = await _unitOfWork.MissionQueryRepository.GetManyByKeyAndValue("PlanId", planId);

            if (!response.Status)
            {
                _logger.LogError("[Service] GetMissionsInPlan failed for PlanId={PlanId}: {Message}", planId, response.Message);
                return ServicesResult<IEnumerable<Mission>>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Found {Count} missions for PlanId={PlanId}", response.Data?.Count(), planId);
            return ServicesResult<IEnumerable<Mission>>.Success(response.Data!);
        }

        public async Task<ServicesResult<Mission>> GetDetailMission(string missionId)
        {
            _logger.LogInformation("[Service] Fetching Mission details: Id={MissionId}", missionId);
            var response = await _unitOfWork.MissionQueryRepository.GetOneByKeyAndValue(
                "Id", missionId);

            if (!response.Status)
            {
                _logger.LogError("[Service] GetDetailMission failed for Id={MissionId}: {Message}", missionId, response.Message);
                return ServicesResult<Mission>.Failure(response.Message);
            }

            _logger.LogInformation("[Service] Successfully fetched Mission: Id={MissionId}", missionId);
            return ServicesResult<Mission>.Success(response.Data!);
        }
        #endregion

        #region CREATE/UPDATE Methods
        public async Task<ServicesResult<bool>> CreateMission(Mission mission)
        {
            _logger.LogInformation("[Service] Creating new Mission: PlanId={PlanId}", mission.PlanId);
            var addMissionResponse = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionCommandRepository.AddAsync(new List<Mission> { mission }, mission)
            );

            if (!addMissionResponse.Status)
            {
                _logger.LogError("[Service] CreateMission failed: {Message}", addMissionResponse.Message);
                return ServicesResult<bool>.Failure(addMissionResponse.Message);
            }

            _logger.LogInformation("[Service] Successfully created Mission: Id={MissionId}", mission.Id);
            return ServicesResult<bool>.Success(true);
        }

        public async Task<ServicesResult<bool>> UpdateMission(Mission mission)
        {
            _logger.LogInformation("[Service] Updating Mission: Id={MissionId}", mission.Id);
            var updateMissionResponse = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionCommandRepository.UpdateAsync(new List<Mission> { mission }, mission)
            );

            if (!updateMissionResponse.Status)
            {
                _logger.LogError("[Service] UpdateMission failed for Id={MissionId}: {Message}", mission.Id, updateMissionResponse.Message);
                return ServicesResult<bool>.Failure(updateMissionResponse.Message);
            }

            _logger.LogInformation("[Service] Successfully updated Mission: Id={MissionId}", mission.Id);
            return ServicesResult<bool>.Success(true);
        }

        public async Task<ServicesResult<bool>> PatchMission(string missionId, Mission mission)
        {
            _logger.LogInformation("[Service] Patching Mission: Id={MissionId}", missionId);
            var patchMissionResponse = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionCommandRepository.PatchAsync(new List<Mission> { mission }, missionId,
                    new Dictionary<string, object> { { "PlanId", mission.PlanId }, { "Name", mission.Name } })
            );

            if (!patchMissionResponse.Status)
            {
                _logger.LogError("[Service] PatchMission failed for Id={MissionId}: {Message}", missionId, patchMissionResponse.Message);
                return ServicesResult<bool>.Failure(patchMissionResponse.Message);
            }

            _logger.LogInformation("[Service] Successfully patched Mission: Id={MissionId}", missionId);
            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region DELETE Method
        public async Task<ServicesResult<bool>> DeleteMission(string missionId)
        {
            _logger.LogInformation("[Service] Deleting Mission: Id={MissionId}", missionId);
            var deleteResponse = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionCommandRepository.DeleteAsync(missionId)
            );

            if (!deleteResponse.Status)
            {
                _logger.LogError("[Service] DeleteMission failed for Id={MissionId}: {Message}", missionId, deleteResponse.Message);
                return ServicesResult<bool>.Failure(deleteResponse.Message);
            }

            _logger.LogInformation("[Service] Successfully deleted Mission: Id={MissionId}", missionId);
            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region Delete Mission Assignment In Mission
        /// <summary>
        /// Deletes all mission assignments associated with a given mission ID.
        /// </summary>
        /// <param name="missionId">The ID of the mission.</param>
        /// <returns>A ServicesResult indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> DeleteMissionAssignmentInMission(string missionId)
        {
            if (string.IsNullOrWhiteSpace(missionId))
            {
                _logger.LogError("[Service] DeleteMissionAssignmentInMission failed: Invalid missionId.");
                return ServicesResult<bool>.Failure("Invalid mission ID.");
            }

            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionAssignmentCommandRepository.DeleteManyAsync("MissionId", missionId)
            );

            if (!response.Status)
            {
                _logger.LogError("[Service] DeleteMissionAssignmentInMission failed: {ErrorMessage}", response.Message);
                return ServicesResult<bool>.Failure(response.Message ?? "Database error.");
            }

            _logger.LogInformation("[Service] Successfully deleted MissionAssignments for MissionId={MissionId}", missionId);
            return ServicesResult<bool>.Success(response.Data!);
        }
        #endregion

    }
}