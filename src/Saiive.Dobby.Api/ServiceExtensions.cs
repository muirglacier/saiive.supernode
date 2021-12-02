using Microsoft.Extensions.DependencyInjection;

namespace Saiive.Dobby.Api
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDobbyApi(this IServiceCollection services)
        {
            services.AddSingleton<IDobbyService, DobbyService>();

            return services;
        }
    }
}
