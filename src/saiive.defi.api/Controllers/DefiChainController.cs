using System;
using System.Collections.Generic;
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
    [Route("/api/v1/")]
    public class DefiChainController : BaseController
    {
        public DefiChainController(ILogger<DefiChainController> logger, IConfiguration config) : base(logger, config)
        {
          
        }


        [HttpGet("{network}/{coin}/list-yield-farming")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IDictionary<int, YieldFramingModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> ListYieldFarming(string coin, string network)
        {
            var response = await _client.GetAsync($"{DefiChainApiUrl}/v1/listyieldfarming?network={network}net");

            try
            {
                var data = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                YieldFramingModelRequest obj = JsonConvert.DeserializeObject<YieldFramingModelRequest>(data);  

                return Ok(obj.pools);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
