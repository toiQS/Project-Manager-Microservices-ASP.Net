using Microsoft.EntityFrameworkCore;

namespace PM.Project.Infrastructure
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
        {
        }
    }
}
