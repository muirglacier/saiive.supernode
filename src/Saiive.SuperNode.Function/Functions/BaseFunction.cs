using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction;
using System;
using System.Net.Http;

namespace Saiive.SuperNode.Function.Functions
{
    public abstract class BaseFunction
    {
        protected readonly ILogger Logger;
        private readonly IServiceProvider serviceProvider;


        protected readonly string DefiChainApiUrl;
        protected readonly string CoingeckoApiUrl;
        protected readonly string ApiUrl;


        protected readonly HttpClient _client;

        protected BaseFunction(ILogger logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider)
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromMinutes(5);

            Logger = logger;
            ChainProviderCollection = chainProviderCollection;
            this.serviceProvider = serviceProvider;

            var services = serviceProvider.GetServices<IChainProvider>();

            foreach (var service in services)
            {
                if(!chainProviderCollection.ContainsKey(service.CoinType))
                    chainProviderCollection.Add(service.CoinType, service);
            }

            var config = serviceProvider.GetService<IConfiguration>();

            DefiChainApiUrl = config["DEFI_CHAIN_API_URL"];
            CoingeckoApiUrl = config["COINGECKO_API_URL"];
            ApiUrl = config["LEGACY_API_URL"];

        }

        public ChainProviderCollection ChainProviderCollection { get; }
    }
}
