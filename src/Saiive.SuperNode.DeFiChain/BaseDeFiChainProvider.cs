using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction;
using System;
using System.Net.Http;

namespace Saiive.SuperNode.DeFiChain
{
    internal class BaseDeFiChainProvider : BaseProvider
    {
        protected readonly string OceanUrl;
        protected readonly string DefiChainApiUrl;
        protected readonly string CoingeckoApiUrl;
        protected readonly string ApiUrl;
        protected readonly string LegacyBitcoreUrl;

        protected readonly HttpClient _client;

        public BaseDeFiChainProvider(ILogger logger, IConfiguration config) : base(logger, config)
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromMinutes(5);
                
            OceanUrl = config["OCEAN_URL"];
            DefiChainApiUrl = config["DEFI_CHAIN_API_URL"];
            CoingeckoApiUrl = config["COINGECKO_API_URL"];
            ApiUrl = config["LEGACY_API_URL"];

            LegacyBitcoreUrl = config["LEGACY_BITCORE_URL"];

            Logger?.LogTrace($"Using ocean {OceanUrl}");
            Logger?.LogTrace($"Using DefiChainApi {DefiChainApiUrl}");
            Logger?.LogTrace($"Using CoingeckoApi {CoingeckoApiUrl}");
            Logger?.LogTrace($"Using LEGACY_API_URL {ApiUrl}");
            Logger?.LogTrace($"Using LEGACY_BITCORE_URL {LegacyBitcoreUrl}");
        }

        public DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            return dtDateTime;
        }
    }
}
