using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Saiive.SuperNode.Abstaction;

namespace Saiive.SuperNode
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddChainProviderService(this IServiceCollection services)
        {
            services.AddSingleton<ChainProviderCollection>();
            return services;
        }


        public static IApplicationBuilder UseChainProivderService(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices.GetServices<IChainProvider>();

            var chainProviderCollection = app.ApplicationServices.GetRequiredService<ChainProviderCollection>();

            foreach(var service in services)
            {
                chainProviderCollection.Add(service.CoinType, service);
            }

            return app;
        }
    }
}
