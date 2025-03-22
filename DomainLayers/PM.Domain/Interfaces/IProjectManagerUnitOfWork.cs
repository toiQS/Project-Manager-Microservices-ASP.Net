using PM.Domain.Entities;

namespace PM.Domain.Interfaces
{
    /// <summary>
    /// Interface for Unit of Work pattern to manage repositories and handle transactions.
    /// </summary>
    public interface IProjectManagerUnitOfWork : IDisposable
    {
        // Query Repositories
        
        IQueryRepository<Document, string> DocumentQueryRepository { get; }
        IQueryRepository<Mission, string> MissionQueryRepository { get; }
        IQueryRepository<MissionAssignment, string> MissionAssignmentQueryRepository { get; }
        IQueryRepository<ProgressReport, string> ProgressReportQueryRepository { get; }
        IQueryRepository<Project, string> ProjectQueryRepository { get; }
        IQueryRepository<RoleInProject, string> RoleInProjectQueryRepository { get; }
        IQueryRepository<ProjectMember, string> ProjectMemberQueryRepository { get; }
        IQueryRepository<Status, int> StatusQueryRepository { get; }
        IQueryRepository<User, string> UserQueryRepository { get; }
        IQueryRepository<Plan, string> PlanQueryRepository { get; }
        IQueryRepository<RefreshToken, string> RefreshTokenQueryRepository { get; }

        // Command Repositories
       
        ICommandRepository<Document, string> DocumentCommandRepository { get; }
        ICommandRepository<Mission, string> MissionCommandRepository { get; }
        ICommandRepository<MissionAssignment, string> MissionAssignmentCommandRepository { get; }
        ICommandRepository<ProgressReport, string> ProgressReportCommandRepository { get; }
        ICommandRepository<Project, string> ProjectCommandRepository { get; }
        ICommandRepository<RoleInProject, string> RoleInProjectCommandRepository { get; }
        ICommandRepository<ProjectMember, string> ProjectMemberCommandRepository { get; }
        ICommandRepository<Status, int> StatusCommandRepository { get; }
        ICommandRepository<User, string> UserCommandRepository { get; }
        ICommandRepository<Plan, string> PlanCommandRepository { get; }
        ICommandRepository<RefreshToken, string> RefreshTokenCommandRepository { get; }

        // Transaction Management
        /// <summary>
        /// Executes a transaction with the provided operations.
        /// </summary>
        /// <param name="transactionOperations">Operations to be executed within the transaction.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Result of the transaction.</returns>
        Task<ServicesResult<bool>> ExecuteTransactionAsync(Func<Task<ServicesResult<bool>>> transactionOperations, CancellationToken cancellationToken = default);

        // Caching
        /// <summary>
        /// Retrieves a cached item by its ID.
        /// </summary>
        /// <typeparam name="T">Type of the cached item.</typeparam>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="id">Identifier of the cached item.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Result of the cached item retrieval.</returns>
        //Task<ServicesResult<T>> GetCachedAsync<T, TKey>(TKey id, CancellationToken cancellationToken = default) where T : class;

        // Persistence
        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Result indicating success or failure.</returns>
        Task<ServicesResult<bool>> SaveChangesAsync(CancellationToken cancellationToken = default);

        // Event-driven Architecture
        /// <summary>
        /// Publishes an event with the provided data.
        /// </summary>
        /// <typeparam name="T">Type of the event data.</typeparam>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="eventData">Event data to be published.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Result indicating success or failure.</returns>
        //Task<ServicesResult<bool>> PublishEventAsync<T>(string eventName, T eventData, CancellationToken cancellationToken = default);
    }
}
