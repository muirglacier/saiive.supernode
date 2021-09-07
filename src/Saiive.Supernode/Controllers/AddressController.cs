using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;

namespace Saiive.SuperNode.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class AddressController : BaseController
    {
        public AddressController(ILogger<AddressController> logger, ChainProviderCollection chainProviderCollection) : base(logger, chainProviderCollection)
        {
        }

        [HttpGet("{network}/{coin}/balance-all/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BalanceModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]

        public async Task<IActionResult> GetTotalBalance(string coin, string network, string address)
        {

            try
            {
                var balanceTokens = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetTotalBalance(network, address);

                return Ok(balanceTokens);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
        [HttpPost("{network}/{coin}/balance-all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dictionary<string, AccountModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTotalBalance(string coin, string network, AddressesBodyRequest addresses)
        {
            try
            {
               var retAccountList = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetTotalBalance(network, addresses);

                return Ok(retAccountList);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/balance/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BalanceModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(ErrorModel))]

        public async Task<IActionResult> GetBalance(string coin, string network, string address)
        {
            
            try
            {
                return Ok(await ChainProviderCollection.GetInstance(coin).AddressProvider.GetBalance(network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
        [HttpPost("{network}/{coin}/balances")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BalanceModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetBalances(string coin, string network, AddressesBodyRequest addresses)
        {
            try
            {
                var ret = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetBalance(network, addresses);

                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }


        [HttpGet("{network}/{coin}/account/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<AccountModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetAccount(string coin, string network, string address)
        {

            try
            {
                return Ok(await ChainProviderCollection.GetInstance(coin).AddressProvider.GetAccount(network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
        
        [HttpPost("{network}/{coin}/accounts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<Account>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetAccounts(string coin, string network, AddressesBodyRequest request)
        {
            try
            {
                var retList = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetAccount(network, request);

                return Ok(retList);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/txs/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetTransactions(string coin, string network, string address)
        {
            try
            {
                return Ok(await ChainProviderCollection.GetInstance(coin).AddressProvider.GetTransactions(network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost("{network}/{coin}/txs")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetMultiTransactions(string coin, string network, AddressesBodyRequest request)
        {
            try
            {
                var ret = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetBalance(network, request);
                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpGet("{network}/{coin}/unspent/{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetUnspentTransactionOutput(string coin, string network, string address)
        {
            try
            {
                return Ok(await ChainProviderCollection.GetInstance(coin).AddressProvider.GetUnspentTransactionOutput(network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost("{network}/{coin}/unspent")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TransactionModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetMultiUnspentTransactionOutput(string coin, string network, AddressesBodyRequest request)
        {
            try
            {
                var ret = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetUnspentTransactionOutput(network, request);
                return Ok(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
