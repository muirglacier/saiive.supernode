using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using saiive.defi.api.Model;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("v1/api/health")]
    public class HealthCheckController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public HealthCheckController(IConfiguration config, ILogger<HealthCheckController> logger) : base(logger, config)
        {
            _config = config;
            _logger = logger;
        }
        [HttpGet]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> HealthCheck()
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/status/enabled-chains");

            try
            {
                response.EnsureSuccessStatusCode();

                return NoContent();
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

    }
}
