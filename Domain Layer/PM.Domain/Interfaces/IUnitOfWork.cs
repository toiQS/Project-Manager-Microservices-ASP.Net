using PM.Domain.Entities;

namespace PM.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<ActivityLog,string> ActivityLogRepository {  get; }
        IRepository<Document,string> DocumentRepository { get; }
        IRepository<Mission,string> MissionRepository { get; }
        IRepository<MissionAssignment, string> MissionAssignmentRepository { get; }
        IRepository<ProgressReport,string> ProgressReportRepository { get; }
        IRepository<Project,string> ProjectRepository { get; }
        IRepository<RoleInProject, string> RoleInProjectRepository { get; }
        IRepository<ProjectMember, string> ProjectMemberRepository { get; }
        IRepository<Status,int> StatusRepository { get; }
        IRepository<User,string> UserRepository { get; }
        IRepository<Plan,string> PlanRepository { get; }
        IRepository<RefreshToken,string> RefreshTokenRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
