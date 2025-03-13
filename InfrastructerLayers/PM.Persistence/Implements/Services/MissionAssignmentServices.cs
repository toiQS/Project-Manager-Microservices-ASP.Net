using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace PM.Persistence.Implements.Services
{
    public class MissionAssignmentServices
    {
        private readonly ILogger<MissionAssignmentServices> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public MissionAssignmentServices(ILogger<MissionAssignmentServices> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #region Get All Assignments
        public async Task<ServicesResult<IEnumerable<MissionAssignment>>> GetMissionAssignmentsAsync()
        {
            _logger.LogInformation("[Service] Fetching all MissionAssignments...");
            var response = await _unitOfWork.MissionAssignmentQueryRepository.GetAllAsync(1, 100);

            if (!response.Status)
            {
                _logger.LogError("[Service] GetMissionAssignmentsAsync failed: Database fetch error.");
                return ServicesResult<IEnumerable<MissionAssignment>>.Failure("Database error.");
            }

            _logger.LogInformation("[Service] Successfully fetched {Count} MissionAssignments.", response.Data?.Count());
            return ServicesResult<IEnumerable<MissionAssignment>>.Success(response.Data!);
        }
        #endregion

        #region Get Assignments By Mission
        public async Task<ServicesResult<IEnumerable<MissionAssignment>>> GetMissionAssignmentsInMissionAsync(string missionId)
        {
            _logger.LogInformation("[Service] Fetching MissionAssignments for MissionId={MissionId}", missionId);
            var missionResponse = await _unitOfWork.MissionAssignmentQueryRepository.GetManyByKeyAndValue("MissionId", missionId);

            if (!missionResponse.Status)
            {
                _logger.LogError("[Service] No assignments found for MissionId={MissionId}", missionId);
                return ServicesResult<IEnumerable<MissionAssignment>>.Failure("No mission assignments found.");
            }

            _logger.LogInformation("[Service] Found {Count} assignments for MissionId={MissionId}", missionResponse.Data?.Count(), missionId);
            return ServicesResult<IEnumerable<MissionAssignment>>.Success(missionResponse.Data!);
        }
        #endregion

        #region Get Assignment By ID
        public async Task<ServicesResult<MissionAssignment>> GetMissionAssignmentAsync(string missionAssignmentId)
        {
            _logger.LogInformation("[Service] Fetching MissionAssignment: Id={AssignmentId}", missionAssignmentId);
            var missionResponse = await _unitOfWork.MissionAssignmentQueryRepository.GetOneByKeyAndValue("Id", missionAssignmentId);

            if (!missionResponse.Status)
            {
                _logger.LogError("[Service] No MissionAssignment found for Id={AssignmentId}", missionAssignmentId);
                return ServicesResult<MissionAssignment>.Failure("Mission assignment not found.");
            }

            _logger.LogInformation("[Service] Successfully fetched MissionAssignment: Id={AssignmentId}", missionAssignmentId);
            return ServicesResult<MissionAssignment>.Success(missionResponse.Data!);
        }
        #endregion

        #region Add Assignment
        public async Task<ServicesResult<bool>> AddAsync(MissionAssignment missionAssignment)
        {
            _logger.LogInformation("[Service] Adding new MissionAssignment: MissionId={MissionId}", missionAssignment.MissionId);

            var addResponse = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionAssignmentCommandRepository.AddAsync(new List<MissionAssignment> { missionAssignment }, missionAssignment)
            );

            if (!addResponse.Status)
            {
                _logger.LogError("[Service] AddAsync failed: Database error for MissionId={MissionId}", missionAssignment.MissionId);
                return ServicesResult<bool>.Failure("Database error.");
            }

            _logger.LogInformation("[Service] Successfully added MissionAssignment: Id={AssignmentId}", missionAssignment.Id);
            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region Remove Assignment
        public async Task<ServicesResult<bool>> RemoveAsync(string missionAssignmentId)
        {
            _logger.LogInformation("[Service] Removing MissionAssignment: Id={AssignmentId}", missionAssignmentId);

            var removeResponse = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionAssignmentCommandRepository.DeleteAsync(missionAssignmentId)
            );

            if (!removeResponse.Status)
            {
                _logger.LogError("[Service] RemoveAsync failed: Database error for AssignmentId={AssignmentIdMi}", missionAssignmentId);
                return ServicesResult<bool>.Failure("Database error.");
            }

            _logger.LogInformation("[Service] Successfully removed MissionAssignment: Id={AssignmentId}", missionAssignmentId);
            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region Update Assignment
        public async Task<ServicesResult<bool>> UpdateAsync(MissionAssignment missionAssignment)
        {
            _logger.LogInformation("[Service] Updating MissionAssignment: Id={AssignmentId}", missionAssignment.Id);

            var updateResponse = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionAssignmentCommandRepository.UpdateAsync(new List<MissionAssignment> { missionAssignment }, missionAssignment)
            );

            if (!updateResponse.Status)
            {
                _logger.LogError("[Service] UpdateAsync failed: Database error for AssignmentId={AssignmentId}", missionAssignment.Id);
                return ServicesResult<bool>.Failure("Database error.");
            }

            _logger.LogInformation("[Service] Successfully updated MissionAssignment: Id={AssignmentId}", missionAssignment.Id);
            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region Patch Assignment
        public async Task<ServicesResult<bool>> PatchAsync(string missionId, MissionAssignment missionAssignment)
        {
            _logger.LogInformation("[Service] Patching MissionAssignment: MissionId={MissionId}", missionId);

            var updateResponse = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.MissionAssignmentCommandRepository.PatchAsync(
                    new List<MissionAssignment> { missionAssignment }, missionId,
                    new Dictionary<string, object> { { "MissionId", missionAssignment.MissionId }, { "ProjectMemberId", missionAssignment.ProjectMemberId } }
                )
            );

            if (!updateResponse.Status)
            {
                _logger.LogError("[Service] PatchAsync failed: Database error for MissionId={MissionId}", missionId);
                return ServicesResult<bool>.Failure("Database error.");
            }

            _logger.LogInformation("[Service] Successfully patched MissionAssignment: MissionId={MissionId}", missionId);
            return ServicesResult<bool>.Success(true);
        }
        #endregion
    }
}
