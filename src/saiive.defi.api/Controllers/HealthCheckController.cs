using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("v1/api/health")]
    public class HealthCheckController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public HealthCheckController(IConfiguration config, ILogger<HealthCheckController> logger)
        {
            _config = config;
            _logger = logger;
        }
        [HttpGet]
        public bool HealthCheck()
        {
            return true;
        }

        [HttpGet("test")]
        public bool Test()
        {
            var root = (IConfigurationRoot)_config;
            var debugView = root.GetDebugView();

            _logger.LogInformation(debugView);


            return true;
        }
        
    }
}
