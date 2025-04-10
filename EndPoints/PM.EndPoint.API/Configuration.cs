using PM.EndPoint.API.Flows.Implements;
using PM.EndPoint.API.Flows.Interfaces;

namespace PM.EndPoint.API
{
    public static class Configuration
    {
        public static void InitializeFlow(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors.AddHttpClient();
            serviceDescriptors.AddHttpContextAccessor();
            serviceDescriptors.AddScoped<IAuthFlow, AuthFlow>();

        }
    }
}
