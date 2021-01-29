using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class TransactionController : BaseController
    {
        public TransactionController(ILogger<TransactionController> logger, IConfiguration config) : base(logger, config)
        {
          
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
        public async Task<IActionResult> GetTransactionsByBlockHeight(string coin, string network, int height)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/tx?blockHeight={height}");

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
                    return NotFound(new ErrorModel($"block with height {height} could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
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
                return Ok(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e} ({data})");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
