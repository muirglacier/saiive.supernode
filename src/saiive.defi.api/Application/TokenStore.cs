using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using saiive.defi.api.Model;

namespace saiive.defi.api.Application
{
    internal class TokenStore : ITokenStore
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client = new HttpClient();
        private readonly string _apiUrl;

        private readonly Dictionary<string, Dictionary<string, TokenModel>> _tokenStore =
            new Dictionary<string, Dictionary<string, TokenModel>>();

        public TokenStore(IConfiguration config)
        {
            _config = config;
            _apiUrl = config["BITCORE_URL"];
        }

        public async Task<TokenModel> GetToken(string coin, string network, string tokenName)
        {
            if (!_tokenStore.ContainsKey(network))
            {
                _tokenStore.Add(network, new Dictionary<string, TokenModel>());
            }

            if (!_tokenStore[network].ContainsKey(tokenName))
            {
                var token = await GetTokenInternal(coin, network, tokenName);
                _tokenStore[network].Add(tokenName, token);
            }

            return _tokenStore[network][tokenName];
        }

        public async Task<TokenModel> GetTokenInternal(string coin, string network, string token)
        {
            var response = await _client.GetAsync($"{_apiUrl}/api/{coin}/{network}/token/get/{token}");

            var data = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<TokenModel>(data);
        }
    }
}
