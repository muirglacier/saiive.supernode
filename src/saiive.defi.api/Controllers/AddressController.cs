using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;
using saiive.defi.api.Requests;

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

            var nativeBalance = await GetBalanceInternal(coin, address);
            if (nativeBalance.Confirmed > 0)
            {
                ret.Add(new AccountModel
                {
                    Address = address,
                    Balance = nativeBalance.Confirmed,
                    Raw = $"{nativeBalance.Confirmed}@$DFI",
                    Token = "$DFI"
                });
            }

            return ret;

        }


        [HttpGet("{coin}/balance-all/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BalanceModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]

        public async Task<IActionResult> GetTotalBalance(string coin, string address)
        {

            try
            {
                var balanceNative = await GetBalanceInternal(coin, address);
                var balanceTokens = await GetAccountInternal(coin, address);
                
                balanceTokens.Add(new AccountModel
                {
                    Address = address,
                    Balance = balanceNative.Balance,
                    Token = "$DFI"
                });

                foreach (var account in balanceTokens)
                {
                    account.Address = null;
                    account.Raw = null;
                }
                return Ok(balanceTokens);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
        [HttpPost("{coin}/balance-all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dictionary<string, AccountModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTotalBalance(string coin, AddressesBodyRequest addresses)
        {
            try
            {
                var retAccountList = new Dictionary<string, AccountModel>();
                var accounts = new Dictionary<string, List<AccountModel>>();

                foreach (var address in addresses.Addresses)
                {
                    var accountModel = await GetAccountInternal(coin, address);
                    accounts.Add(address, accountModel);

                    foreach (var account in accountModel)
                    {
                        account.Address = null;
                        account.Raw = null;
                        if (!retAccountList.ContainsKey(account.Token))
                        {
                            retAccountList.Add(account.Token, account);
                        }
                        else
                        {
                            retAccountList[account.Token].Balance += account.Balance;
                        }
                    }
                    
                }
                
                
                foreach (var address in addresses.Addresses)
                {
                    var balanceNative = await GetBalanceInternal(coin, address);
                    var account = new AccountModel
                    {
                        Balance = balanceNative.Balance,
                        Token = "$DFI"
                    };
                    accounts[address].Add(account);


                    if (!retAccountList.ContainsKey(account.Token))
                    {
                        retAccountList.Add(account.Token, account);
                    }
                    else
                    {
                        retAccountList[account.Token].Balance += account.Balance;
                    }
                }

                return Ok(retAccountList);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{coin}/balance/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BalanceModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(ErrorModel))]

        public async Task<IActionResult> GetBalance(string coin, string address)
        {
            
            try
            {
                return Ok(await GetBalanceInternal(coin, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
        [HttpPost("{coin}/balances")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BalanceModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetBalances(string coin, AddressesBodyRequest addresses)
        {
            try
            {
                var ret = new List<BalanceModel>();

                foreach (var address in addresses.Addresses)
                {
                    ret.Add(await GetBalanceInternal(coin, address));
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }


        [HttpGet("{coin}/account/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<AccountModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetAccount(string coin, string address)
        {

            try
            {
                return Ok(await GetAccountInternal(coin, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
        
        [HttpPost("{coin}/accounts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<Account>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetAccounts(string coin, AddressesBodyRequest request)
        {
            try
            {
                var retList = new List<Account>();

                foreach (var address in request.Addresses)
                {
                    var accountModel = await GetAccountInternal(coin, address);

                    if (accountModel.Count > 0)
                    {
                        var account = new Account
                        {
                            Address = address,
                            Accounts = accountModel
                        };
                        retList.Add(account);
                    }
                }

                return Ok(retList);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }



        private async Task<List<TransactionModel>> GetTransactionsInternal(string coin, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/address/{address}/txs");

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<List<TransactionModel>>(data);

            return obj;

        }


        [HttpGet("{coin}/txs/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTransactions(string coin, string address)
        {
            try
            {
                return Ok(await GetTransactionsInternal(coin, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost("{coin}/txs")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetMultiTransactions(string coin, AddressesBodyRequest request)
        {
            try
            {
                var ret = new List<TransactionModel>();

                foreach (var address in request.Addresses)
                {
                    ret.AddRange(await GetTransactionsInternal(coin, address));
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{coin}/unspent/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetUnspentTransactionOutput(string coin, string address)
        {
            try
            {
                return Ok(await GetUnspentTransactionOutputsInternal(coin, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost("{coin}/unspent")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetMultiUnspentTransactionOutput(string coin, AddressesBodyRequest request)
        {
            try
            {
                var ret = new List<TransactionModel>();

                foreach (var address in request.Addresses)
                {
                    ret.AddRange(await GetUnspentTransactionOutputsInternal(coin, address));
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        private async Task<List<TransactionModel>> GetUnspentTransactionOutputsInternal(string coin, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/address/{address}?unspent=true");

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<List<TransactionModel>>(data);

            return obj;

        }

        [HttpGet("{coin}/fee")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeeEstimateModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetEstimateFee(string coin)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/fee/30");
            try
            {
                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<FeeEstimateModel>(data);

                return Ok(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
