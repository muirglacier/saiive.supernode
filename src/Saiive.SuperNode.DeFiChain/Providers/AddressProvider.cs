using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

                var token = await _tokenStore.GetToken(network, acc.SymbolKey);

                var account = new AccountModel
                {
                    Address = address,
                    Raw = acc.Amount,
                    Balance = Convert.ToDouble(acc.Amount, CultureInfo.InvariantCulture) * token.Multiplier,
                    Token = acc.SymbolKey,
                    IsDAT = acc.IsDat,
                    IsLPS = acc.IsLps,
                    SymbolKey = acc.SymbolKey
                };

                ret.Add(account);
            }


            var nativeBalance = await GetBalanceInternal(network, address);
            if (nativeBalance.Balance > 0)
            {
                ret.Add(new AccountModel
                {
                    Address = address,
                    Balance = nativeBalance.Balance,
                    Raw = $"{nativeBalance.Balance}@DFI",
                    Token = $"$DFI"
                });
            }


            return ret;

        }

     
        internal async Task<TransactionDetailModel> GetTransactionDetails(string network, string txId)
        {
            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/transactions/{txId}");
            var responseVouts = await _client.GetAsync($"{OceanUrl}/v0/{network}/transactions/{txId}/vouts");
            var responseVins = await _client.GetAsync($"{OceanUrl}/v0/{network}/transactions/{txId}/vins");

            var data = await response.Content.ReadAsStringAsync();
            var vouts = await responseVouts.Content.ReadAsStringAsync();
            var vins = await responseVins.Content.ReadAsStringAsync();

            var tx = JsonConvert.DeserializeObject<OceanTransactionDetail>(data);
            var vinsObj = JsonConvert.DeserializeObject<OceanVInDetail>(vins);
            var voutsObj = JsonConvert.DeserializeObject<OceanVOutDetail>(vouts);

            return await ConvertOceanTranscationDetails(network, tx, vinsObj, voutsObj);
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
                    Id = vi.Id,
                    MintIndex = vi.Vout == null ? -1 :vi.Vout.N,
                    Value = vi.Vout == null ? 0 : Convert.ToUInt64(Convert.ToDouble(vi.Vout.Value, CultureInfo.InvariantCulture) * token.Multiplier, CultureInfo.InvariantCulture),
                    Script = vi.Script.Hex,
                    MintTxId = vi.Vout == null ? null : vi.Vout.Txid,
                    SpentTxId = vi.Txid,
                    Address = NBitcoin.Script.FromHex(vi.Script.Hex).GetDestinationAddress(Helper.GetNBitcoinNetwork(network)).ToString()
            });
               ; 
            }

            foreach (var vo in vout.Data)
            {
                ret.Outputs.Add(new TransactionModel
                {
                    Id = vo.Id,
                    Value = Convert.ToUInt64(Convert.ToDouble(vo.Value, CultureInfo.InvariantCulture) * token.Multiplier, CultureInfo.InvariantCulture),
                    Script = vo.Script.Hex,
                    MintTxId = vo.Txid,
                    Coinbase = vo.Script.Type == "nulldata",
                    MintIndex = vo.N,
                    Address = NBitcoin.Script.FromHex(vo.Script.Hex).GetDestinationAddress(Helper.GetNBitcoinNetwork(network)).ToString()
                });
            }


            return ret;
        }

        private async Task<List<TransactionModel>> GetTransactionsInternal(string network, string address)
        { var url = $"{OceanUrl}/v0/{network}/address/{address}/transactions";

            var getAllTxs = await Helper.LoadAllFromPagedRequest<OceanTransactionData>(url);


            return await ConvertOceanModel(getAllTxs, network, address);

        }

        internal async Task<TransactionModel> ConvertOceanModel(OceanTransactionData tx, string network, string address)
        {
            var token = await _tokenStore.GetToken(network, "DFI");
        
            var valueProp = String.IsNullOrEmpty(tx.Value) ? tx.Vout.Value : tx.Value;
            var txType = tx.Type;

            return new TransactionModel
            {
                Address = address,
                Chain = "DFI",
                Id = tx.Id,
                MintHeight = (tx.Type == "vout" ? tx.Block.Height : -1),
                MintIndex = String.IsNullOrEmpty(tx.Type) ? (tx.Vout?.N ?? -1) : (tx.Type == "vout" ? tx.Vout.N : -1),
                MintTxId = String.IsNullOrEmpty(tx.Type) ? (tx.Vout?.Txid ?? "") : (tx.Type == "vout" ? tx.Vout.Txid : ""),
                Network = network,
                Script = tx.Script.Hex,
                SpentTxId = String.IsNullOrEmpty(tx.Type) ? "" : (tx.Type == "vin" ? tx.Vin.Txid : ""),
                SpentHeight = tx.Type == "vin" ? tx.Block.Height : -1,
                Value = Convert.ToUInt64(Convert.ToDouble(valueProp, CultureInfo.InvariantCulture) * token.Multiplier, CultureInfo.InvariantCulture),
                Type = tx.Type
            };

        }

        internal async Task<List<TransactionModel>> ConvertOceanModel(List<OceanTransactionData> oceanTransactions, string network, string address)
        {
            var ret = new List<TransactionModel>();

            var token = await _tokenStore.GetToken(network, "DFI");

            foreach (var tx in oceanTransactions)
            {
                var valueProp = String.IsNullOrEmpty(tx.Value) ? tx.Vout.Value : tx.Value;
                var txType = tx.Type;

                ret.Add(await ConvertOceanModel(tx, network, address));

            }

            return ret;
        }

        private async Task<List<TransactionModel>> GetUnspentTransactionOutputsInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/address/{address}/transactions/unspent");

            var data = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();

                var txs = JsonConvert.DeserializeObject<OceanTransactions>(data);

                return await ConvertOceanModel(txs.Data, network, address);
            }
            catch
            {
                throw new Exception(data);
            }
        }
    }

}
