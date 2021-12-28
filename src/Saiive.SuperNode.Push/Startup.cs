using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saiive.Dobby.Api;
using Saiive.SuperNode.Bitcoin;
using Saiive.SuperNode.DeFiChain;
using Saiive.SuperNode.Export;
using Saiive.SuperNode.Push;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Saiive.SuperNode.Push
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables().Build();

            builder.Services.AddLogging();
            builder.Services.AddDeFiChain();
            builder.Services.AddBitcoin();

            builder.Services.AddDobbyApi();

            builder.Services.AddChainProviderService();

        }
    }

}
