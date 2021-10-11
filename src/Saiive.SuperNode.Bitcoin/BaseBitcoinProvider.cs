using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction;
using System;
using System.Net.Http;

namespace Saiive.SuperNode.Bitcoin
{
    internal class BaseBitcoinProvider : BaseProvider
    {

        protected const string ApiUrl = "https://api.bitcore.io"; 

        protected readonly HttpClient _client;
        public BaseBitcoinProvider(ILogger logger, IConfiguration config) : base(logger, config)
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromMinutes(5);
        }
    }
}
