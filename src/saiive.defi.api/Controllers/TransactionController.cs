using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("v1/api/")]
    public class TransactionController : BaseController
    {
        public TransactionController(ILogger<TransactionController> logger, IConfiguration config) : base(logger, config)
        {
          
        }

        [HttpGet("{coin}/tx/id/{txId}")]
        public async Task<TransactionModel> GetTransactionById(string coin, string txId)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/tx/{txId}");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<TransactionModel>(data);
                return obj;
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                throw;
            }
        }

        [HttpGet("{coin}/tx/block/{block}")]
        public async Task<List<TransactionModel>> GetTransactionsByBlock(string coin, string block)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/tx?blockHash={block}");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<List<TransactionModel>>(data);
                return obj;
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                throw;
            }
        }

        [HttpGet("{coin}/tx/height/{height}")]
        public async Task<List<TransactionModel>> GetTransactionsByBlockHeight(string coin, int height)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/tx?blockHeight={height}");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<List<TransactionModel>>(data);
                return obj;
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                throw;
            }
        }
    }
}
