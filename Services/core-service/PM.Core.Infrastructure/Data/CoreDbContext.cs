using Microsoft.EntityFrameworkCore;
using PM.Core.Entities;

namespace PM.Core.Infrastructure.Data
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Mission> Missions { get; set; }
        public DbSet<MissionMember> MissionMembers { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> Members { get; set; }
    }
}
