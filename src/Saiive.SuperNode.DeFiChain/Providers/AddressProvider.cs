using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.DeFiChain.Sharp.Parser;
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
        private readonly IMasterNodeCache _masterNodeCache;

        public AddressProvider(ILogger<AddressProvider> logger, IConfiguration config, ITokenStore tokenStore, IMasterNodeCache masterNodeCache) : base(logger, config)
        {
            _logger = logger;
            _tokenStore = tokenStore;
            _masterNodeCache = masterNodeCache;
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
            var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/address/{address}/balance");

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

            var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/address/{address}/tokens");

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
            var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/transactions/{txId}");
            var responseVouts = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/transactions/{txId}/vouts");
            var responseVins = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/transactions/{txId}/vins");

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
                ulong value = 0;
                var mintTxId = "";
                int mintIndex = 0;
                var script = "";
                if (String.IsNullOrEmpty(vi.Coinbase))
                {
                    value = vi.Vout == null ? 0 : Convert.ToUInt64(Convert.ToDouble(vi.Vout.Value, CultureInfo.InvariantCulture) * token.Multiplier, CultureInfo.InvariantCulture);
                    mintTxId = vi.Vout == null ? null : vi.Vout.Txid;
                    mintIndex = vi.Vout == null ? -1 : vi.Vout.N;
                    script = vi.Script.Hex;
                }
                else
                {
                    value = 100;
                    mintTxId = vi.Txid;
                    script = null;
                }

                ret.Inputs.Add(new TransactionModel
                {
                    Id = vi.Id,
                    MintIndex = mintIndex,
                    Value = value,
                    Script = script,
                    MintTxId = mintTxId,
                    SpentTxId = vi.Txid
            });
               ; 
            }

            foreach (var vo in vout.Data)
            {
                //
                var voutTx = new TransactionModel
                {
                    Id = vo.Txid,
                    Value = Convert.ToUInt64(Convert.ToDouble(vo.Value, CultureInfo.InvariantCulture) * token.Multiplier, CultureInfo.InvariantCulture),
                    Script = vo.Script.Hex,
                    MintTxId = vo.Txid,
                    Coinbase = vo.Script.Type == "nulldata",
                    MintIndex = vo.N
                };

                if(vo.Script.Hex.Contains("44665478"))
                {
                    voutTx.IsCustom = true;
                    voutTx.Address = "false";

                    var startIndexOfDfTx = vo.Script.Hex.IndexOf("44665478");

                    var parsedTx = DefiScriptParser.Parse(vo.Script.Hex.Substring(startIndexOfDfTx).ToByteArray());

                    voutTx.TxType = Convert.ToChar(parsedTx.TxType).ToString();
                }
                else
                {
                    voutTx.Address = NBitcoin.Script.FromHex(vo.Script.Hex)?.GetDestinationAddress(Helper.GetNBitcoinNetwork(network))?.ToString();
                }


                ret.Outputs.Add(voutTx);
            }


            return ret;
        }

        private async Task<List<TransactionModel>> GetTransactionsInternal(string network, string address)
        { var url = $"{OceanUrl}/{ApiVersion}/{network}/address/{address}/transactions";

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
                Type = tx.Type,
                BlockTime = UnixTimeToDateTime(tx.Block.Time)
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
            var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/address/{address}/transactions/unspent");

            var data = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();

                var txs = JsonConvert.DeserializeObject<OceanTransactions>(data);
                var useTxs = new List<OceanTransactionData>();

                foreach (var tx in txs.Data)
                {
                    var useTx = true;
                    if (Convert.ToDouble(tx.Vout.Value, CultureInfo.InvariantCulture) == 20000.0)
                    {
                        var txDetails = await GetTransactionDetails(network, tx.Vout.Txid);
                        foreach (var outp in txDetails.Outputs)
                        {
                            if (!String.IsNullOrEmpty(outp.Script) && outp.Script[0..2] == "6a")
                            {
                                var script = outp.Script.ToByteArray()[2..];
                                if (DefiScriptParser.IsDeFiTransaction(script))
                                {
                                    var defiTx = DefiScriptParser.Parse(script);

                                    if (defiTx.TxType == DefiTransactions.CustomTxType.CreateMasternode)
                                    {
                                        if(await _masterNodeCache.IsMasternodeStillAlive(network, address, tx.Vout.Txid))
                                        {
                                            useTx = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (useTx)
                    {
                        useTxs.Add(tx);
                    }
                }

                return await ConvertOceanModel(useTxs, network, address);
            }
            catch
            {
                throw new Exception(data);
            }
        }

        public async Task<AggregatedAddress> GetAggregatedAddress(string network, string address)
        {
            var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/address/{address}/aggregation");

            var data = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();

                var agg = JsonConvert.DeserializeObject<OceanAggregatedAddress>(data);

                return agg.Data;
            }
            catch
            {
                throw new Exception(data);
            }
        }

        public async Task<IList<AggregatedAddress>> GetAggregatedAddresses(string network, AddressesBodyRequest addresses)
        {
            var ret = new List<AggregatedAddress>();

            foreach(var address in addresses.Addresses)
            {
                ret.Add(await GetAggregatedAddress(network, address));
            }

            return ret;
            
            
        }


        public async Task<IList<LoanVault>> GetLoanVaultsForAddress(string network, string address)
        {
            var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/address/{address}/vaults");

            var data = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw new ArgumentException(data);
            }


            var obj = JsonConvert.DeserializeObject<OceanDataEntity<List<LoanVault>>>(data);
            return obj.Data;
        }

        public async Task<IList<LoanVault>> GetLoanVaultsForAddresses(string network, IList<string> addresses)
        {
           var ret = new List<LoanVault>();

            foreach(var address in addresses)
            {
                ret.AddRange(await GetLoanVaultsForAddress(network, address));  
            }

            return ret;
        }
    }

}
