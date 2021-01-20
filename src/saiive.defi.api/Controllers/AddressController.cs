using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("v1/api/")]
    public class AddressController : BaseController
    {
        

        public AddressController(ILogger<AddressController> logger, IConfiguration config) : base(logger, config)
        {
        }

        [HttpGet("{coin}/balance/{address}")]
        public async Task<BalanceModel> GetBalance(string coin, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/address/{address}/balance");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<BalanceModel>(data);
                obj.Address = address;

                return obj;
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                throw;
            }
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
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/address/{address}/txs");
            try
            {
                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<List<TransactionModel>>(data);

                return obj;
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                throw;
            }
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
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/fee/30");
            try
            {
                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<FeeEstimateModel>(data);

                return obj;
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                throw;
            }
        }
    }
}
