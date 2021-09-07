using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.DeFiChain.Sharp.Parser;
using Saiive.DeFiChain.Sharp.Parser.Txs;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Application;
using Saiive.SuperNode.DeFiChain.Ocean;
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
            return await GetAccountInternal(network, address);
        }

        public async Task<IList<Account>> GetAccount(string network, AddressesBodyRequest addresses)
        {
            var retList = new List<Account>();

            foreach (var address in addresses.Addresses)
            {
                var accountModel = await GetAccountInternal(network, address);

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
            return await GetBalanceInternal(network, address);
        }

        public async Task<IList<BalanceModel>> GetBalance(string network, AddressesBodyRequest addresses)
        {
            var ret = new List<BalanceModel>();

            foreach (var address in addresses.Addresses)
            {
                ret.Add(await GetBalanceInternal(network, address));
            }
            return ret;
        }

        public async Task<IList<AccountModel>> GetTotalBalance(string network, string address)
        {
            var balanceTokens = await GetAccountInternal(network, address);

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
                var accountModel = await GetAccountInternal(network, address);
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
            return await GetTransactionsInternal(network, address);
        }

        public async Task<IList<TransactionModel>> GetTransactions(string network, AddressesBodyRequest addresses)
        {
            var ret = new List<TransactionModel>();

            foreach (var address in addresses.Addresses)
            {
                ret.AddRange(await GetTransactionsInternal(network, address));
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


        private async Task<BalanceModel> GetBalanceInternal(string network, string address)
        {
            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/address/{address}/balance");

            var data = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw new ArgumentException(data);
            }


            var obj = JsonConvert.DeserializeObject<OceanBalance>(data);
            

            var dfiToken = await _tokenStore.GetToken(network, "DFI");

            return new BalanceModel
            {
                Address = address,
                Balance = Convert.ToUInt64(Convert.ToDouble(obj.Data, CultureInfo.InvariantCulture) * dfiToken.Multiplier),
                Confirmed = Convert.ToUInt64(Convert.ToDouble(obj.Data, CultureInfo.InvariantCulture) * dfiToken.Multiplier),
                Unconfirmed = Convert.ToUInt64(Convert.ToDouble(obj.Data, CultureInfo.InvariantCulture) * dfiToken.Multiplier),
            };



        }

        private async Task<IList<AccountModel>> GetAccountInternal(string network, string address)
        {
            var ret = new List<AccountModel>();

            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/address/{address}/tokens");

            var data = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw new ArgumentException(data);
            }

            var obj = JsonConvert.DeserializeObject<OceanDataEntity<List<OceanTokens>>>(data);


            foreach (var acc in obj.Data)
            {

                var token = await _tokenStore.GetToken(network, acc.Symbol);

                var account = new AccountModel
                {
                    Address = address,
                    Raw = acc.Amount,
                    Balance = Convert.ToDouble(acc.Amount, CultureInfo.InvariantCulture) * token.Multiplier,
                    Token = acc.DisplaySymbol
                };

                ret.Add(account);
            }


            var nativeBalance = await GetBalanceInternal(network, address);

            ret.Add(new AccountModel
            {
                Address = address,
                Balance = nativeBalance.Balance,
                Raw = $"{nativeBalance.Balance}@DFI",
                Token = $"$DFI"
            });



            return ret;

        }

        internal async Task<TransactionDetailModel> GetTransactionDetails(string coin, string network, string txId)
        {
            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/transactions/{txId}");
            var responseVouts = await _client.GetAsync($"{OceanUrl}/v0/{network}/transactions/{txId}/vouts");
            var responseVins = await _client.GetAsync($"{OceanUrl}/v0/{network}/transactions/{txId}/vins");

            var data = await response.Content.ReadAsStringAsync();
            var vouts = await responseVouts.Content.ReadAsStringAsync();
            var vins = await responseVins.Content.ReadAsStringAsync();

            var tx = JsonConvert.DeserializeObject<OceanTransactionDetail>(data);
            var voutsObj = JsonConvert.DeserializeObject<OceanVInDetail>(data);
            var vinsObj = JsonConvert.DeserializeObject<OceanVOutDetail>(data);

            return await ConvertOceanTranscationDetails(network, tx, voutsObj, vinsObj);
        }

        private async Task<TransactionDetailModel> ConvertOceanTranscationDetails(string network, OceanTransactionDetail tx, OceanVInDetail vin, OceanVOutDetail vout)
        {
            var token = await _tokenStore.GetToken(network, "DFI");
            var ret = new TransactionDetailModel
            {
                Inputs = new List<TransactionModel>(),
                Outputs = new List<TransactionModel>()
            };

            foreach (var vi in vin.Data)
            {
                ret.Inputs.Add(new TransactionModel
                {
                    Value = Convert.ToUInt64(Convert.ToDouble(vi.Vout.Value, CultureInfo.InvariantCulture) * token.Multiplier, CultureInfo.InvariantCulture),
                    Script = vi.Script.Hex,
                    SpentTxId = vi.Vout.Txid,
                    MintTxId = vi.Txid
                });
            }

            foreach (var vo in vout.Data)
            {
                ret.Outputs.Add(new TransactionModel
                {
                    Value = Convert.ToUInt64(Convert.ToDouble(vo.Vout.Value, CultureInfo.InvariantCulture) * token.Multiplier, CultureInfo.InvariantCulture),
                    Script = vo.Script.Hex,
                    SpentTxId = vo.Vout.Txid,
                    MintTxId = vo.Txid,
                    Coinbase = vo.Script.Type == "nulldata"
                });
            }


            return ret;
        }

        private async Task<List<TransactionModel>> GetTransactionsInternal(string network, string address)
        {
            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/address/{address}/transactions");

            var data = await response.Content.ReadAsStringAsync();

            var txs = JsonConvert.DeserializeObject<OceanTransactions>(data);

            return await ConvertOceanModel(txs, network, address);

        }

        private async Task<List<TransactionModel>> ConvertOceanModel(OceanTransactions oceanTransactions, string network, string address)
        {
            var ret = new List<TransactionModel>();

            var token = await _tokenStore.GetToken(network, "DFI");

            foreach (var tx in oceanTransactions.Data)
            {
                var valueProp = String.IsNullOrEmpty(tx.Value) ? tx.Vout.Value : tx.Value;
                var txType = tx.Type;

                ret.Add(new TransactionModel
                {
                    Address = address,
                    Chain = "DFI",
                    Id = tx.Id,
                    MintHeight = (tx.Type == "vout" ? tx.Block.Height : -1),
                    MintIndex = String.IsNullOrEmpty(tx.Type) ? -1 : (tx.Type == "vout" ? tx.Vout.N : -1),
                    MintTxId = String.IsNullOrEmpty(tx.Type) ? null : (tx.Type == "vout" ? tx.Vout.Txid : null),
                    Network = network,
                    Script = tx.Script.Hex,
                    SpentTxId = String.IsNullOrEmpty(tx.Type) ? null : (tx.Type == "vin" ? tx.Vin.Txid : null),
                    SpentHeight = tx.Type == "vin" ? tx.Block.Height : -1,
                    Value = Convert.ToUInt64(Convert.ToDouble(valueProp, CultureInfo.InvariantCulture) * token.Multiplier, CultureInfo.InvariantCulture),
                    Type = tx.Type
                });

            }

            return ret;
        }

        private async Task<List<TransactionModel>> GetUnspentTransactionOutputsInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/address/{address}/transactions/unspent");

            var data = await response.Content.ReadAsStringAsync();

            var txs = JsonConvert.DeserializeObject<OceanTransactions>(data);

            return await ConvertOceanModel(txs, network, address);
        }
    }

}
