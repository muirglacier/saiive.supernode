using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class LoanController : BaseController
    {
        public TokenStore TokenStore { get; }
        public PriceStore PriceStore { get; }
        public LoanSchemeStore LoanSchemeStore { get; }

        public LoanController(ILogger<LoanController> logger, IConfiguration config, TokenStore tokenStore, PriceStore priceStore, LoanSchemeStore loanSchemeStore) : base(logger, config)
        {
            TokenStore = tokenStore;
            PriceStore = priceStore;
            LoanSchemeStore = loanSchemeStore;
        }

        private async Task<IList<LoanVault>> GetVaultInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{String.Format(ApiUrl, network)}/api/{coin}/{network}/address/{address}/vault");


            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var list = JsonConvert.DeserializeObject<IList<BitcoreVaultAddress>>(data);
            var retList = new List<LoanVault>();

            foreach (var vault in list)
            {
                var loanVault = await GetLoanVault(coin, network, vault.VaultId);
                retList.Add(loanVault);
            }

            return retList;
        }

        [HttpGet("{network}/{coin}/address/loans/vaults/{address}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetVaults(string coin, string network, string address)
        {
          
            try
            {
                var ret = await GetVaultInternal(coin, network, address);
                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost("{network}/{coin}/loanss/vaults")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetVaultsMulti(string coin, string network, AddressesBodyRequest addresses)
        {


            try
            {
                var retList = new List<LoanVault>();
                foreach (var address in addresses.Addresses)
                {
                    var vaults = await GetVaultInternal(coin, network, address);
                    retList.AddRange(vaults);
                }
                return Ok(retList);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }



        [HttpGet("{network}/{coin}/loans/vaults")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetVaults(string coin, string network)
        {
            var response = await _client.GetAsync($"{String.Format(ApiUrl, network)}/api/{coin}/{network}/loans/vaults");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var list = JsonConvert.DeserializeObject<IList<BitcoreVault>>(data);
                var retList = new List<LoanVault>();

                foreach (var vault in list)
                {
                    var collats = await LoadAmountsInfo(network, vault.CollateralAmounts);
                    var loanAmounts = await LoadAmountsInfo(network, vault.LoanAmounts);
                    var interestAmounts = await LoadAmountsInfo(network, vault.InterestAmounts);
                    var loanScheme = await LoanSchemeStore.GetScheme(network, vault.LoanSchemeId);
                    var loanVault = new LoanVault
                    {
                        VaultId = vault.VaultId,
                        LoanScheme = loanScheme,
                        OwnerAddress = vault.OwnerAddress,
                        State = vault.State,
                        InformativeRatio = vault.InformativeRatio.ToString(),
                        CollateralRatio = vault.CollateralRatio.ToString(),
                        CollateralValue = vault.CollateralValue.ToString(),
                        LoanValue = vault.LoanValue.ToString(),
                        InterestValue = vault.InterestValue.ToString(),
                        CollateralAmounts = collats,
                        LoanAmounts = loanAmounts,
                        InterestAmounts = interestAmounts
                    };
                }

                return Ok(retList);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        private async Task<LoanVault> GetLoanVault(string coin, string network, string id)
        {
            var response = await _client.GetAsync($"{String.Format(ApiUrl, network)}/api/{coin}/{network}/loans/vaults/{id}");


            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var vault = JsonConvert.DeserializeObject<BitcoreVault>(data);

            var collats = await LoadAmountsInfo(network, vault.CollateralAmounts);
            var loanAmounts = await LoadAmountsInfo(network, vault.LoanAmounts);
            var interestAmounts = await LoadAmountsInfo(network, vault.InterestAmounts);
            var loanScheme = await LoanSchemeStore.GetScheme(network, vault.LoanSchemeId);
            var loanVault = new LoanVault
            {
                VaultId = vault.VaultId,
                LoanScheme = loanScheme,
                OwnerAddress = vault.OwnerAddress,
                State = vault.State,
                InformativeRatio = vault.InformativeRatio.ToString(),
                CollateralRatio = vault.CollateralRatio.ToString(),
                CollateralValue = vault.CollateralValue.ToString(),
                LoanValue = vault.LoanValue.ToString(),
                InterestValue = vault.InterestValue.ToString(),
                CollateralAmounts = collats,
                LoanAmounts = loanAmounts,
                InterestAmounts = interestAmounts

            };
            return loanVault;
        }


        private async Task<LoanCollateral> GetLoanCollateral(string network, BitcoreCollateral collateral)
        {
            var token = await TokenStore.GetToken(network, collateral.Token);
            var price = await PriceStore.GetPrice(network, $"{token.Symbol}-USD");

            return new LoanCollateral
            {
                ActivePrice = price,
                Token
                = new Token
                {
                    CollateralAddress = token.CollateralAddress,
                    Creation = new Creation
                    {
                        Height = token.CreationHeight,
                        Tx = token.CreationTx
                    },
                    Decimal = token.Decimal,
                    Destruction = new Destruction
                    {
                        Height = token.DestructionHeight,
                        Tx = token.DestructionTx
                    },
                    DisplaySymbol = token.SymbolKey,
                    Finalized = token.Finalized,
                    SymbolKey = token.SymbolKey,
                    Id = token.Id.ToString(),
                    IsDAT = token.IsDAT,
                    IsLPS = token.IsLPS,
                    Limit = "0",
                    Mintable = token.Mintable,
                    Minted = token.Minted.ToString(),
                    Name = token.Name,
                    Symbol = token.Symbol,
                    Tradeable = token.Tradeable
                },
                TokenId = collateral.TokenId,
                ActivateAfterBlock = collateral.ActivateAfterBlock,
                PriceFeedId = collateral.FixedIntervalPriceId
            };
        }

        private async Task<LoanToken> GetLoanToken(string network, BitcoreLoanToken loanToken, TokenModel token)
        {
            var price = await PriceStore.GetPrice(network, $"{token.Symbol}-USD");

            return new LoanToken
            {
                ActivePrice = price,
                Token = new Token
                {
                    CollateralAddress = token.CollateralAddress,
                    Creation = new Creation
                    {
                        Height = token.CreationHeight,
                        Tx = token.CreationTx
                    },
                    Decimal = token.Decimal,
                    Destruction = new Destruction
                    {
                        Height = token.DestructionHeight,
                        Tx = token.DestructionTx
                    },
                    DisplaySymbol = token.SymbolKey,
                    Finalized = token.Finalized,
                    SymbolKey = token.SymbolKey,
                    Id = token.Id.ToString(),
                    IsDAT = token.IsDAT,
                    IsLPS = token.IsLPS,
                    Limit = "0",
                    Mintable = token.Mintable,
                    Minted = token.Minted.ToString(),
                    Name = token.Name,
                    Symbol = token.Symbol,
                    Tradeable = token.Tradeable
                },
                TokenId = token.CreationTx,
                FixedIntervalPriceId = loanToken.FixedIntervalPriceId,
                Interest = loanToken.Interest.ToString()
            };
        }


        [HttpGet("{network}/{coin}/loans/vaults/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetVault(string coin, string network, string id)
        {
           
            try
            {
                var loanVault = await GetLoanVault(coin, network, id);
                return Ok(loanVault);
            }
            catch (Exception e)
            {
               
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        private async Task<List<LoanVaultAmount>> LoadAmountsInfo(string network, IList<string> amounts)
        {
            var ret = new List<LoanVaultAmount>();
            foreach(var amount in amounts)
            {
                var a =  await LoadAmountInfo(network, amount);
                ret.Add(a);
            }
            return ret;
        }

        private async Task<LoanVaultAmount> LoadAmountInfo(string network, string amount)
        {
            var split = amount.Split("@");
            var tokenSymbol = split[1];

            var token = await TokenStore.GetToken(network, tokenSymbol);
            var price = await PriceStore.GetPrice(network, $"{tokenSymbol}-USD");

            return new LoanVaultAmount
            {
                Id = token.Id.ToString(),
                ActivePrice = price,
                Amount = split[0],
                DisplaySymbol = token.Symbol,
                Symbol = token.Symbol,
                SymbolKey = token.SymbolKey,
                Name = token.Name
            };
        }


        [HttpGet("{network}/{coin}/loans/schemes")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetSchemes(string coin, string network)
        {
            var response = await _client.GetAsync($"{String.Format(ApiUrl, network)}/api/{coin}/{network}/loans/schemes");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                return Ok(data);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/loans/collaterals")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetCollaterals(string coin, string network)
        {
            var response = await _client.GetAsync($"{String.Format(ApiUrl, network)}/api/{coin}/{network}/loans/collaterals");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var ret = new List<LoanCollateral>();

                var bitcoreCollat = JsonConvert.DeserializeObject<List<BitcoreCollateral>>(data);

                foreach(var bit in bitcoreCollat)
                {
                    var collat = await GetLoanCollateral(network, bit);
                    ret.Add(collat);
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/loans/tokens")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTokens(string coin, string network)
        {
            var response = await _client.GetAsync($"{String.Format(ApiUrl, network)}/api/{coin}/{network}/loans/tokens");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var model = JsonConvert.DeserializeObject<List<BitcoreLoanToken>>(data);

                var ret = new List<LoanToken>();

                foreach(var token in model)
                {
                    var tokenModel = token.Token.First().Value;

                    ret.Add(await GetLoanToken(network, token, tokenModel));
                }


                return Ok(ret);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }






        [HttpGet("{network}/{coin}/loans/schemes/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetScheme(string coin, string network, string id)
        {
            var response = await _client.GetAsync($"{String.Format(ApiUrl, network)}/api/{coin}/{network}/loans/schemes/{id}");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                return Ok(data);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/loans/collaterals/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetCollateral(string coin, string network, string id)
        {
            var response = await _client.GetAsync($"{String.Format(ApiUrl, network)}/api/{coin}/{network}/loans/collaterals/{id}");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var bitcoreCollat = JsonConvert.DeserializeObject<BitcoreCollateral>(data);


                var collat = await GetLoanCollateral(network, bitcoreCollat);


                return Ok(collat);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/loans/tokens/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetToken(string coin, string network, string id)
        {
            var response = await _client.GetAsync($"{String.Format(ApiUrl, network)}/api/{coin}/{network}/loans/tokens/{id}");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var model = JsonConvert.DeserializeObject<BitcoreLoanToken>(data);

                var ret = model.Token.First().Value;

                var loanToken = await GetLoanToken(network, model, ret);

                return Ok(loanToken);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
