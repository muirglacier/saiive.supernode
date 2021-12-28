using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Ocean;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class TransactionProvider : BaseDeFiChainProvider, ITransactionProvider
    {
        private readonly AddressProvider _addressProvider;
        private readonly BlockProvider _blockProvider;
        private readonly AddressTransactionDetailProvider _txProvider;

        public TransactionProvider(ILogger<TransactionProvider> logger, IConfiguration config, AddressProvider addressProvider, BlockProvider blockProvider, AddressTransactionDetailProvider txDetailProvider) : base(logger, config)
        {
            _addressProvider = addressProvider;
            _blockProvider = blockProvider;
            _txProvider = txDetailProvider;
        }

        private TransactionModel ConvertOceanModel(OceanDataEntity<OceanTransactionDetailData> data)
        {


            var tx = new TransactionModel
            {
                Id = data.Data.Id,
                BlockHeight = data.Data.Block.Height,
                BlockHash = data.Data.Block.Hash,
                BlockTime = UnixTimeToDateTime(data.Data.Block.Time)
            };
            return tx;


        }
        private async Task<TransactionDetailModel> GetLegacyTransactionDetails(string coin, string network, string txId)
        {

            try
            {
                var response = await _client.GetAsync($"{LegacyBitcoreUrl}api/{coin}/{network}/tx/{txId}/coins");

                var data = await response.Content.ReadAsStringAsync();
                var tx = JsonConvert.DeserializeObject<TransactionDetailModel>(data);

                foreach (var o in tx.Outputs)
                {
                    if (!o.Script.Contains("44665478"))
                    {
                        try
                        {
                            o.Address = NBitcoin.Script.FromHex(o.Script)?.GetDestinationAddress(Helper.GetNBitcoinNetwork(network))?.ToString();
                        }
                        catch
                        {
                            //ignore
                        }
                    }
                }
                return tx;
            }
            catch
            {
                return null;
            }
        }


        public async Task<TransactionModel> GetTransactionById(string network, string txId, bool onlyConfirmed)
        {
            var detailModel = await _addressProvider.GetTransactionDetails(network, txId);


            var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/transactions/{txId}");

            try
            {

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<OceanDataEntity<OceanTransactionDetailData>>(data);
                var ret = ConvertOceanModel(obj);
                ret.Details = detailModel;
                ret.Coinbase = detailModel.Inputs.Any(a => a.Coinbase);

                return ret;
            }
            catch(Exception)
            {
                if(onlyConfirmed)
                {
                    throw;
                }

                var responseLegacy = await _client.GetAsync($"{ApiUrl}api/v1/{network}/DFI/tx/id/{txId}");

                responseLegacy.EnsureSuccessStatusCode();

                var data = await responseLegacy.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<TransactionModel>(data);
                obj.Details = await GetLegacyTransactionDetails("DFI", network, txId);

                obj.Id = obj.TxId;

                return obj;
            }
        }

        public async Task<IList<TransactionModel>> GetTransactionsByBlock(string network, string block)
        {
            var txs = await _blockProvider.GetTransactionForBlock(network, block);
            return txs;
        }

        public async Task<IList<BlockTransactionModel>> GetTransactionsByBlockHeight(string network, int height, bool includeDetails)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/tx/block/{height}/{includeDetails}", async () =>
            {
                var blockInstance = await _blockProvider.GetBlockByHeightOrHash(network, height.ToString());

                var txs = await _blockProvider.GetTransactionForBlock(network, blockInstance.Hash);

                var ret = new List<BlockTransactionModel>();

                foreach (var tx in txs)
                {
                    ret.Add(await _txProvider.GetBlockTransaction(network, tx.Id));
                }

                return ret;
            }, null);

        }

        public async Task<string> SendRawTransaction(string network, TransactionRequest request)
        {
            var body = new OceanRawTx
            {
                Hex = request.RawTx
            };
            var httpContent =
                 new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{OceanUrl}/{ApiVersion}/{network}/rawtx/send", httpContent);

            try
            {
              
                var data = await response.Content.ReadAsStringAsync();

                try
                {
                    response.EnsureSuccessStatusCode();
                    var obj = JsonConvert.DeserializeObject<OceanDataEntity<string>>(data);

                    Logger.LogInformation("{coin}+{network}: Committed tx to blockchain, {txId} {txHex}", "DFI", network, obj?.Data, request.RawTx);

                    return obj.Data;
                }
                catch
                {
                    throw new ArgumentException(data);
                }
            }
            catch
            {
                if(response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw;
                }
                var nhttpContent =
                     new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var nresponse = await _client.PostAsync($"{ApiUrl}/v1/{network}/DFI/tx/raw", nhttpContent);

                var data = await nresponse.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<TransactionResponse>(data);

                Logger.LogInformation("{coin}+{network}: Committed tx to blockchain, {txId} {txHex}", "DFI", network, obj?.TxId, request.RawTx);

                return obj.TxId;
            }

        }

        public async Task<object> DecodeRawTransaction(string network, TransactionRequest rawTx)
        {
            var responseLegacy = await _client.PostAsync($"{LegacyBitcoreUrl}api/DFI/{network}/tx/decode", new StringContent(JsonConvert.SerializeObject(rawTx), Encoding.UTF8, "application/json"));

            responseLegacy.EnsureSuccessStatusCode();

            var data = await responseLegacy.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject(data);

            return obj;
        }

        public async Task<object> DecodeRawTransactionFromTxId(string network, string txId)
        {
            var responseLegacy = await _client.GetAsync($"{LegacyBitcoreUrl}api/DFI/{network}/tx/{txId}/decoderaw");

            responseLegacy.EnsureSuccessStatusCode();

            var data = await responseLegacy.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject(data);

            return obj;
        }
    }
}
