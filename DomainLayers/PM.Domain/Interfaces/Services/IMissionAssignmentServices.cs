using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing mission assignments.
    /// </summary>
    public interface IMissionAssignmentServices
    {
        /// <summary>
        /// Retrieves all mission assignments.
        /// </summary>
        /// <returns>A collection of mission assignments.</returns>
        public Task<ServicesResult<IEnumerable<MissionAssignment>>> GetMissionAssignmentsAsync();

        /// <summary>
        /// Retrieves mission assignments associated with a specific mission.
        /// </summary>
        /// <param name="missionId">The unique identifier of the mission.</param>
        /// <returns>A collection of mission assignments for the specified mission.</returns>
        public Task<ServicesResult<IEnumerable<MissionAssignment>>> GetMissionAssignmentsInMissionAsync(string missionId);

        /// <summary>
        /// Retrieves a specific mission assignment by its unique identifier.
        /// </summary>
        /// <param name="missionAssistanceId">The unique identifier of the mission assignment.</param>
        /// <returns>The mission assignment associated with the provided ID.</returns>
        public Task<ServicesResult<MissionAssignment>> GetMissionAssignmentAsync(string missionAssistanceId);

        /// <summary>
        /// Adds a new mission assignment.
        /// </summary>
        /// <param name="missionAssignment">The mission assignment to add.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> AddAsync(MissionAssignment missionAssignment);

        /// <summary>
        /// Removes an existing mission assignment.
        /// </summary>
        /// <param name="missionAssignmentId">The unique identifier of the mission assignment to remove.</param>
        /// <returns>True if the removal was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> RemoveAsync(string missionAssignmentId);

        /// <summary>
        /// Updates an existing mission assignment.
        /// </summary>
        /// <param name="missionAssignment">The updated mission assignment.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> UpdateAsync(MissionAssignment missionAssignment);

        /// <summary>
        /// Applies partial updates to a mission assignment.
        /// </summary>
        /// <param name="missionId">The unique identifier of the mission.</param>
        /// <param name="missionAssignment">The mission assignment with updates to apply.</param>
        /// <returns>True if the patch was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> PatchAsync(string missionId, MissionAssignment missionAssignment);
    }
}
