using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace saiive.defi.api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger Logger;
        protected readonly string ApiUrl;

        protected readonly HttpClient _client = new HttpClient();

        protected BaseController(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            ApiUrl = config["BITCORE_URL"];

            Logger.LogInformation($"Using bitcore {ApiUrl}");
        }
    }
}
