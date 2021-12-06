using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using Saiive.SuperNode.Bitcoin.Helper;
using System.Linq;

namespace Saiive.SuperNode.Bitcoin.Providers
{
    internal class TransactionProvider : BaseBitcoinProvider, ITransactionProvider
    {
        private const string coin = "BTC";
        public TransactionProvider(ILogger<TransactionProvider> logger, IConfiguration config) : base(logger, config)
        {
        }


        private async Task<TransactionDetailModel> GetTransactionDetails(string coin, string network, string txId)
        {
            try
            {
                var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/tx/{txId}/coins");

                var data = await response.Content.ReadAsStringAsync();
                var tx = JsonConvert.DeserializeObject<TransactionDetailModel>(data);
                return tx;
            }
            catch
            {
                return null;
            }
        }

        public async Task<TransactionModel> GetTransactionById(string network, string txId, bool onlyConfirmed)
        {
            try
            {
                var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/tx/{txId}");

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<TransactionModel>(data);
                obj.Details = await GetTransactionDetails(coin, network, txId);
                return obj;
            }
            catch
            {
                return await GetTransactionByIdCypher(network, txId);
            }
        }

        public async Task<IList<TransactionModel>> GetTransactionsByBlock(string network, string block)
        {
            try
            {
                var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/tx?blockHash={block}");
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<List<TransactionModel>>(data);
                return obj;
            }
            catch
            {
                return await GetTransactionsByBlockCypher(network, block);
            }
        }

        public async Task<IList<BlockTransactionModel>> GetTransactionsByBlockHeight(string network, int height, bool includeDetails)
        {
            try
            {
                var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/tx?blockHeight={height}&limit=7");

                var data = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();


                var obj = JsonConvert.DeserializeObject<List<BlockTransactionModel>>(data);
                if (obj != null && includeDetails)
                {
                    foreach (var tx in obj)
                    {
                        tx.Details = await GetTransactionDetails(coin, network, tx.Txid);
                    }
                }
                return obj;
            }
            catch
            {
                return await GetTransactionsByBlockHeightCypher(network, height, includeDetails);
            }
        }

        public async Task<string> SendRawTransaction(string network, TransactionRequest request)
        {
            try
            {
                var httpContent =
                    new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"{ApiUrl}/api/{coin}/{network}/tx/send", httpContent);

                var data = await response.Content.ReadAsStringAsync();
                try
                {
                    response.EnsureSuccessStatusCode();
                    var obj = JsonConvert.DeserializeObject<TransactionResponse>(data);

                    Logger.LogInformation("{coin}+{network}: Committed tx to blockchain, {txId} {txHex}", coin, network, obj?.TxId, request.RawTx);


                    return obj.TxId;
                }
                catch
                {
                    throw new ArgumentException(data);
                }
            }
            catch
            {
                return await SendRawTransactionCypher(network, request);
            }
        }


        public async Task<TransactionModel> GetTransactionByIdCypher(string network, string txId)
        {
            var instance = GetInstance(network);
            var transactions = await instance.GetTransactionByHash(txId);

            return transactions.ToTransactionModel(network, transactions.Addresses.First());
        }

        public async Task<TransactionModel> GetTransactionByIdBlockStream(string network, string txId)
        {
            var client = GetEsplora(network);

            var tx = await client.GetTransaction(txId);

            return tx.ToTransactionModel(network, "");
        }

        public async Task<IList<TransactionModel>> GetTransactionsByBlockCypher(string network, string block)
        {
            var instance = GetInstance(network);
            var ret = new List<TransactionModel>();

            var transactions = await instance.GetTransactionsByBlockHash(block);

            foreach (var tx in transactions)
            {
                ret.Add(tx.ToTransactionModel(network, ""));
            }


            return ret;
        }

        public async Task<IList<BlockTransactionModel>> GetTransactionsByBlockHeightCypher(string network, int height, bool includeDetails)
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

        public async Task<string> SendRawTransactionCypher(string network, TransactionRequest request)
        {
            var instance = GetInstance(network);


            var ret = await instance.SendRawTransaction(request.RawTx);
            return ret.Hash;
        }

        public Task<object> DecodeRawTransaction(string network, TransactionRequest rawTx)
        {
            throw new System.NotImplementedException();
        }

        public Task<object> DecodeRawTransactionFromTxId(string network, string txId)
        {
            throw new System.NotImplementedException();
        }
    }
}
