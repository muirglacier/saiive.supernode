using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using saiive.defi.api.Model;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class HealthCheckController : BaseController
    {
        public HealthCheckController(IConfiguration config, ILogger<HealthCheckController> logger) : base(logger, config)
        {
        }

        [HttpGet("health")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult HealthCheck()
        {
            return Ok();
        }
        
        [HttpGet("{network}/{coin}/health")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> HealthCheckNetwork(string network, string coin)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/block/tip");

            var data = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    throw new ArgumentException("Node is not synced or running...");
                }


                var obj = JsonConvert.DeserializeObject<BlockModel>(data);

                if (obj == null)
                {
                    throw new ArgumentException("block model is empty");
                }
                var time = Convert.ToDateTime(obj.Time);
                var timeStartCheck = DateTime.Now.AddHours(-5);
                var timeEndCheck = DateTime.Now.AddHours(5);

                if (time >= timeStartCheck && time <= timeEndCheck)
                {
                    return Ok(obj);
                }

                return Problem("Chain is not synced yet!");
            }
            catch (Exception e)
            {
                Logger.LogError($"{data}\n{e}", e);
                return BadRequest(new ErrorModel(e.Message));
            }
        }

    }
}
