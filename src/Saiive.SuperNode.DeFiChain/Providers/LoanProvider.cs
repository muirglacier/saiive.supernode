using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Ocean;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class LoanProvider : BaseDeFiChainProvider, ILoanProvider
    {
        public LoanProvider(ILogger<ILoanProvider> logger, IConfiguration config) : base(logger, config)
        {
        }

        public async Task<LoanCollateral> GetLoanCollateral(string network, string id)
        {
            var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/loans/collaterals/{id}");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var json = JsonConvert.DeserializeObject<OceanDataEntity<LoanCollateral>>(data);
            return json.Data;

        }

        public async Task<IList<LoanCollateral>> GetLoanCollaterals(string network)
        {
            var oceanData = await Helper.LoadAllFromPagedRequest<LoanCollateral>($"{OceanUrl}/{ApiVersion}/{network}/loans/collaterals");
            return oceanData;
        }

        public async Task<LoanScheme> GetLoanScheme(string network, string id)
        {
            var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/loans/schemes/{id}");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var json = JsonConvert.DeserializeObject<OceanDataEntity<LoanScheme>>(data);
            return json.Data;
        }

        public async Task<IList<LoanScheme>> GetLoanSchemes(string network)
        {
            var oceanData = await Helper.LoadAllFromPagedRequest<LoanScheme>($"{OceanUrl}/{ApiVersion}/{network}/loans/schemes");
            return oceanData;
        }

        public async Task<LoanToken> GetLoanToken(string network, string id)
        {
            var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/loans/tokens/{id}");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var json = JsonConvert.DeserializeObject<OceanDataEntity<LoanToken>>(data);
            return json.Data;
        }

        public async Task<IList<LoanToken>> GetLoanTokens(string network)
        {
            var oceanData = await Helper.LoadAllFromPagedRequest<LoanToken>($"{OceanUrl}/{ApiVersion}/{network}/loans/tokens");
            return oceanData;
        }
    }
}
