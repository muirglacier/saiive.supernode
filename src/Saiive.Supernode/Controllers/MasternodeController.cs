using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Cache;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class MasternodeController : BaseController
    {
        private readonly IMasterNodeCache _masterNodeCache;
        private const string NULL_TX_ID = "0000000000000000000000000000000000000000000000000000000000000000";

        public MasternodeController(ILogger<MasternodeController> logger, IConfiguration config, IMasterNodeCache masterNodeCache) : base(logger, config)
        {
            _masterNodeCache = masterNodeCache;
        }

        [HttpGet("{network}/{coin}/masternodes/list")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Masternode>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> ListMasternodes(string coin, string network)
        {
            try
            {
                var mn = await _masterNodeCache.GetMasterNodes(network, coin);

                return Ok(mn);
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
                var mn = await _masterNodeCache.GetMasterNodes(network, coin);
                
                var retList = mn.Where(a => a.ResignTx == NULL_TX_ID);
                return Ok(retList);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }


    }
}
