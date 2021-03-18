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
    [Route("v1/api")]
    public class HealthCheckController : BaseController
    {
        public HealthCheckController(IConfiguration config, ILogger<HealthCheckController> logger) : base(logger, config)
        {
        }

        [HttpGet("health")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult HealthCheck()
        {
            return NoContent();
        }
        
        [HttpGet("{network}/{coin}/health")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> HealthCheckNetwork(string network, string coin)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/block/tip");

            try
            {
                response.EnsureSuccessStatusCode();

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    throw new ArgumentException("Node is not synced or running...");
                }

                var data = await response.Content.ReadAsStringAsync();

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

                return BadRequest("Chain is not synced yet!");
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

    }
}
