using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;
using System.Collections.Generic;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class PoolPairController : BaseController
    {
        public PoolPairController(ILogger<PoolPairController> logger, IConfiguration config) : base(logger, config)
        {
          
        }

        [HttpGet("{network}/{coin}/listpoolpairs")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(PoolPairModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> ListPoolPairs(string coin, string network)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/lp/listpoolpairs");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<Dictionary<string, PoolPairModel>>(data);
                
                foreach (KeyValuePair<string, PoolPairModel> entry in obj)
                {
                    entry.Value.ID = entry.Key;
                }

                return Ok(obj);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"could not be found"));
                }
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
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/lp/getpoolpair/{poolID}");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<Dictionary<string, PoolPairModel>>(data);
                
                foreach (KeyValuePair<string, PoolPairModel> entry in obj)
                {
                    entry.Value.ID = entry.Key;

                }

                if (obj.Count == 0) {
                    return NotFound(new ErrorModel($"Pool with ID {poolID} could not be found"));
                }

                return Ok(obj);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
