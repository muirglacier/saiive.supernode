using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.DeFiChain.Sharp.Parser;
using Saiive.DeFiChain.Sharp.Parser.Txs;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Application;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class AddressProvider : BaseDeFiChainProvider, IAddressProvider
    {
        private readonly ILogger<AddressProvider> _logger;
        private readonly ITokenStore _tokenStore;

        public AddressProvider(ILogger<AddressProvider> logger, IConfiguration config, ITokenStore tokenStore) : base(logger, config)
        {
            _logger = logger;
            _tokenStore = tokenStore;
        }

        public async Task<IList<AccountModel>> GetAccount(string network, string address)
        {
            return await GetAccountInternal("DFI", network, address);
        }

        public async Task<IList<Account>> GetAccount(string network, AddressesBodyRequest addresses)
        {
            var retList = new List<Account>();

            foreach (var address in addresses.Addresses)
            {
                var accountModel = await GetAccountInternal("DFI", network, address);

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
            return retList;
        }

        public async Task<BalanceModel> GetBalance(string network, string address)
        {
            return await GetBalanceInternal("DFI", network, address);
        }

        public async Task<IList<BalanceModel>> GetBalance(string network, AddressesBodyRequest addresses)
        {
            var ret = new List<BalanceModel>();

            foreach (var address in addresses.Addresses)
            {
                ret.Add(await GetBalanceInternal("DFI", network, address));
            }
            return ret;
        }

        public async Task<IList<AccountModel>> GetTotalBalance(string network, string address)
        {
            var balanceTokens = await GetAccountInternal("DFI", network, address);

            foreach (var account in balanceTokens)
            {
                account.Address = null;
                account.Raw = null;
            }
            return balanceTokens;
        }

        public async Task<IDictionary<string, AccountModel>> GetTotalBalance(string network, AddressesBodyRequest addresses)
        {
            var retAccountList = new Dictionary<string, AccountModel>();
            var accounts = new Dictionary<string, IList<AccountModel>>();

            foreach (var address in addresses.Addresses)
            {
                var accountModel = await GetAccountInternal("DFI", network, address);
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

            return retAccountList;
        }

        public async Task<IList<TransactionModel>> GetTransactions(string network, string address)
        {
            return await GetTransactionsInternal("DFI", network, address);
        }

        public async Task<IList<TransactionModel>> GetTransactions(string network, AddressesBodyRequest addresses)
        {
            var ret = new List<TransactionModel>();

            foreach (var address in addresses.Addresses)
            {
                ret.AddRange(await GetTransactionsInternal("DFI", network, address));
            }
            return ret;
        }

        public async Task<IList<TransactionModel>> GetUnspentTransactionOutput(string network, string address)
        {
            return await GetUnspentTransactionOutputsInternal("DFI", network, address);
        }

        public async Task<IList<TransactionModel>> GetUnspentTransactionOutput(string network, AddressesBodyRequest addresses)
        {
            var ret = new List<TransactionModel>();

            foreach (var address in addresses.Addresses)
            {
                ret.AddRange(await GetUnspentTransactionOutputsInternal("DFI", network, address));
            }
            return ret;
        }


        private async Task<BalanceModel> GetBalanceInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/address/{address}/balance");

            var data = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw new ArgumentException(data);
            }


            var obj = JsonConvert.DeserializeObject<BalanceModel>(data);
            obj.Address = address;

            return obj;

        }

        private async Task<IList<AccountModel>> GetAccountInternal(string coin, string network, string address)
        {
            var ret = new List<AccountModel>();

            if (coin == "DFI")
            {
                var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/address/{address}/account");

                var data = await response.Content.ReadAsStringAsync();

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    throw new ArgumentException(data);
                }

                var obj = JsonConvert.DeserializeObject<List<string>>(data);


                foreach (var acc in obj)
                {
                    var split = acc.Split("@");

                    var token = await _tokenStore.GetToken(coin, network, split[1]);

                    var account = new AccountModel
                    {
                        Address = address,
                        Raw = acc,
                        Balance = Convert.ToDouble(split[0], CultureInfo.InvariantCulture) * token.Multiplier,
                        Token = split[1]
                    };

                    ret.Add(account);
                }
            }

            var nativeBalance = await GetBalanceInternal(coin, network, address);
            if (nativeBalance.Confirmed > 0)
            {
                ret.Add(new AccountModel
                {
                    Address = address,
                    Balance = nativeBalance.Confirmed,
                    Raw = $"{nativeBalance.Confirmed}@{coin}",
                    Token = $"${coin}"
                });
            }


            return ret;

        }

        private async Task<TransactionDetailModel> GetTransactionDetails(string coin, string network, string txId)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/tx/{txId}/coins");

            var data = await response.Content.ReadAsStringAsync();
            var tx = JsonConvert.DeserializeObject<TransactionDetailModel>(data);
            return tx;
        }

        private async Task<List<TransactionModel>> GetTransactionsInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/address/{address}/txs?limit=1000");

            var data = await response.Content.ReadAsStringAsync();

            var txs = JsonConvert.DeserializeObject<List<TransactionModel>>(data);

            var retTxs = await CheckValidTransactions(txs, coin, network);
            return retTxs;

        }

        private async Task<List<TransactionModel>> GetUnspentTransactionOutputsInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/address/{address}?unspent=true&limit=1000");

            var data = await response.Content.ReadAsStringAsync();

            var txs = JsonConvert.DeserializeObject<List<TransactionModel>>(data);
            var retTxs = await CheckValidTransactions(txs, coin, network);
            return retTxs;


        }

        private async Task<List<TransactionModel>> CheckValidTransactions(List<TransactionModel> txs, string coin,
           string network)
        {
            var retTxs = new List<TransactionModel>();

            foreach (var tx in txs)
            {
                if (!tx.Coinbase)
                {

                    var details = await GetTransactionDetails(coin, network, tx.MintTxId);

                    if (details.Inputs == null || details.Inputs.Count == 0)
                    {
                        _logger.LogError($"Found invalid tx at height {tx.MintHeight}. TX inputs already spent, tx will never get confirmed. Ignore it here.... ({tx.MintTxId})");
                        continue;
                    }

                    var useTx = true;
                    foreach (var output in details.Outputs)
                    {
                        var script = output.Script.Substring(4, output.Script.Length - 4);

                        var byteArray = script.ToByteArray();
                        if (!DefiScriptParser.IsDeFiTransaction(byteArray))
                        {
                            continue;
                        }
                        var dfiScript = DefiScriptParser.Parse(byteArray);

                        if (dfiScript is CreateMasterNode)
                        {
                            _logger.LogInformation(
                                $"This transaction is a create masternode tx - we can't spent them, so discard them!");
                            useTx = false;
                            break;
                        }
                    }

                    if (!useTx)
                    {
                        continue;
                    }

                }

                var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/tx/{tx.MintTxId}");
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<BlockTransactionModel>(data);


                if (obj.BlockHeight > 0)
                {
                    tx.Confirmations = obj.Confirmations;

                    tx.IsCustom = obj.IsCustom;
                    tx.IsCustomTxApplied = obj.IsCustomTxApplied;
                    tx.CustomData = obj.CustomData;
                    tx.TxType = obj.TxType;

                    retTxs.Add(tx);
                }
            }

            return retTxs;
        }

    }

}
