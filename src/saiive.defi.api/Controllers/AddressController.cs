using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;
using saiive.defi.api.Requests;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class AddressController : BaseController
    {
        

        public AddressController(ILogger<AddressController> logger, IConfiguration config) : base(logger, config)
        {
        }
        
        private async Task<BalanceModel> GetBalanceInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/address/{address}/balance");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<BalanceModel>(data);
            obj.Address = address;

            return obj;

        }
        
        private async Task<List<AccountModel>> GetAccountInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/address/{address}/account");

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

            var nativeBalance = await GetBalanceInternal(coin, network, address);
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


        [HttpGet("{network}/{coin}/balance-all/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BalanceModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]

        public async Task<IActionResult> GetTotalBalance(string coin, string network, string address)
        {

            try
            {
                var balanceNative = await GetBalanceInternal(coin, network, address);
                var balanceTokens = await GetAccountInternal(coin, network, address);
                
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
        [HttpPost("{network}/{coin}/balance-all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dictionary<string, AccountModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTotalBalance(string coin, string network, AddressesBodyRequest addresses)
        {
            try
            {
                var retAccountList = new Dictionary<string, AccountModel>();
                var accounts = new Dictionary<string, List<AccountModel>>();

                foreach (var address in addresses.Addresses)
                {
                    var accountModel = await GetAccountInternal(coin, network, address);
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
                    var balanceNative = await GetBalanceInternal(coin, network, address);
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

        [HttpGet("{network}/{coin}/balance/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BalanceModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(ErrorModel))]

        public async Task<IActionResult> GetBalance(string coin, string network, string address)
        {
            
            try
            {
                return Ok(await GetBalanceInternal(coin, network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
        [HttpPost("{network}/{coin}/balances")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BalanceModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetBalances(string coin, string network, AddressesBodyRequest addresses)
        {
            try
            {
                var ret = new List<BalanceModel>();

                foreach (var address in addresses.Addresses)
                {
                    ret.Add(await GetBalanceInternal(coin, network, address));
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }


        [HttpGet("{network}/{coin}/account/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<AccountModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetAccount(string coin, string network, string address)
        {

            try
            {
                return Ok(await GetAccountInternal(coin, network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
        
        [HttpPost("{network}/{coin}/accounts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<Account>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetAccounts(string coin, string network, AddressesBodyRequest request)
        {
            try
            {
                var retList = new List<Account>();

                foreach (var address in request.Addresses)
                {
                    var accountModel = await GetAccountInternal(coin, network, address);

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



        private async Task<List<TransactionModel>> GetTransactionsInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/address/{address}/txs");

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<List<TransactionModel>>(data);

            return obj;

        }


        [HttpGet("{network}/{coin}/txs/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTransactions(string coin, string network, string address)
        {
            try
            {
                return Ok(await GetTransactionsInternal(coin, network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost("{network}/{coin}/txs")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetMultiTransactions(string coin, string network, AddressesBodyRequest request)
        {
            try
            {
                var ret = new List<TransactionModel>();

                foreach (var address in request.Addresses)
                {
                    ret.AddRange(await GetTransactionsInternal(coin, network, address));
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/unspent/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetUnspentTransactionOutput(string coin, string network, string address)
        {
            try
            {
                return Ok(await GetUnspentTransactionOutputsInternal(coin, network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost("{network}/{coin}/unspent")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetMultiUnspentTransactionOutput(string coin, string network, AddressesBodyRequest request)
        {
            try
            {
                var ret = new List<TransactionModel>();

                foreach (var address in request.Addresses)
                {
                    ret.AddRange(await GetUnspentTransactionOutputsInternal(coin, network, address));
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        private async Task<List<TransactionModel>> GetUnspentTransactionOutputsInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/address/{address}?unspent=true");

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<List<TransactionModel>>(data);

            return obj;

        }

        [HttpGet("{network}/{coin}/fee")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeeEstimateModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetEstimateFee(string coin, string network)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/fee/30");
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
