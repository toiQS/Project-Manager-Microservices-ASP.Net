using PM.Domain.Entities;
using PM.Domain.Interfaces;

namespace PM.Persistence.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<ActivityLog, string> ActivityLogRepository => new Repository<ActivityLog, string>(_context);
        public IRepository<Document, string> DocumentRepository => new Repository<Document, string>(_context);
        public IRepository<Mission, string> MissionRepository => new Repository<Mission, string>(_context);
        public IRepository<MissionAssignment, string> MissionAssignmentRepository => new Repository<MissionAssignment, string>(_context);
        public IRepository<ProgressReport, string> ProgressReportRepository => new Repository<ProgressReport, string>(_context);
        public IRepository<Project, string> ProjectRepository => new Repository<Project, string>(_context);
        public IRepository<RoleInProject, string> RoleInProjectRepository => new Repository<RoleInProject, string>(_context);
        public IRepository<ProjectMember, string> ProjectMemberRepository => new Repository<ProjectMember, string>(_context);
        public IRepository<Status, int> StatusRepository => new Repository<Status, int>(_context);
        public IRepository<User, string> UserRepository => new Repository<User, string>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
