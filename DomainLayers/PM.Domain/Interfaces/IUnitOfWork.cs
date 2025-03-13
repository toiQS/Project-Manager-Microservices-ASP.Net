using PM.Domain.Entities;

namespace PM.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
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

        // Transaction & Caching
        public Task<ServicesResult<bool>> ExecuteTransactionAsync(Func<Task<ServicesResult<bool>>> transactionOperations, CancellationToken cancellationToken = default);
        public Task<ServicesResult<T>> GetCachedAsync<T, TKey>(TKey id, CancellationToken cancellationToken = default) where T : class;

        // SaveChanges & Event-driven
        public Task<ServicesResult<bool>> SaveChangesAsync(CancellationToken cancellationToken = default);
        public Task<ServicesResult<bool>> PublishEventAsync<T>(string eventName, T eventData, CancellationToken cancellationToken = default);
    }
}
