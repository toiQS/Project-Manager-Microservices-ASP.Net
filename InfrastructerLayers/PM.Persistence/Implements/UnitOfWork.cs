using EasyNetQ;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;

namespace PM.Persistence.Implements
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IPubSub _eventBus;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<UnitOfWork> _logger;   

        public IRepository<ActivityLog, string> ActivityLogRepository { get; }
        public IRepository<Document, string> DocumentRepository { get; }
        public IRepository<Mission, string> MissionRepository { get; }
        public IRepository<MissionAssignment, string> MissionAssignmentRepository { get; }
        public IRepository<ProgressReport, string> ProgressReportRepository { get; }
        public IRepository<Project, string> ProjectRepository { get; }
        public IRepository<RoleInProject, string> RoleInProjectRepository { get; }
        public IRepository<ProjectMember, string> ProjectMemberRepository { get; }
        public IRepository<Status, int> StatusRepository { get; }
        public IRepository<User, string> UserRepository { get; }
        public IRepository<Plan, string> PlanRepository { get; }
        public IRepository<RefreshToken, string> RefreshTokenRepository { get; }

        public UnitOfWork(ApplicationDbContext context, IMemoryCache cache, IPubSub eventBus, ILoggerFactory loggerFactory, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _cache = cache;
            _eventBus = eventBus;
            _logger = logger;
            _loggerFactory = loggerFactory;
            ActivityLogRepository = new Repository<ActivityLog, string>(_context, _loggerFactory.CreateLogger<Repository<ActivityLog, string>>());
            DocumentRepository = new Repository<Document, string>(_context, _loggerFactory.CreateLogger<Repository<Document, string>>());
            MissionRepository = new Repository<Mission, string>(_context, _loggerFactory.CreateLogger<Repository<Mission, string>>());
            MissionAssignmentRepository = new Repository<MissionAssignment, string>(_context, _loggerFactory.CreateLogger<Repository<MissionAssignment, string>>());
            ProgressReportRepository = new Repository<ProgressReport, string>(_context, _loggerFactory.CreateLogger<Repository<ProgressReport, string>>());
            ProjectRepository = new Repository<Project, string>(_context, _loggerFactory.CreateLogger<Repository<Project, string>>());
            RoleInProjectRepository = new Repository<RoleInProject, string>(_context, _loggerFactory.CreateLogger<Repository<RoleInProject, string>>());
            ProjectMemberRepository = new Repository<ProjectMember, string>(_context, _loggerFactory.CreateLogger<Repository<ProjectMember, string>>());
            StatusRepository = new Repository<Status, int>(_context, _loggerFactory.CreateLogger<Repository<Status, int>>());
            UserRepository = new Repository<User, string>(_context, _loggerFactory.CreateLogger<Repository<User, string>>());
            PlanRepository = new Repository<Plan, string>(_context, _loggerFactory.CreateLogger<Repository<Plan, string>>());
            RefreshTokenRepository = new Repository<RefreshToken, string>(_context, _loggerFactory.CreateLogger<Repository<RefreshToken, string>>());
        }

        // Transaction & Caching
        #region Transaction Management - ExecuteTransactionAsync
        /// <summary>
        /// Executes database operations within a transaction scope.
        /// </summary>
        public async Task<ServicesResult<bool>> ExecuteTransactionAsync(Func<Task> transactionOperations, CancellationToken cancellationToken = default)
        {
            if (transactionOperations == null)
                return ServicesResult<bool>.Failure("Transaction operations cannot be null.");

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await transactionOperations();
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

        // Caching
        #region Caching - GetCachedAsync
        /// <summary>
        /// Retrieves an entity from cache or loads it from the database if not found.
        /// </summary>
        public async Task<ServicesResult<T>> GetCachedAsync<T, TKey>(TKey id, CancellationToken cancellationToken = default) where T : class
        {
            if (id == null)
                return ServicesResult<T>.Failure("Primary key is null.");

            try
            {
                var cacheKey = $"Entity:{typeof(T).Name}:{id}";
                if (_cache.TryGetValue(cacheKey, out T cachedEntity))
                    return ServicesResult<T>.Success(cachedEntity);

                var entity = await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
                if (entity == null)
                    return ServicesResult<T>.Failure("Entity not found.");

                _cache.Set(cacheKey, entity, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
                return ServicesResult<T>.Success(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving cached data.");
                return ServicesResult<T>.Failure("Cache retrieval error.");
            }
        }
        #endregion

        // Event-Driven
        #region Event Management - PublishEventAsync
        /// <summary>
        /// Publishes an event to the event bus.
        /// </summary>
        public async Task<ServicesResult<bool>> PublishEventAsync<T>(string eventName, T eventData, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(eventName))
                return ServicesResult<bool>.Failure("Event name cannot be empty.");

            if (eventData == null)
                return ServicesResult<bool>.Failure("Event data cannot be null.");

            try
            {
                await _eventBus.PublishAsync(eventData, cancellationToken);
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing event: {EventName}", eventName);
                return ServicesResult<bool>.Failure("Event publishing failed.");
            }
        }
        #endregion

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
    }
}
