using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Ocean;
using Saiive.SuperNode.Model;
using System.Collections.Generic;
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
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/loans/collaterals/{id}", async () =>
            {
                var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/loans/collaterals/{id}");

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var json = JsonConvert.DeserializeObject<OceanDataEntity<LoanCollateral>>(data);
                return json.Data;
            });

        }

        public async Task<IList<LoanCollateral>> GetLoanCollaterals(string network)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/loans/collaterals", async () =>
            {
                var oceanData = await Helper.LoadAllFromPagedRequest<LoanCollateral>($"{OceanUrl}/{ApiVersion}/{network}/loans/collaterals");
                return oceanData;
            });
        }

        public async Task<LoanScheme> GetLoanScheme(string network, string id)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/loans/schemes/{id}", async () =>
            {

                var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/loans/schemes/{id}");

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var json = JsonConvert.DeserializeObject<OceanDataEntity<LoanScheme>>(data);
                return json.Data;
            });
        }

        public async Task<IList<LoanScheme>> GetLoanSchemes(string network)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/loans/schemes", async () =>
            {

                var oceanData = await Helper.LoadAllFromPagedRequest<LoanScheme>($"{OceanUrl}/{ApiVersion}/{network}/loans/schemes");
                return oceanData;
            });
        }

        public async Task<LoanToken> GetLoanToken(string network, string id)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/loans/tokens/{id}", async () =>
            {
                var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/loans/tokens/{id}");

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var json = JsonConvert.DeserializeObject<OceanDataEntity<LoanToken>>(data);
                return json.Data;
            });
        }

        public async Task<IList<LoanToken>> GetLoanTokens(string network)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/loans/tokens", async () =>
            {

                var oceanData = await Helper.LoadAllFromPagedRequest<LoanToken>($"{OceanUrl}/{ApiVersion}/{network}/loans/tokens");
                return oceanData;
            }, (a) =>
            {
                return new List<LoanToken>();
            });
        }

        public async Task<LoanVault> GetLoanVault(string network, string id)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/loans/vaults/{id}", async () =>
            {

                var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/loans/vaults/{id}");

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var json = JsonConvert.DeserializeObject<OceanDataEntity<LoanVault>>(data);
                return json.Data;
            });
        }

        public async Task<IList<LoanVault>> GetLoanVaults(string network)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/loans/vaults", async () =>
            {

                var oceanData = await Helper.LoadAllFromPagedRequest<LoanVault>($"{OceanUrl}/{ApiVersion}/{network}/loans/vaults");
                return oceanData;
            });
        }

        public async Task<IList<LoanAuction>> GetAuctions(string network)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/loans/auctions", async () =>
            {
                var oceanData = await Helper.LoadAllFromPagedRequest<LoanAuction>($"{OceanUrl}/{ApiVersion}/{network}/loans/auctions");
                return oceanData;
            });
        }

    }
}
