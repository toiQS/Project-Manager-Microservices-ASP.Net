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
            var arr = new[] { "Not Selected", "Waiting", "In Progress", "Completed Early", "Finished On Time", "Behind Schedule", "Finished Late" };
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
