using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class CoingecokController : BaseController
    {
        public CoingecokController(ILogger<DefiChainController> logger, IConfiguration config) : base(logger, config)
        {
          
        }


        [HttpGet("{network}/{coin}/coin-price/{currency}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IDictionary<int, CoinPrice>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> CoinPrice(string coin, string network, string currency)
        {

            //We control the coins server-side, so we can update faster if new pairs come along
            var response = await _client.GetAsync($"{CoingeckoApiUrl}/simple/price?ids=defichain,bitcoin,ethereum,tether,dogecoin,litecoin&vs_currencies={currency}");

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
                    coinPrice.fiat = item.Value[currency.ToLower()];

                    ret.Add(item.Key, coinPrice);  
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
