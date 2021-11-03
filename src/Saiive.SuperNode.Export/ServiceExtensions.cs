using Microsoft.Extensions.DependencyInjection;
using Saiive.SuperNode.Abstaction;

namespace Saiive.SuperNode.Export
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddExportHandler(this IServiceCollection services)
        {

            services.AddSingleton<IExportHandler, ExportHandler>();

          
            return services;
        }
    }
}
