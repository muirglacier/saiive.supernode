using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("v1/api/")]
    public class TokenController : BaseController
    {
        public TokenController(ILogger<TokenController> logger, IConfiguration config) : base(logger, config)
        {
          
        }


        [HttpGet("{coin}/tokens")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IDictionary<int, TokenModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> ListTokens(string coin)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/token/list");

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

        [HttpGet("{coin}/tokens/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTokens(string coin, string token)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/token/get/{token}");

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

    }
}
