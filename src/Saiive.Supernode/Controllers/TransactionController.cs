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
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class TransactionController : BaseController
    {
        public TransactionController(ILogger<TransactionController> logger, ChainProviderCollection chainStateProvider) : base(logger, chainStateProvider)
        {
          
        }

        [HttpGet("{network}/{coin}/tx/id/{txId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(TransactionModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTransactionById(string coin, string network, string txId)
        {
           
            try
            {
               
                var obj = await ChainProviderCollection.GetInstance(coin).TransactionProvider.GetTransactionById(network, txId);
                return Ok(obj);
            }
            catch (Exception e)
            {
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
             try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).TransactionProvider.GetTransactionsByBlock(network, block);
                return Ok(obj);
            }
            catch (Exception e)
            {
               
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
           

            try
            {
              var obj = await ChainProviderCollection.GetInstance(coin).TransactionProvider.GetTransactionsByBlockHeight(network, height, includeDetails);

                return Ok(obj);
            }
            catch (Exception e)
            {
               
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

      

        [HttpPost("{network}/{coin}/tx/raw")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TransactionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> SendRawTransaction(string coin, string network, TransactionRequest request)
        {
            try
            {
                var obj = await ChainProviderCollection.GetInstance(coin).TransactionProvider.SendRawTransaction(network, request);
                var ret = new TransactionResponse()
                {
                    TxId = obj
                };
                return Ok(ret);
            }
            catch(ArgumentException e)
            {
                var currentBlock = await ChainProviderCollection.GetInstance(coin).BlockProvider.GetCurrentHeight(network);

                Logger.LogError("{coin}+{network}: Error commiting tx to blockchain ({response} for {txHex}) @ {blockHeight} block", coin, network, e.Message, request.RawTx, currentBlock.Height);
                return BadRequest(new ErrorModel($"{e.Message}"));
            }
        }
    }
}
