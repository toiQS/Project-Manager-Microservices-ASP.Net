using PM.Domain.Entities;

namespace PM.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
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

        // Transaction & Caching
        public Task<ServicesResult<bool>> ExecuteTransactionAsync(Func<Task<ServicesResult<bool>>> transactionOperations, CancellationToken cancellationToken = default);
        public Task<ServicesResult<T>> GetCachedAsync<T, TKey>(TKey id, CancellationToken cancellationToken = default) where T : class;

        // SaveChanges & Event-driven
        public Task<ServicesResult<bool>> SaveChangesAsync(CancellationToken cancellationToken = default);
        public Task<ServicesResult<bool>> PublishEventAsync<T>(string eventName, T eventData, CancellationToken cancellationToken = default);
    }
}
