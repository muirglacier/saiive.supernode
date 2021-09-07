using System;
using System.Collections.Generic;
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
    public class TokenController : BaseController
    {

        public TokenController(ILogger<TokenController> logger, ChainProviderCollection chainProvider) : base(logger, chainProvider)
        {
        }


        [HttpGet("{network}/{coin}/tokens")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IDictionary<int, TokenModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> ListTokens(string coin, string network)
        {
            

            try
            {
                return Ok(await ChainProviderCollection.GetInstance(coin).TokenProvider.GetAll(network));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/tokens/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTokens(string coin, string network, string token)
        {
            try
            {
                return Ok(await ChainProviderCollection.GetInstance(coin).TokenProvider.GetToken(network, token));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

    }
}
