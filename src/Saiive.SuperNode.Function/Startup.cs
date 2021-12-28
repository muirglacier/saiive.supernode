﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Saiive.SuperNode.Bitcoin;
using Saiive.SuperNode.DeFiChain;
using Saiive.SuperNode.Export;
using Saiive.SuperNode.Function;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Saiive.SuperNode.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddDeFiChain();
            builder.Services.AddBitcoin();

            builder.Services.AddChainProviderService();
            builder.Services.AddExportHandler();

        }
    }

}
