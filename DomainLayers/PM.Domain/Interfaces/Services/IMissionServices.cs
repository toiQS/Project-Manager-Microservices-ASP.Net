using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing missions.
    /// </summary>
    public interface IMissionServices
    {
        /// <summary>
        /// Retrieves all missions.
        /// </summary>
        /// <returns>A collection of missions.</returns>
        public Task<ServicesResult<IEnumerable<Mission>>> GetMissions();

        /// <summary>
        /// Retrieves missions associated with a specific plan.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan.</param>
        /// <returns>A collection of missions for the specified plan.</returns>
        public Task<ServicesResult<IEnumerable<Mission>>> GetMissionsInPlan(string planId);

        /// <summary>
        /// Retrieves the details of a specific mission.
        /// </summary>
        /// <param name="missionId">The unique identifier of the mission.</param>
        /// <returns>The mission associated with the provided ID.</returns>
        public Task<ServicesResult<Mission>> GetDetailMission(string missionId);

        /// <summary>
        /// Creates a new mission.
        /// </summary>
        /// <param name="mission">The mission to create.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> CreateMission(Mission mission);

        /// <summary>
        /// Updates an existing mission.
        /// </summary>
        /// <param name="mission">The updated mission data.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> UpdateMission(Mission mission);

        /// <summary>
        /// Applies partial updates to a mission.
        /// </summary>
        /// <param name="missionId">The unique identifier of the mission.</param>
        /// <param name="mission">The mission data to patch.</param>
        /// <returns>True if the patch was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> PatchMission(string missionId, Mission mission);

        /// <summary>
        /// Deletes a mission.
        /// </summary>
        /// <param name="missionId">The unique identifier of the mission to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> DeleteMission(string missionId);

        /// <summary>
        /// Deletes all mission assignments within a mission.
        /// </summary>
        /// <param name="missionId">The unique identifier of the mission.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> DeleteMissionAssignmentInMission(string missionId);
    }
}
