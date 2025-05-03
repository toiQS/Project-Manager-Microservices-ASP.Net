using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;

namespace PM.Infrastructer.Implements
{
    public class AuthUnitOfWork : IAuthUnitOfWork
    {
        private readonly AuthDbContext _context;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<AuthUnitOfWork> _logger;

        public IQueryRepository<ActivityLog, string> ActivityLogQueryRepository { get; }
        public IQueryRepository<RefreshToken, string> RefreshTokenQueryRepoitory { get; }
        public IQueryRepository<User, string> UserQueryRepository { get; }

        public ICommandRepository<ActivityLog, string> ActivityLogCommandRepository { get; }
        public ICommandRepository<RefreshToken, string> RefreshTokenCommandRepoitory { get; }
        public ICommandRepository<User, string> UserCommandRepository { get; }

        public AuthUnitOfWork(AuthDbContext authDbContext, ILoggerFactory loggerFactory, ILogger<AuthUnitOfWork> logger)
        {
            _context = authDbContext;
            _loggerFactory = loggerFactory;
            _logger = logger;

            ActivityLogQueryRepository = CreateQueryRepository<ActivityLog , string>();
            RefreshTokenQueryRepoitory = CreateQueryRepository<RefreshToken , string>();
            UserQueryRepository = CreateQueryRepository<User , string>();

            ActivityLogCommandRepository = CreateCommandRepository<ActivityLog , string>();
            RefreshTokenCommandRepoitory = CreateCommandRepository<RefreshToken , string>();
            UserCommandRepository = CreateCommandRepository<User , string>();
        }


        private IQueryRepository<TEntity, TKey> CreateQueryRepository<TEntity, TKey>() where TEntity : class
        {
            return new QueryRepository<TEntity, TKey>(_context, _loggerFactory.CreateLogger<QueryRepository<TEntity, TKey>>());
        }

        private ICommandRepository<TEntity, TKey> CreateCommandRepository<TEntity, TKey>() where TEntity : class
        {
            return new CommandRepository<TEntity, TKey>(_context, _loggerFactory.CreateLogger<CommandRepository<TEntity, TKey>>());
        }
        // Transaction & Caching
        #region Transaction Management - ExecuteTransactionAsync
        /// <summary>
        /// Executes database operations within a transaction scope.
        /// </summary>
        public async Task<ServicesResult<bool>> ExecuteTransactionAsync(Func<Task<ServicesResult<bool>>> transactionOperations, CancellationToken cancellationToken = default)
        {
            if (transactionOperations == null)
                return ServicesResult<bool>.Failure("Transaction operations cannot be null.");

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await transactionOperations();
                if (!result.Status)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return result;
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Transaction failed.");
                return ServicesResult<bool>.Failure("Transaction failed.");
            }
        }
        #endregion
        #region SaveChanges - SaveChangesAsync
        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        public async Task<ServicesResult<bool>> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving changes to the database.");
                return ServicesResult<bool>.Failure("Database save error.");
            }
        }
        #endregion
        public void Dispose() => _context.Dispose();
    }
}
