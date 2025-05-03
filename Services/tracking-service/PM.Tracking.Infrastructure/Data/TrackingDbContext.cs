using Microsoft.EntityFrameworkCore;
using PM.Tracking.Entities;

namespace PM.Tracking.Infrastructure.Data
{
    public class TrackingDbContext :DbContext
    {
        public TrackingDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ActivityLog> ActivityLogs { get; set; } = null!;
    }
}
