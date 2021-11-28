using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;

namespace Saiive.SuperNode.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class AddressTransactionDetailController : BaseController
    {
        public AddressTransactionDetailController(ILogger<AddressTransactionDetailController> logger, ChainProviderCollection chainProvider) : base(logger, chainProvider)
        {
        }

        [HttpGet("{network}/{coin}/tx-history/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTransactions(string coin, string network, string address)
        {
            try
            {
                return Ok(await ChainProviderCollection.GetInstance(coin).AddressTransactionDetailProvider.GetTransactions(network, address));
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
                request.Addresses = request.Addresses.Distinct().ToList();
                var ret = await ChainProviderCollection.GetInstance(coin).AddressTransactionDetailProvider.GetTransactions(network, request);

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
