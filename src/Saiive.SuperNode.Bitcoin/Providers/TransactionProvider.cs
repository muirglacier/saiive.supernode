using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saiive.SuperNode.Bitcoin.Helper;

namespace Saiive.SuperNode.Bitcoin.Providers
{
    internal class TransactionProvider : BaseBitcoinProvider, ITransactionProvider
    {
        public TransactionProvider(ILogger<TransactionProvider> logger, IConfiguration config) : base(logger, config)
        {
        }

        public async Task<TransactionModel> GetTransactionById(string network, string txId)
        {
            var instance = GetInstance(network);
            var transactions = await instance.GetTransactionByHash(txId);

            return transactions.ToTransactionModel(network, transactions.Addresses.First());        
        }

        public async Task<IList<TransactionModel>> GetTransactionsByBlock(string network, string block)
        {
            var instance = GetInstance(network);
            var ret = new List<TransactionModel>();

            var transactions = await instance.GetTransactionsByBlockHash(block);

            foreach(var tx in transactions)
            {
                ret.Add(tx.ToTransactionModel(network, ""));
            }


            return ret;
        }

        public async Task<IList<BlockTransactionModel>> GetTransactionsByBlockHeight(string network, int height, bool includeDetails)
        {
            var instance = GetInstance(network);
            var ret = new List<BlockTransactionModel>();

            var transactions = await instance.GetTransactionsByBlockHeight(height);

            foreach (var tx in transactions)
            {
                ret.Add(tx.ToBlockTransactionModel(network, ""));
            }


            return ret;
        }

        public async Task<string> SendRawTransaction(string network, TransactionRequest request)
        {
            var instance = GetInstance(network);


            var ret = await instance.SendRawTransaction(request.RawTx);
            return ret.Hash;
        }
    }
}
