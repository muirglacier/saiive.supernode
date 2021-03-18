using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;
using saiive.defi.api.Requests;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class DexController : BaseController
    {
        public DexController(ILogger<DexController> logger, IConfiguration config) : base(logger, config)
        {
          
        }

        [HttpPost("{network}/{coin}/dex/testpoolswap")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TestPoolSwapModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> Testpoolswap(string coin, string network, TestPoolSwapBodyRequest request)
        {
            AddBaseResponseHeaders();
            var httpContent =
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{ApiUrl}/api/{coin}/{network}/lp/testpoolswap", httpContent);

            var data = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();

                var result = new TestPoolSwapModel();
                result.Result = data;

                return Ok(result);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e} ({data})");
                return BadRequest(new ErrorModel($"{e.Message} ({data})"));
            }
        }
    }
}
