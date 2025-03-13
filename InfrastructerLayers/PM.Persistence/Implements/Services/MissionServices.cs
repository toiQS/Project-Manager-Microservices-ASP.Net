using EasyNetQ.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;

namespace PM.Persistence.Implements.Services
{
    /// <summary>
    /// Service class to handle operations related to missions.
    /// </summary>
    public class MissionServices : IMissionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MissionServices> _logger;

        public MissionServices(IUnitOfWork unitOfWork, ILogger<MissionServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region GET Methods

        /// <summary>
        /// Retrieves all missions.
        /// </summary>
        public async Task<ServicesResult<IEnumerable<Mission>>> GetMissions()
        {
            var response = await _unitOfWork.MissionRepository.GetAllAsync(1, 100);
            if (!response.Status)
            {
                _logger.Error($"GetMissions failed: {response.Message}");
                return ServicesResult<IEnumerable<Mission>>.Failure(response.Message);
            }
            return ServicesResult<IEnumerable<Mission>>.Success(response.Data!);
        }

        /// <summary>
        /// Retrieves all missions belonging to a specific plan.
        /// </summary>
        public async Task<ServicesResult<IEnumerable<Mission>>> GetMissionsInPlan(string planId)
        {
            var response = await _unitOfWork.MissionRepository.GetManyByKeyAndValue(
                new Dictionary<string, string> { { "PlanId", planId } }, true, 1, 100);
            if (!response.Status)
            {
                _logger.Error($"GetMissionsInPlan failed: {response.Message}");
                return ServicesResult<IEnumerable<Mission>>.Failure(response.Message);
            }
            return ServicesResult<IEnumerable<Mission>>.Success(response.Data!);
        }

        /// <summary>
        /// Retrieves mission details by ID.
        /// </summary>
        public async Task<ServicesResult<Mission>> GetDetailMission(string missionId)
        {
            var response = await _unitOfWork.MissionRepository.GetOneByKeyAndValue(
                new Dictionary<string, string> { { "Id", missionId } }, true);
            if (!response.Status)
            {
                _logger.Error($"GetDetailMission failed: {response.Message}");
                return ServicesResult<Mission>.Failure(response.Message);
            }
            return ServicesResult<Mission>.Success(response.Data!);
        }

        #endregion

        #region CREATE/UPDATE Methods

        /// <summary>
        /// Creates a new mission.
        /// </summary>
        public async Task<ServicesResult<bool>> CreateMission(Mission mission)
        {
            var addMissionResponse = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionRepository.AddAsync(new List<Mission>(), mission)
            );

            if (!addMissionResponse.Status)
            {
                _logger.Error($"CreateMission failed: {addMissionResponse.Message}");
                return ServicesResult<bool>.Failure(addMissionResponse.Message);
            }

            return ServicesResult<bool>.Success(true);
        }

        /// <summary>
        /// Updates an existing mission.
        /// </summary>
        public async Task<ServicesResult<bool>> UpdateMission(Mission mission)
        {
            var updateMissionResponse = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionRepository.UpdateAsync(new List<Mission>(), mission)
            );

            if (!updateMissionResponse.Status)
            {
                _logger.Error($"UpdateMission failed: {updateMissionResponse.Message}");
                return ServicesResult<bool>.Failure(updateMissionResponse.Message);
            }
            return ServicesResult<bool>.Success(true);
        }

        /// <summary>
        /// Patches a mission with partial updates.
        /// </summary>
        public async Task<ServicesResult<bool>> PatchMission(string missionId, Mission mission)
        {
            var patchMissionResponse = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionRepository.PatchAsync(new List<Mission>(), missionId,
                    new Dictionary<string, object> { { "PlanId", mission.PlanId }, { "Name", mission.Name } })
            );

            if (!patchMissionResponse.Status)
            {
                _logger.Error($"PatchMission failed: {patchMissionResponse.Message}");
                return ServicesResult<bool>.Failure(patchMissionResponse.Message);
            }
            return ServicesResult<bool>.Success(true);
        }

        #endregion

        #region DELETE Method

        /// <summary>
        /// Deletes a mission by ID.
        /// </summary>
        public async Task<ServicesResult<bool>> DeleteMission(string missionId)
        {
            var deleteResponse = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionRepository.DeleteAsync(missionId)
            );

            if (!deleteResponse.Status)
            {
                _logger.Error($"DeleteMission failed: {deleteResponse.Message}");
                return ServicesResult<bool>.Failure(deleteResponse.Message);
            }
            return ServicesResult<bool>.Success(true);
        }

        #endregion
    }
}
