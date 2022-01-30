using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.DeFiChain.Ocean;
using Saiive.SuperNode.DeFiChain.Providers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain
{
    internal class BaseDeFiChainProvider : BaseProvider
    {
        protected readonly string OceanUrl;
        protected readonly string DefiChainApiUrl;
        protected readonly string CoingeckoApiUrl;
        protected readonly string ApiUrl;
        protected readonly string LegacyBitcoreUrl;
        internal const string ApiVersion = "v0";

        protected readonly HttpClient _client;


        private Dictionary<string, bool> _oceanSyncState = new Dictionary<string, bool>();

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

            _oceanSyncState.Add("mainnet", false);
            _oceanSyncState.Add("testnet", false);

            var timer = new Timer(DoCheckOceanSyncState, null, 1, Convert.ToInt32(TimeSpan.FromMinutes(5).TotalMilliseconds));
        }

        private async void DoCheckOceanSyncState(object state)
        {
            await CheckOceanSyncState("mainnet");
            await CheckOceanSyncState("testnet");
        }

        private async Task<bool> CheckOceanSyncState(string network)
        {
            var stats = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/stats");
            var statsData = await stats.Content.ReadAsStringAsync();

            var statsObj = JsonConvert.DeserializeObject<OceanStats>(statsData);

            var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/blocks/{statsObj.Data.Count.Blocks}");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<OceanBlock>(data);

            var d = BlockProvider.ConvertOceanModel(obj.Data);
            var time = Convert.ToDateTime(d.Time);

            var timeStartCheck = DateTime.Now.AddMinutes(20 * -1);
            var timeEndCheck = DateTime.Now.AddMinutes(20);

            var result = false;
            if (time >= timeStartCheck && time <= timeEndCheck)
            {
                result = true;
            }
            _oceanSyncState[network] = result;
            return result;

        }

        public async Task<T> RunWithFallbackProvider<T>(string fallbackUrl, string network, Func<Task<T>> func, Func<string, T> fallbackFunc=null)
        {
            try
            {
                var isOceanOk = await CheckOceanSyncState(network);

                if (isOceanOk)
                {
                    var t = await func();
                    return t;
                }
                else
                {
                    return await DoFallbackRequest(fallbackUrl, network, fallbackFunc);
                }

            }
            catch
            {
                return await DoFallbackRequest(fallbackUrl, network, fallbackFunc);
            }
        }

        private async Task<T> DoFallbackRequest<T>(string fallbackUrl, string network, Func<string, T> fallbackFunc = null)
        {
            var responseLegacy = await _client.GetAsync($"{ApiUrl}/{fallbackUrl}");

            responseLegacy.EnsureSuccessStatusCode();

            var data = await responseLegacy.Content.ReadAsStringAsync();

            if (fallbackFunc != null)
            {
                return fallbackFunc(data);
            }
            return JsonConvert.DeserializeObject<T>(data);
        }

        internal static DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            return dtDateTime;
        }
    }
}
