using PM.Domain.Entities;
using PM.Persistence;

namespace RootAPIs.SeedData
{
    public static class StatusSeeding
    {
        public static async Task Initialize(this IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if (context.Status.Any()) return;
            var arr = new[] { "not selected", "waiting", "in progress", "completed early", "finished on time", "behind schedule", "finished late" };
            foreach (var item in arr)
            {
                context.AddRange(new Status
                {
                    Name = item,
                });
            }
            await context.SaveChangesAsync();
        }
    }
}
