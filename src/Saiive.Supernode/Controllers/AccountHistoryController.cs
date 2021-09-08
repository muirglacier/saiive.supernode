using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;

namespace Saiive.SuperNode.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class AccountHistoryController : BaseController
    {
        public AccountHistoryController(ILogger<AccountHistoryController> logger, ChainProviderCollection chainProviderCollection) : base(logger, chainProviderCollection)
        {

        }

        [HttpPost("{network}/{coin}/accounthistory/{address}/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AccountHistory>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetAccountHistory(string coin, string network, string address, string token, string? limit, string? maxBlockHeight, bool no_rewards)
        {
            try
            {
                var history = await ChainProviderCollection.GetInstance(coin).AccountHistoryProvider.GetAccountHistory(network, address, token, limit, maxBlockHeight, no_rewards); 

                return Ok(history);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }

        }

        [HttpPost("{network}/{coin}/history-all/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AccountHistory>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTotalBalance(string coin, string network, string token, string? limit, string? maxBlockHeight, bool no_rewards, AddressesBodyRequest addresses)
        {
            try
            {
                var retHistory = await ChainProviderCollection.GetInstance(coin).AccountHistoryProvider.GetTotalBalance(network, token, limit, maxBlockHeight, no_rewards, addresses);

                return Ok(retHistory);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
