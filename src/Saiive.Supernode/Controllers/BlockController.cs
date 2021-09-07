using System;
using System.Net;
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
    public class BlockController : BaseController
    {
        public BlockController(ILogger<BlockController> logger, ChainProviderCollection chainProviderCollection) : base(logger, chainProviderCollection)
        {
          
        }


        [HttpGet("{network}/{coin}/block/{height}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlockModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetCurrentBlock(string coin, string network, int height)
        {
           

            try
            {
           
                var obj = await ChainProviderCollection.GetInstance(coin).BlockProvider.GetCurrentBlock(network, height);
                return Ok(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/block/tip")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlockModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetCurrentHeight(string coin, string network)
        {

            try
            {
                var obj = await ChainProviderCollection.GetInstance(coin).BlockProvider.GetCurrentHeight(network);
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
