using System;
using System.Collections.Generic;
using System.Net;
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
    public class HealthCheckController : BaseController
    {
        private readonly Dictionary<string, double> _blockchainTimeCheckMinuteInterval;
        private const double DefaultCheckMinuteInterval = 300;

        public HealthCheckController(IConfiguration config, ILogger<HealthCheckController> logger) : base(logger, config)
        {
            _blockchainTimeCheckMinuteInterval = new Dictionary<string, double>();
            _blockchainTimeCheckMinuteInterval.Add("BTC", DefaultCheckMinuteInterval);
            _blockchainTimeCheckMinuteInterval.Add("DFI", TimeSpan.FromMinutes(30).TotalMinutes);
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

                var checkInterval = DefaultCheckMinuteInterval;
                if (_blockchainTimeCheckMinuteInterval.ContainsKey(coin))
                {
                    checkInterval = _blockchainTimeCheckMinuteInterval[coin];
                }

                var timeStartCheck = DateTime.Now.AddMinutes(checkInterval * -1);
                var timeEndCheck = DateTime.Now.AddMinutes(checkInterval);

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
