using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Application;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class AddressTransactionDetailProvider : BaseDeFiChainProvider, IAddressTransactionDetailProvider
    {
        private readonly ILogger _logger;

        public AddressTransactionDetailProvider(ILogger<AddressTransactionDetailProvider> logger, IConfiguration config) : base(logger, config)
        {
            _logger = logger;
        }

        public async Task<List<BlockTransactionModel>> GetTransactions(string network, string address)
        {
            return await GetTransactionsInternal("DFI", network, address);
        }

        public async Task<List<BlockTransactionModel>> GetTransactions(string network, AddressesBodyRequest addresses)
        {
            var ret = new List<BlockTransactionModel>();

            foreach (var address in addresses.Addresses)
            {
                ret.AddRange(await GetTransactionsInternal("DFI", network, address));
            }
            return ret;
        }

        private async Task<List<BlockTransactionModel>> GetTransactionsInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/address/{address}/txs?limit=1000");

            var data = await response.Content.ReadAsStringAsync();

            var txs = JsonConvert.DeserializeObject<List<TransactionModel>>(data);
            var ret = new List<BlockTransactionModel>();

            foreach (var tx in txs)
            {
                try
                {
                    var vin = await GetBlockTransaction(coin, network, tx.MintTxId);
                    ret.Add(vin);

                    if (tx.SpentHeight > 0)
                    {
                        var vout = await GetBlockTransaction(coin, network, tx.SpentTxId);
                        ret.Add(vout);
                    }


                }
                catch (Exception e)
                {
                    _logger.LogError(e, "error occurred");
                }
            }

            return ret;

        }
        private async Task<BlockTransactionModel> GetBlockTransaction(string coin, string network, string txId)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/tx/{txId}");
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<BlockTransactionModel>(data);
            return obj;
        }

    }
}
