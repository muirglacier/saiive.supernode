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
    public class HealthCheckController : BaseController
    {
        private readonly Dictionary<string, double> _blockchainTimeCheckMinuteInterval;
        private const double DefaultCheckMinuteInterval = 300;

        public HealthCheckController(ILogger<HealthCheckController> logger, ChainProviderCollection chainProviderCollection) : base(logger, chainProviderCollection)
        {
            _blockchainTimeCheckMinuteInterval = new Dictionary<string, double>();
            _blockchainTimeCheckMinuteInterval.Add("BTC", DefaultCheckMinuteInterval);
            _blockchainTimeCheckMinuteInterval.Add("DFI", TimeSpan.FromMinutes(60).TotalMinutes);
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
            try
            {
                var obj = await ChainProviderCollection.GetInstance(coin).BlockProvider.GetCurrentHeight(network);

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
                Logger.LogError($"{e}", e);
                return BadRequest(new ErrorModel(e.Message));
            }
        }

    }
}
