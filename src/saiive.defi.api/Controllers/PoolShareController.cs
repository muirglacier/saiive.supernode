using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;
using saiive.defi.api.Requests;
using System.Collections.Generic;

namespace saiive.defi.api.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class PoolShareController : BaseController
    {
        public PoolShareController(ILogger<PoolShareController> logger, IConfiguration config) : base(logger, config)
        {
          
        }

        private async Task<List<PoolShareModel>> GetPoolSharesInternal(string coin, string network, int start = 0, int limit = 1, bool including_start = false)
        {
            AddBaseResponseHeaders();
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/lp/listpoolshares?start={start}&limit={limit}&including_start={including_start}");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            var ret = new List<PoolShareModel>();

            Dictionary<string, Dictionary<string, dynamic>> obj = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(data);

            foreach (var item in obj)
            {
                var shareModel = new PoolShareModel();
                shareModel.Key = item.Key;
                shareModel.PoolID = item.Value["poolID"];
                shareModel.Owner = item.Value["owner"];
                shareModel.Percent = item.Value["%"];
                shareModel.Amount = item.Value["amount"];
                shareModel.TotalLiquidity = item.Value["totalLiquidity"];

                ret.Add(shareModel);
            }

            return ret;
        }

        private async Task<List< PoolShareModel>> GetAllPoolSharesInternal(string coin, string network)
        {
            int start = 0;
            int limit = 10000;

            var ret = new List<PoolShareModel>();
            var loopResult = new List<PoolShareModel>();

            do
            {
                loopResult = await this.GetPoolSharesInternal(coin, network, start, limit);

                if (loopResult.Count == 0) {
                    break;
                }

                foreach (PoolShareModel entry in loopResult)
                {
                    ret.Add(entry);
                }

                var startString = loopResult[loopResult.Count - 1].Key.Split('@')[0];
                start = int.Parse(startString);

            } while (loopResult.Count > 0);

            return ret;
        }

        [HttpPost("{network}/{coin}/listminepoolshares")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dictionary<string, PoolShareModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetMinePoolShares(string coin, string network, AddressesBodyRequest addresses)
        {
            AddBaseResponseHeaders();
            try
            {
                var ret = new Dictionary<string, PoolShareModel>();
                var poolShares = await this.GetAllPoolSharesInternal(coin, network);

                foreach (var address in addresses.Addresses)
                {
                    foreach (var poolShare in poolShares)
                    {
                        if (addresses.Addresses.Contains(poolShare.Owner))
                        {
                            if (ret.ContainsKey(poolShare.Key))
                            {
                                continue;
                            }

                            ret.Add(poolShare.Key, poolShare);
                        }
                    }
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/listpoolshares")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dictionary<string, PoolShareModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetAllPoolShares(string coin, string network)
        {
            AddBaseResponseHeaders();
            try
            {
                var ret = new Dictionary<string, PoolShareModel>();
                var poolShares = await this.GetAllPoolSharesInternal(coin, network);

                foreach (var poolShare in poolShares)
                {
                    if (ret.ContainsKey(poolShare.Key))
                    {
                        continue;
                    }

                    ret.Add(poolShare.Key, poolShare);
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/listpoolshares/{start}/{limit}/{including_start}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dictionary<string, PoolShareModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetPoolShares(string coin, string network, int start, int limit, bool including_start)
        {
            AddBaseResponseHeaders();
            try
            {
                var ret = new Dictionary<string, PoolShareModel>();
                var poolShares = await this.GetPoolSharesInternal(coin, network, start, limit, including_start);

                foreach (var poolShare in poolShares)
                {
                    ret.Add(poolShare.Key, poolShare);		
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
