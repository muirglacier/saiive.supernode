using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class TransactionController : BaseController
    {
        public TransactionController(ILogger<TransactionController> logger, IConfiguration config) : base(logger, config)
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

        [HttpGet("{network}/{coin}/tx/id/{txId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(TransactionModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTransactionById(string coin, string network, string txId)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/tx/{txId}");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<TransactionModel>(data);
                obj.Details = await GetTransactionDetails(coin, network, txId);
                return Ok(obj);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"tx {txId} could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/tx/block/{block}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTransactionsByBlock(string coin, string network, string block)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/tx?blockHash={block}");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<List<TransactionModel>>(data);
                return Ok(obj);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"block with hash {block} could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }


        [HttpGet("{network}/{coin}/tx/height/{height}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public Task<IActionResult> GetTransactionsByBlockHeight(string coin, string network, int height)
        {
            return GetTransactionsByBlockHeight(coin, network, height, true);
        }

        [HttpGet("{network}/{coin}/tx/height/{height}/{includeDetails}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTransactionsByBlockHeight(string coin, string network, int height, bool includeDetails)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/tx?blockHeight={height}");

            try
            {
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

                return Ok(obj);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"block with height {height} could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        private async Task<BlockModel> GetCurrentHeight(string coin, string network)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/block/tip");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<BlockModel>(data);
            return obj;
        }

        [HttpPost("{network}/{coin}/tx/raw")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TransactionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> SendRawTransaction(string coin, string network, TransactionRequest request)
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
                
                return Ok(obj);
            }
            catch
            {
                var currentBlock = await GetCurrentHeight(coin, network);

                Logger.LogError("{coin}+{network}: Error commiting tx to blockchain ({response} for {txHex}) @ {blockHeight} block", coin, network, data, request.RawTx, currentBlock.Height);
                return BadRequest(new ErrorModel($"{data}"));
            }
        }
    }
}
