using Microsoft.EntityFrameworkCore;
using PM.Tracking.Domain;

namespace PM.Tracking.Infrastructure
{
    public class TrackingDbContext : DbContext
    {
        public TrackingDbContext(DbContextOptions<TrackingDbContext> options) : base(options) { }
        public DbSet<ActivityLog> ActivityLog { get; set; }
    }
}
