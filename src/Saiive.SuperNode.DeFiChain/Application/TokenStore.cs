﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Saiive.SuperNode.DeFiChain.Ocean;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.DeFiChain.Application
{
    internal class TokenStore : ITokenStore
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client = new HttpClient();

        private readonly string _apiUrl;
        private readonly string _legacyApiUrl;

        private readonly Dictionary<string, Dictionary<string, TokenModel>> _tokenStore =
            new Dictionary<string, Dictionary<string, TokenModel>>();
        private readonly Dictionary<string, Dictionary<string, TokenModel>> _tokenStoreId =
            new Dictionary<string, Dictionary<string, TokenModel>>();

        private readonly Dictionary<string, List<TokenModel>> _tokenStoreRaw =
                    new Dictionary<string, List<TokenModel>>();

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);


        public TokenStore(IConfiguration config)
        {
            _config = config;
            _apiUrl = config["OCEAN_URL"];
            _legacyApiUrl = config["LEGACY_API_URL"];
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
            try
            {
                var oceanData = await Helper.LoadAllFromPagedRequest<OceanTokens>($"{_apiUrl}/{BaseDeFiChainProvider.ApiVersion}/{network}/tokens");
                foreach (var token in oceanData)
                {
                    if (!_tokenStore.ContainsKey(network))
                    {
                        _tokenStore.Add(network, new Dictionary<string, TokenModel>());
                        _tokenStoreId.Add(network, new Dictionary<string, TokenModel>());
                        _tokenStoreRaw.Add(network, new List<TokenModel>());
                    }

                    var tokenModel = ConvertOceanModel(token);

                    if (!_tokenStore[network].ContainsKey(token.SymbolKey))
                    {
                        _tokenStore[network].Add(token.SymbolKey, tokenModel);
                        _tokenStoreId[network].Add(token.Id, tokenModel);
                        _tokenStoreRaw[network].Add(tokenModel);
                    }
                }
            }
            catch
            {
                var responseLegacy = await _client.GetAsync($"{_legacyApiUrl}/api/v1/{network}/DFI/tokens");

                responseLegacy.EnsureSuccessStatusCode();

                var data = await responseLegacy.Content.ReadAsStringAsync();
                var tokens = JsonConvert.DeserializeObject<List<TokenModel>>(data);

                foreach (var token in tokens)
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
                        _tokenStoreId[network].Add(token.Id.ToString(), token);
                        _tokenStoreRaw[network].Add(token);
                    }
                }
            }
        }

        private TokenModel ConvertOceanModel(OceanTokens token)
        {
            return new TokenModel
            {
                CreationHeight = token.Creation.Height,
                CreationTx = token.Creation.Tx,
                Decimal = token.Decimal,
                DestructionHeight = token.Destruction.Height,
                DestructionTx = token.Destruction.Tx,
                Finalized = token.Finalized,
                Id = Convert.ToInt32(token.Id, CultureInfo.InvariantCulture),
                IsDAT = token.IsDat,
                IsLPS = token.IsLps,
                Mintable = token.Mintable,
                Minted = Convert.ToDouble(token.Minted, CultureInfo.InvariantCulture),
                Name = token.Name,
                Symbol = token.Symbol,
                SymbolKey = token.SymbolKey,
                Tradeable = token.Tradeable
            };
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
