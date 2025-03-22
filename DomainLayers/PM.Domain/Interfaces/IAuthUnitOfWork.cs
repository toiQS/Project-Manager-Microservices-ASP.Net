using PM.Domain.Entities;

namespace PM.Domain.Interfaces
{
    public interface IAuthUnitOfWork
    {

        IQueryRepository<ActivityLog, string> ActivityLogQueryRepository { get; }
        IQueryRepository<RefreshToken, string> RefreshTokenQueryRepoitory { get; } 
        IQueryRepository<User, string> UserQueryRepository { get; }
        ICommandRepository<ActivityLog, string> ActivityLogCommandRepository { get; }
        ICommandRepository<RefreshToken, string> RefreshTokenCommandRepoitory { get; }
        ICommandRepository<User, string> UserCommandRepository { get; }
        // Transaction Management
        /// <summary>
        /// Executes a transaction with the provided operations.
        /// </summary>
        /// <param name="transactionOperations">Operations to be executed within the transaction.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Result of the transaction.</returns>
        Task<ServicesResult<bool>> ExecuteTransactionAsync(Func<Task<ServicesResult<bool>>> transactionOperations, CancellationToken cancellationToken = default);
        // Persistence
        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Result indicating success or failure.</returns>
        Task<ServicesResult<bool>> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
