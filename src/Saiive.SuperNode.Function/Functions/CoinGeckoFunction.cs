using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Function.Base;

namespace Saiive.SuperNode.Function.Functions
{
    public class CoinGeckoFunction : BaseFunction
    {
        private static Dictionary<string, Dictionary<string, CoinPrice>> _lastPrices = new Dictionary<string, Dictionary<string, CoinPrice>>();
        private static DateTime _lastRefresh;
        private static int _priceUpdateInMinutes = 5;

        public CoinGeckoFunction(ILogger<AddressFunctions> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }

        [FunctionName("CoinPrice")]
        [OpenApiOperation(operationId: "CoinPrice", tags: new[] { "Coingecko" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "currency", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Dictionary<string, CoinPrice>), Description = "The OK response")]
        public async Task<IActionResult> GetCurrentBlock(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/coin-price/{currency}")] HttpRequestMessage req,
            string network, string coin, string currency,
            ILogger log)
        {
            if( _lastPrices.ContainsKey(currency.ToLower()) && (DateTime.UtcNow - _lastRefresh).TotalMinutes <= _priceUpdateInMinutes)
            {
                return new OkObjectResult(_lastPrices[currency.ToLower()]);
            }

            //We control the coins server-side, so we can update faster if new pairs come along
            var response = await _client.GetAsync($"{CoingeckoApiUrl}/simple/price?ids=defichain,bitcoin,ethereum,tether,dogecoin,litecoin,bitcoin-cash,tether&vs_currencies={currency}");

            var map = new Dictionary<string, string>();

            if (network == "testnet")
            {
                map.Add("defichain", "0");
                map.Add("bitcoin", "1");
                map.Add("ethereum", "2");
                map.Add("tether", "5");
                map.Add("dogecoin", "7");
                map.Add("litecoin", "9");
            }
            else
            {
                map.Add("defichain", "0");
                map.Add("bitcoin", "2");
                map.Add("ethereum", "1");
                map.Add("tether", "3");
                map.Add("dogecoin", "7");
                map.Add("litecoin", "9");
                map.Add("bitcoin-cash", "12");
            }

            try
            {
                var data = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                var ret = new Dictionary<string, CoinPrice>();

                Dictionary<string, Dictionary<string, double>> obj = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>(data);

                foreach (var item in obj)
                {
                    var coinPrice = new CoinPrice();
                    coinPrice.Coin = item.Key;
                    coinPrice.Currency = currency;
                    coinPrice.Fiat = item.Value[currency.ToLower()];
                    coinPrice.IdToken = map.ContainsKey(item.Key) ? map[item.Key] : null;

                    ret.Add(item.Key.Replace("-", ""), coinPrice);
                }

                _lastRefresh = DateTime.UtcNow;

                ret.Add("lastUpdate", new CoinPrice()
                {
                    Coin = $"{_lastRefresh:o}",
                    Currency = $"{_lastRefresh:o}",
                    Fiat = 0,
                    IdToken = $"Prices are updated every {_priceUpdateInMinutes} minute!"
                });

                if(!_lastPrices.ContainsKey(currency.ToLower()))
                {
                    _lastPrices.Add(currency.ToLower(), ret);
                }

                _lastPrices[currency.ToLower()] = ret;

                return new OkObjectResult(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

       
    }
}

