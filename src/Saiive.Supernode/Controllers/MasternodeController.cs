using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MasternodeController : BaseController
    {
        private Dictionary<string, List<Masternode>> _cachedList;
        private DateTime? _lastRefreshTime;
        private const string NULL_TX_ID = "0000000000000000000000000000000000000000000000000000000000000000";

        public MasternodeController(ILogger<MasternodeController> logger, IConfiguration config) : base(logger, config)
        {
            _cachedList = new Dictionary<string, List<Masternode>>();
        }

        private async Task UpdateCachedList(string coin, string network)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/masternodes/list");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var masterNodeList = JsonConvert.DeserializeObject<Dictionary<string, Masternode>>(data);

                var ret = new List<Masternode>();

                foreach (var masternode in masterNodeList)
                {
                    masternode.Value.Id = masternode.Key;
                    ret.Add(masternode.Value);
                }

                _lastRefreshTime = DateTime.UtcNow;

                if (!_cachedList.ContainsKey(network))
                {
                    _cachedList.Add(network, ret);
                }
                else
                {
                    _cachedList[network] = ret;
                }


            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
            }
        }


        [HttpGet("{network}/{coin}/list")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Masternode>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> ListMasternodes(string coin, string network)
        {

            try
            {
                if (!_cachedList.ContainsKey(coin) ||
                    _cachedList[coin].Count == 0 ||
                    _lastRefreshTime == null ||
                    (DateTime.UtcNow - _lastRefreshTime.Value) > TimeSpan.FromDays(1))
                {
                    await UpdateCachedList(coin, network);
                }


                return Ok(_cachedList[network]);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/list/active")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Masternode>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> ListActiveMasternodes(string coin, string network)
        {

            try
            {
                if (!_cachedList.ContainsKey(coin) ||
                    _cachedList[coin].Count == 0 ||
                    _lastRefreshTime == null ||
                    (DateTime.UtcNow - _lastRefreshTime.Value) > TimeSpan.FromDays(1))
                {
                    await UpdateCachedList(coin, network);
                }

                var retList = _cachedList[network].Where(a => a.ResignTx == NULL_TX_ID);
                return Ok(retList);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }


    }
}
