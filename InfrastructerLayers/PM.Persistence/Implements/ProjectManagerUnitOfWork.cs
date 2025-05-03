using EasyNetQ;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;

namespace PM.Persistence.Implements
{
    public class ProjectManagerUnitOfWork : IProjectManagerUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ProjectManagerUnitOfWork> _logger;
        // Query Repositories
        public IQueryRepository<ActivityLog, string> ActivityLogQueryRepository { get; }
        public IQueryRepository<Document, string> DocumentQueryRepository { get; }
        public IQueryRepository<Mission, string> MissionQueryRepository { get; }
        public IQueryRepository<MissionAssignment, string> MissionAssignmentQueryRepository { get; }
        public IQueryRepository<ProgressReport, string> ProgressReportQueryRepository { get; }
        public IQueryRepository<Project, string> ProjectQueryRepository { get; }
        public IQueryRepository<RoleInProject, string> RoleInProjectQueryRepository { get; }
        public IQueryRepository<ProjectMember, string> ProjectMemberQueryRepository { get; }
        public IQueryRepository<Status, int> StatusQueryRepository { get; }
        public IQueryRepository<User, string> UserQueryRepository { get; }
        public IQueryRepository<Plan, string> PlanQueryRepository { get; }
        public IQueryRepository<RefreshToken, string> RefreshTokenQueryRepository { get; }

        // Command Repositories
        public ICommandRepository<ActivityLog, string> ActivityLogCommandRepository { get; }
        public ICommandRepository<Document, string> DocumentCommandRepository { get; }
        public ICommandRepository<Mission, string> MissionCommandRepository { get; }
        public ICommandRepository<MissionAssignment, string> MissionAssignmentCommandRepository { get; }
        public ICommandRepository<ProgressReport, string> ProgressReportCommandRepository { get; }
        public ICommandRepository<Project, string> ProjectCommandRepository { get; }
        public ICommandRepository<RoleInProject, string> RoleInProjectCommandRepository { get; }
        public ICommandRepository<ProjectMember, string> ProjectMemberCommandRepository { get; }
        public ICommandRepository<Status, int> StatusCommandRepository { get; }
        public ICommandRepository<User, string> UserCommandRepository { get; }
        public ICommandRepository<Plan, string> PlanCommandRepository { get; }
        public ICommandRepository<RefreshToken, string> RefreshTokenCommandRepository { get; }

        public ProjectManagerUnitOfWork(ApplicationDbContext context,
                                        ILoggerFactory loggerFactory,
                                        ILogger<ProjectManagerUnitOfWork> logger)
        {
            _context = context;
           
            _logger = logger;
            _loggerFactory = loggerFactory;

            // Khởi tạo QueryRepository
            ActivityLogQueryRepository = CreateQueryRepository<ActivityLog, string>();
            DocumentQueryRepository = CreateQueryRepository<Document, string>();
            MissionQueryRepository = CreateQueryRepository<Mission, string>();
            MissionAssignmentQueryRepository = CreateQueryRepository<MissionAssignment, string>();
            ProgressReportQueryRepository = CreateQueryRepository<ProgressReport, string>();
            ProjectQueryRepository = CreateQueryRepository<Project, string>();
            RoleInProjectQueryRepository = CreateQueryRepository<RoleInProject, string>();
            ProjectMemberQueryRepository = CreateQueryRepository<ProjectMember, string>();
            StatusQueryRepository = CreateQueryRepository<Status, int>();
            UserQueryRepository = CreateQueryRepository<User, string>();
            PlanQueryRepository = CreateQueryRepository<Plan, string>();
            RefreshTokenQueryRepository = CreateQueryRepository<RefreshToken, string>();

            // Khởi tạo CommandRepository
            ActivityLogCommandRepository = CreateCommandRepository<ActivityLog, string>();
            DocumentCommandRepository = CreateCommandRepository<Document, string>();
            MissionCommandRepository = CreateCommandRepository<Mission, string>();
            MissionAssignmentCommandRepository = CreateCommandRepository<MissionAssignment, string>();
            ProgressReportCommandRepository = CreateCommandRepository<ProgressReport, string>();
            ProjectCommandRepository = CreateCommandRepository<Project, string>();
            RoleInProjectCommandRepository = CreateCommandRepository<RoleInProject, string>();
            ProjectMemberCommandRepository = CreateCommandRepository<ProjectMember, string>();
            StatusCommandRepository = CreateCommandRepository<Status, int>();
            UserCommandRepository = CreateCommandRepository<User, string>();
            PlanCommandRepository = CreateCommandRepository<Plan, string>();
            RefreshTokenCommandRepository = CreateCommandRepository<RefreshToken, string>();
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

        //// Caching
        //#region Caching - GetCachedAsync
        ///// <summary>
        ///// Retrieves an entity from cache or loads it from the database if not found.
        ///// </summary>
        //public async Task<ServicesResult<T>> GetCachedAsync<T, TKey>(TKey id, CancellationToken cancellationToken = default) where T : class
        //{
        //    if (id == null)
        //        return ServicesResult<T>.Failure("Primary key is null.");

        //    try
        //    {
        //        var cacheKey = $"Entity:{typeof(T).Name}:{id}";
        //        if (_cache.TryGetValue(cacheKey, out T cachedEntity))
        //            return ServicesResult<T>.Success(cachedEntity!);

        //        var entity = await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        //        if (entity == null)
        //            return ServicesResult<T>.Failure("Entity not found.");

        //        _cache.Set(cacheKey, entity, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
        //        return ServicesResult<T>.Success(entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while retrieving cached data.");
        //        return ServicesResult<T>.Failure("Cache retrieval error.");
        //    }
        //}
        //#endregion

        //// Event-Driven
        //#region Event Management - PublishEventAsync
        ///// <summary>
        ///// Publishes an event to the event bus.
        ///// </summary>
        //public async Task<ServicesResult<bool>> PublishEventAsync<T>(string eventName, T eventData, CancellationToken cancellationToken = default)
        //{
        //    if (string.IsNullOrWhiteSpace(eventName))
        //        return ServicesResult<bool>.Failure("Event name cannot be empty.");

        //    if (eventData == null)
        //        return ServicesResult<bool>.Failure("Event data cannot be null.");

        //    try
        //    {
        //        await _eventBus.PublishAsync(eventData, cancellationToken);
        //        return ServicesResult<bool>.Success(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error publishing event: {EventName}", eventName);
        //        return ServicesResult<bool>.Failure("Event publishing failed.");
        //    }
        //}
        //#endregion

        // Save Changes
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
