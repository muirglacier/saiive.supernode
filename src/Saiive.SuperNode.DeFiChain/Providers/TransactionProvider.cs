using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class TransactionProvider : BaseDeFiChainProvider, ITransactionProvider
    {
        public TransactionProvider(ILogger<TransactionProvider> logger, IConfiguration config) : base(logger, config)
        {
        }

        public async Task<TransactionModel> GetTransactionById(string network, string txId)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/DFI/{network}/tx/{txId}");

            
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<TransactionModel>(data);
                obj.Details = await GetTransactionDetails("DFI", network, txId);
            return obj;
            
        }

        public async Task<IList<TransactionModel>> GetTransactionsByBlock(string network, string block)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/DFI/{network}/tx?blockHash={block}");


            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<List<TransactionModel>>(data);
            return obj;

        }

        public async Task<IList<BlockTransactionModel>> GetTransactionsByBlockHeight(string network, int height, bool includeDetails)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/DFI/{network}/tx?blockHeight={height}");

           
                var data = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();


                var obj = JsonConvert.DeserializeObject<List<BlockTransactionModel>>(data);
                if (obj != null && includeDetails)
                {
                    foreach (var tx in obj)
                    {
                        tx.Details = await GetTransactionDetails("DFI", network, tx.Txid);
                    }
                }

                return obj;
            
        }

        public async Task<string> SendRawTransaction(string network, TransactionRequest request)
        {
            var httpContent =
                 new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{ApiUrl}/api/DFI/{network}/tx/send", httpContent);

            var data = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();
                var obj = JsonConvert.DeserializeObject<TransactionResponse>(data);

                Logger.LogInformation("{coin}+{network}: Committed tx to blockchain, {txId} {txHex}", "DFI", network, obj?.TxId, request.RawTx);

                return obj.TxId;
            }
            catch
            {
                throw new ArgumentException(data);
            }

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
    }
}
