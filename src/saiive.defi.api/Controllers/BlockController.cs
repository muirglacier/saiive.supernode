using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("v1/api/")]
    public class BlockController : BaseController
    {
        public BlockController(ILogger<BlockController> logger, IConfiguration config) : base(logger, config)
        {
          
        }


        [HttpGet("{coin}/block/{height}")]
        public async Task<IActionResult> GetCurrentBlock(string coin, int height)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/block/{height}");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<BlockModel>(data);
                return Ok(obj);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound($"block with height {height} could not be found");
                }
                Logger.LogError($"{e}");
                return BadRequest(e);
            }
        }

        [HttpGet("{coin}/block/tip")]
        public async Task<IActionResult> GetCurrentHeight(string coin)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{Network}/block/tip");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<BlockModel>(data);
                return Ok(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(e);
            }
        }

    }
}
