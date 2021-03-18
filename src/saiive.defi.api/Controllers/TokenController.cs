using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using saiive.defi.api.Application;
using saiive.defi.api.Model;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class TokenController : BaseController
    {
        private readonly ITokenStore _store;

        public TokenController(ILogger<TokenController> logger, IConfiguration config, ITokenStore store) : base(logger, config)
        {
            _store = store;
        }


        [HttpGet("{network}/{coin}/tokens")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IDictionary<int, TokenModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> ListTokens(string coin, string network)
        {
            AddBaseResponseHeaders();
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/token/list");

            try
            {
                var data = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                
                return Ok(data);
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
            AddBaseResponseHeaders();
            try
            {

                return Ok(await _store.GetToken(coin, network, token));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

    }
}
