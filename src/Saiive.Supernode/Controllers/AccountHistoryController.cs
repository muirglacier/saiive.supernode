﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;

namespace Saiive.SuperNode.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class AccountHistoryController : BaseController
    {
        public AccountHistoryController(ILogger<AccountHistoryController> logger, IConfiguration config) : base(logger, config)
        {

        }

        private async Task<List<AccountHistory>> GetAccountHistoryInternal(string coin, string network, string address, string token, string? limit, string? maxBlockHeight, bool? noRewards)
        {
            string query = $"{ApiUrl}/api/{coin}/{network}/lp/listaccounthistory/{address}/{System.Web.HttpUtility.UrlEncode(token)}";

            var dict = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(maxBlockHeight))
            {
                dict.Add("maxBlockHeight", maxBlockHeight);
            }

            if (!String.IsNullOrEmpty(limit))
            {
                dict.Add("limit", limit);
            }

            dict.Add("no_rewards", noRewards.ToString());

            var response = await _client.GetAsync(Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(query, dict));

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<List<AccountHistory>>(data);

            return obj;
        }

        [HttpPost("{network}/{coin}/accounthistory/{address}/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AccountHistory>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetAccountHistory(string coin, string network, string address, string token, string? limit, string? maxBlockHeight, bool? no_rewards)
        {
            try
            {
                var history = await GetAccountHistoryInternal(coin, network, address, token, limit, maxBlockHeight, no_rewards);

                return Ok(history);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }

        }

        [HttpPost("{network}/{coin}/history-all/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AccountHistory>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTotalBalance(string coin, string network, string token, string? limit, string? maxBlockHeight, bool? no_rewards, AddressesBodyRequest addresses)
        {
            try
            {
                var retHistory = new List<AccountHistory>();

                foreach (var address in addresses.Addresses)
                {
                    var histories = await GetAccountHistoryInternal(coin, network, address, token, limit, maxBlockHeight, no_rewards);

                    retHistory.AddRange(histories);
                }

                return Ok(retHistory);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
