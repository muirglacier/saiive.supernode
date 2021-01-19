using Microsoft.AspNetCore.Mvc;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("v1/api/health")]
    public class HealthCheckController : ControllerBase
    {
        
        [HttpGet]
        public bool HealthCheck()
        {
            return true;
        }
        
    }
}
