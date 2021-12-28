using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.DeFiChain.Application;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;

namespace Saiive.SuperNode.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class AddressTransactionDetailController : BaseController
    {
        private readonly ILogger<AddressTransactionDetailController> _logger;
        private readonly ITokenStore _tokenStore;


        public AddressTransactionDetailController(ILogger<AddressTransactionDetailController> logger, IConfiguration config, ITokenStore tokenStore) : base(logger, config)
        {
            _logger = logger;
            _tokenStore = tokenStore;
        }//BlockTransactionModel


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

        [HttpGet("{network}/{coin}/tx-history/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTransactions(string coin, string network, string address)
        {
            try
            {
                return Ok(await GetTransactionsInternal(coin, network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost("{network}/{coin}/tx-history")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetMultiTransactions(string coin, string network, AddressesBodyRequest request)
        {
            try
            {
                var ret = new List<BlockTransactionModel>();

                foreach (var address in request.Addresses)
                {
                    ret.AddRange(await GetTransactionsInternal(coin, network, address));
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

    }
}
