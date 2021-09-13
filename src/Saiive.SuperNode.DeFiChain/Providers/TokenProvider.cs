using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Application;
using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class TokenProvider : BaseDeFiChainProvider, ITokenProvider
    {
        private readonly ITokenStore _store;

        public TokenProvider(ILogger<TokenProvider> logger, IConfiguration config, ITokenStore store) : base(logger, config)
        {
            _store = store;
        }

        public async Task<Dictionary<int, TokenModel>> GetAll(string network)
        {
            var ret = new Dictionary<int, TokenModel> ();

            var tokens = await _store.GetAll(network);


            foreach(var token in tokens)
            {
                ret.Add(token.Id, token);
            }

            return ret;
        }

        public Task<TokenModel> GetToken(string network, string tokenName)
        {
            return _store.GetToken(network, tokenName);
        }
    }
}
