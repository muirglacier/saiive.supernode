using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("v1/api/")]
    public class BitcoreController : ControllerBase
    {
        private readonly ILogger<BitcoreController> _logger;
        private readonly string _apiUrl;
        private readonly string _network;

        private readonly HttpClient _client = new HttpClient();

        public BitcoreController(ILogger<BitcoreController> logger, IConfiguration config)
        {
            _logger = logger;
            _apiUrl = config["BITCORE_URL"];
            _network = config["NETWORK"];
            
            _logger.LogDebug($"Using bitcore {_apiUrl} on network {_network}");
        }

        [HttpGet("{coin}/balance/{address}")]
        public async Task<BalanceModel> GetBalance(string coin, string address)
        {
            var response = await _client.GetAsync($"{_apiUrl}/api/{coin}/{_network}/address/{address}/balance");

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<BalanceModel>(data);
            obj.Address = address;

            return obj;
        }

        [HttpPost("{coin}/balances")]
        public async Task<List<BalanceModel>> GetBalances(string coin, List<string> addresses)
        {
            var ret = new List<BalanceModel>();

            foreach (var address in addresses)
            {
                ret.Add(await GetBalance(coin, address));
            }

            return ret;
        }


        [HttpGet("{coin}/txs/{address}")]
        public async Task<List<TransactionModel>> GetTransactions(string coin, string address)
        {
            var response = await _client.GetAsync($"{_apiUrl}/api/{coin}/{_network}/address/{address}/txs");

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<List<TransactionModel>>(data);

            return obj;
        }

        [HttpPost("{coin}/txs")]
        public async Task<List<TransactionModel>> GetMultiTransactions(string coin, List<string> addresses)
        {
            var ret = new List<TransactionModel>();

            foreach (var address in addresses)
            {
                ret.AddRange(await GetTransactions(coin, address));
            }

            return ret;
        }

        [HttpGet("{coin}/fee")]
        public async Task<FeeEstimateModel> GetEstimateFee(string coin)
        {
            var response = await _client.GetAsync($"{_apiUrl}/api/{coin}/{_network}/fee/30");

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<FeeEstimateModel>(data);

            return obj;
        }
    }
}
