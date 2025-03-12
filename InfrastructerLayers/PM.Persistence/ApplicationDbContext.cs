using Microsoft.EntityFrameworkCore;
using PM.Domain.Entities;

namespace PM.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Document> Document { get; set; }
        public DbSet<Mission> Mission { get; set; }
        public DbSet<MissionAssignment> MissionAssignment { get; set; }
        public DbSet<Plan> Plan { get; set; }
        public DbSet<ProgressReport> ProgressReport { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectMember> ProjectMember { get; set; }
        public DbSet<RoleInProject> RoleInProject { get; set; }
        public DbSet<Status> Status { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Plan>()
                .HasOne(plan => plan.Status)
                .WithMany()
                .HasForeignKey(plan => plan.StatusId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Mission>()
                .HasOne(mis => mis.Status)
                .WithMany()
                .HasForeignKey(mis => mis.StatusId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Project>()
                .HasOne(pro => pro.Status)
                .WithMany()
                .HasForeignKey(pro => pro.StatusId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Document>()
                .HasOne(doc => doc.Project)
                .WithMany()
                .HasForeignKey(doc => doc.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Document>()
               .HasOne(doc => doc.Mission)
               .WithMany()
               .HasForeignKey(doc => doc.MissionId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MissionAssignment>()
                .HasOne(mis => mis.ProjectMember)
                .WithMany()
                .HasForeignKey(mis => mis.ProjectMemberId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
