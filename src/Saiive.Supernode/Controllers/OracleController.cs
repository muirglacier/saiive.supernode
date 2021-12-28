using System;
using System.Collections.Generic;
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
    public class OracleController : BaseController
    {
        public OracleController(ILogger<OracleController> logger, IConfiguration config) : base(logger, config)
        {

        }

        [HttpGet("{network}/{coin}/oracle/prices")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OracleTokenPrice>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> OraclePrices(string coin, string network)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/oracles/prices");

            var data = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();

                var ret = JsonConvert.DeserializeObject<List<OracleTokenPrice>>(data);
                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e} ({data})");
                return BadRequest(new ErrorModel($"{e.Message} ({data})"));
            }
        }

        [HttpGet("{network}/{coin}/oracle/oracles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> Oracles(string coin, string network)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/oracles/oracles");

            var data = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();

                var ret = JsonConvert.DeserializeObject<List<string>>(data);
                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e} ({data})");
                return BadRequest(new ErrorModel($"{e.Message} ({data})"));
            }
        }



        [HttpGet("{network}/{coin}/oracle/data/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OracleData))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> OracleData(string coin, string network, string id)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/oracles/data/{id}");

            var data = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();

                var ret = JsonConvert.DeserializeObject<OracleData>(data);
                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e} ({data})");
                return BadRequest(new ErrorModel($"{e.Message} ({data})"));
            }
        }
    }
}
