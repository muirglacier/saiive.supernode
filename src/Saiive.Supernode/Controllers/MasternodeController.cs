using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class MasternodeController : BaseController
    {

        public MasternodeController(ILogger<MasternodeController> logger, ChainProviderCollection config) : base(logger, config)
        {
        }

        [HttpGet("{network}/{coin}/masternodes/list")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Masternode>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> ListMasternodes(string coin, string network)
        {
            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).MasterNodeProivder.ListMasternodes(network);

                return Ok(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/masternodes/list/active")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Masternode>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> ListActiveMasternodes(string coin, string network)
        {
            try
            {
                var obj = await ChainProviderCollection.GetInstance(coin).MasterNodeProivder.ListActiveMasternodes(network);

                return Ok(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }


    }
}
