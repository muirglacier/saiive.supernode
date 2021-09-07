using System;
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
    public class PoolPairController : BaseController
    {
        public PoolPairController(ILogger<PoolPairController> logger, ChainProviderCollection chainProviderCollection) : base(logger, chainProviderCollection)
        {
          
        }

        [HttpGet("{network}/{coin}/listpoolpairs")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PoolPairModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> ListPoolPairs(string coin, string network)
        {

            try
            {
                var obj = await ChainProviderCollection.GetInstance(coin).PoolPairProvider.GetPoolPairs(network);
                return Ok(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }


        [HttpGet("{network}/{coin}/getpoolpair/{poolID}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PoolPairModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetPoolPair(string coin, string network, string poolID)
        {
            try
            {
                var obj = await ChainProviderCollection.GetInstance(coin).PoolPairProvider.GetPoolPair(network, poolID);
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
