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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Plan>()
                .HasOne(p => p.ProjectMember)
                .WithMany()
                .HasForeignKey(p => p.ProjectMemberId)
                .OnDelete(DeleteBehavior.Restrict); // hoặc .NoAction()

            modelBuilder.Entity<MissionMember>()
                .HasOne(mm => mm.ProjectMember)
                .WithMany()
                .HasForeignKey(mm => mm.MemberId)
                .OnDelete(DeleteBehavior.Restrict); // hoặc .NoAction()

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany()
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Position)
                .WithMany()
                .HasForeignKey(pm => pm.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
