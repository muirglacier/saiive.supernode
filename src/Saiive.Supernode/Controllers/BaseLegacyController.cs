using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Saiive.SuperNode.Controllers
{
    public abstract class BaseLegacyController : ControllerBase
    {
        public IConfiguration Config { get; }
        protected readonly ILogger Logger;
        protected readonly string ApiUrl;
        protected readonly string DefiChainApiUrl;
        protected readonly string CoingeckoApiUrl;

        protected readonly HttpClient _client;

        protected BaseLegacyController(ILogger logger, IConfiguration config)
        {
            Config = config;
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromMinutes(5);

            Logger = logger;
            ApiUrl = config["BITCORE_URL"];
            DefiChainApiUrl = config["DEFI_CHAIN_API_URL"];
            CoingeckoApiUrl = config["COINGECKO_API_URL"];

            Logger.LogTrace($"Using bitcore {ApiUrl}");
            Logger.LogTrace($"Using DefiChainApi {DefiChainApiUrl}");
            Logger.LogTrace($"Using CoingeckoApi {CoingeckoApiUrl}");
        }
    }
}
