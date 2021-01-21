using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
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

        private async Task<BalanceModel> GetBalanceInternal(string coin, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/address/{address}/balance");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<BalanceModel>(data);
            obj.Address = address;

            return obj;

        }

        private async Task<List<AccountModel>> GetAccountInternal(string coin, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/address/{address}/account");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var ret = new  List<AccountModel>();
            var obj = JsonConvert.DeserializeObject<List<string>>(data);

            
            foreach (var acc in obj)
            {
                var split = acc.Split("@");
                var account = new AccountModel
                {
                    Address = address,
                    Raw = acc,
                    Balance = Convert.ToDouble(split[0], CultureInfo.InvariantCulture),
                    Token = split[1]
                };

                ret.Add(account);
            }
            
            
            return ret;

        }

        [HttpGet("{coin}/balance/{address}")]
        public async Task<IActionResult> GetBalance(string coin, string address)
        {
            
            try
            {
                return Ok(await GetBalanceInternal(coin, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(e);
            }
        }
        [HttpPost("{coin}/balances")]
        public async Task<IActionResult> GetBalances(string coin, List<string> addresses)
        {
            try
            {
                var ret = new List<BalanceModel>();

                foreach (var address in addresses)
                {
                    ret.Add(await GetBalanceInternal(coin, address));
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(e);
            }
        }


        [HttpGet("{coin}/account/{address}")]
        public async Task<IActionResult> GetAccount(string coin, string address)
        {

            try
            {
                return Ok(await GetAccountInternal(coin, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(e);
            }
        }
        
        [HttpPost("{coin}/accounts")]
        public async Task<IActionResult> GetAccounts(string coin, List<string> addresses)
        {
            try
            {
                var ret = new Dictionary<string, List<AccountModel>>();

                foreach (var address in addresses)
                {
                    var accountModel = await GetAccountInternal(coin, address);
                    ret.Add(address, accountModel);
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(e);
            }
        }




        public async Task<List<TransactionModel>> GetTransactionsInternal(string coin, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/address/{address}/txs");

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<List<TransactionModel>>(data);

            return obj;

        }


        [HttpGet("{coin}/txs/{address}")]
        public async Task<IActionResult> GetTransactions(string coin, string address)
        {
            try
            {
                return Ok(await GetTransactionsInternal(coin, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(e);
            }
        }

        [HttpPost("{coin}/txs")]
        public async Task<IActionResult> GetMultiTransactions(string coin, List<string> addresses)
        {
            try
            {
                var ret = new List<TransactionModel>();

                foreach (var address in addresses)
                {
                    ret.AddRange(await GetTransactionsInternal(coin, address));
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(e);
            }
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
