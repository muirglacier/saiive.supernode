using Esplora.Client.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestEase;
using Saiive.BlockCypher.Core;
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

        public Blockcypher GetInstance(string network)
        {
            var endpoint = Endpoint.BtcMain;

            if (network.ToLower() == "testnet")
            {
                endpoint = Endpoint.BtcTest3;
            }
            var b = new Blockcypher(Config["BLOCKCYHPER_API_KEY"], endpoint);
            b.EnsureSuccessStatusCode = true;
            return b;
        }

        protected IEsploraClient GetEsplora(string network)
        {
            var client = RestClient.For<IEsploraClient>("https://blockstream.info/api/" + (network.ToLowerInvariant() == "testnet" ? "test/" : ""));

            return client;
        }
    }
}
