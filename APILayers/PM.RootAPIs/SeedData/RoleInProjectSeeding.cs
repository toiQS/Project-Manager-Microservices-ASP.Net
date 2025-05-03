using PM.Domain.Entities;
using PM.Persistence;

namespace PM.RootAPIs.SeedData
{
    public static class RoleInProjectSeeding
    {
        public static async Task Initialize(this IServiceProvider serviceProvider)
        {

            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if (context.RoleInProject.Any()) return;
            var arr = new[] { "Owner", "Leader", "Manager", "Member" };
            foreach (var item in arr)
            {
                context.AddRange(new RoleInProject()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = item,
                    Description = item,
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
