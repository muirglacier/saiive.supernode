using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Ocean;
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
        private readonly AddressProvider _addressProvider;

        public AddressTransactionDetailProvider(ILogger<AddressTransactionDetailProvider> logger, IConfiguration config, AddressProvider addressProvider) : base(logger, config)
        {
            _logger = logger;
            _addressProvider = addressProvider;
        }

        public async Task<List<BlockTransactionModel>> GetTransactions(string network, string address)
        {
            return await GetTransactionsInternal(network, address);
        }

        public async Task<List<BlockTransactionModel>> GetTransactions(string network, AddressesBodyRequest addresses)
        {
            var ret = new List<BlockTransactionModel>();

            foreach (var address in addresses.Addresses)
            {
                ret.AddRange(await GetTransactionsInternal(network, address));
            }
            return ret;
        }

        private async Task<List<BlockTransactionModel>> GetTransactionsInternal(string network, string address)
        {
            var txs =  await _addressProvider.GetTransactions(network, address);

            var ret = new List<BlockTransactionModel>();


            foreach(var tx in txs)
            {
                ret.Add(new BlockTransactionModel
                {
                    Details = tx.Details,
                    Value = tx.Value
                });
            }

            return ret;

        }
     
        internal async Task<BlockTransactionModel> GetBlockTransaction(string network, string txId)
        {
            var txDetail = await _addressProvider.GetTransactionDetails(network, txId);


            return new BlockTransactionModel
            {
                Txid = txId,
                Details = txDetail
            };
        }

    }
}
