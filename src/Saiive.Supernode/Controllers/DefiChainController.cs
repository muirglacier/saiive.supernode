using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class DefiChainController : BaseController
    {
        private YieldFramingModelRequest _lastValidItem = null;

        public DefiChainController(ILogger<DefiChainController> logger, IConfiguration config) : base(logger, config)
        {
          
        }


        [HttpGet("{network}/{coin}/list-yield-farming")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IDictionary<int, YieldFramingModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> ListYieldFarming(string coin, string network)
        {
            var response = await _client.GetAsync($"https://api.defichain.io/v1/listyieldfarming?network={network}");

            try
            {
                var data = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                YieldFramingModelRequest obj = JsonConvert.DeserializeObject<YieldFramingModelRequest>(data);

                _lastValidItem = obj ?? throw new ArgumentException();

                return Ok(_lastValidItem.pools);
            }
            catch (Exception e)
            {
                if (_lastValidItem != null)
                {
                    return Ok(_lastValidItem.pools);
                }

                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
