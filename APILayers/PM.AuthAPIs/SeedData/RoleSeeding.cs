using Microsoft.AspNetCore.Identity;

namespace PM.AuthAPIs.SeedData
{
    public static class RoleSeeding
    {
        public static async Task Initialize(this IServiceProvider serviceProvider)
        {
            try
            {
                var context = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (context.Roles.Any()) return;
                var roles = new[] { "Admin", "Customer" };
                foreach (var role in roles)
                {
                    if (await context.RoleExistsAsync(role)) continue;
                    await context.CreateAsync(new IdentityRole(role));
                }

            }
            catch
            {
                throw;
            }
        }
    }
}
