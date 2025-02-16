using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;

namespace PM.Infrastructers
{
    public static class Configurations
    {
        public static void InitializePersistence(this IServiceCollection services, IConfiguration configuration)
        {

        }
        private static void RegisterJwt(this IServiceCollection services, IConfiguration configuration)
        {

        }
    }
}
