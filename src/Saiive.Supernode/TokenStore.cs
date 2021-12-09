using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Saiive.SuperNode.DeFiChain.Application;
using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Saiive.SuperNode
{
    public class TokenStore : ITokenStore
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _apiUrl;

        private readonly Dictionary<string, Dictionary<string, TokenModel>> _tokenStore =
            new Dictionary<string, Dictionary<string, TokenModel>>();
        private readonly Dictionary<string, Dictionary<string, TokenModel>> _tokenStoreId =
            new Dictionary<string, Dictionary<string, TokenModel>>();

        private readonly Dictionary<string, List<TokenModel>> _tokenStoreRaw =
                    new Dictionary<string, List<TokenModel>>();

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);


        public TokenStore(IConfiguration config)
        {
            _apiUrl = config["BITCORE_URL"];
        }

        public async Task<TokenModel> GetToken(string network, string tokenName)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (!_tokenStore.ContainsKey(network))
                {
                    await LoadAll(network);
                }
                if (!_tokenStore[network].ContainsKey(tokenName))
                {
                    await LoadAll(network);
                }

                if (!_tokenStore[network].ContainsKey(tokenName))
                {
                    return _tokenStoreId[network][tokenName];
                }

                return _tokenStore[network][tokenName];
            }
            finally
            {
                _semaphoreSlim.Release(1);
            }
        }

        private async Task LoadAll(string network)
        {
            var response = await _client.GetAsync($"{_apiUrl}/api/DFI/{network}/token/list");
            var data = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var tokens =
            JsonConvert.DeserializeObject<Dictionary<int, TokenModel>>(data);
            foreach (var token in tokens.Values)
            {
                if (!_tokenStore.ContainsKey(network))
                {
                    _tokenStore.Add(network, new Dictionary<string, TokenModel>());
                    _tokenStoreId.Add(network, new Dictionary<string, TokenModel>());
                    _tokenStoreRaw.Add(network, new List<TokenModel>());
                }
                if (!_tokenStore[network].ContainsKey(token.SymbolKey))
                {
                    _tokenStore[network].Add(token.SymbolKey, token);
                    try
                    {
                        _tokenStoreId[network].Add(token.Id.ToString(), token);
                    }
                    catch
                    {

                    }
                    _tokenStoreRaw[network].Add(token);
                }
            }
        }

      

        public async Task<IList<TokenModel>> GetAll(string network)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (!_tokenStore.ContainsKey(network))
                {
                    await LoadAll(network);
                }

                return _tokenStoreRaw[network];
            }
            finally
            {
                _semaphoreSlim.Release(1);
            }
        }
    }
}
