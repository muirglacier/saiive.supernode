using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Saiive.SuperNode.Bitcoin;
using Saiive.SuperNode.DeFiChain;
using Saiive.SuperNode.Function;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Saiive.SuperNode.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDeFiChain();
            builder.Services.AddBitcoin();

            builder.Services.AddChainProviderService();
        }


        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);
        }
    }

}
