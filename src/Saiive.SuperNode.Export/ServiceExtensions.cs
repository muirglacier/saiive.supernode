using Microsoft.Extensions.DependencyInjection;
using Saiive.SuperNode.Abstaction;
using SendGrid.Extensions.DependencyInjection;
using System;

namespace Saiive.SuperNode.Export
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddExportHandler(this IServiceCollection services)
        {

            services.AddSingleton<IExportHandler, ExportHandler>();
            services.AddSingleton<MailHandler>();
           
            services.AddSendGrid(options =>
            {
                options.ApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            });
            return services;
        }
    }
}
