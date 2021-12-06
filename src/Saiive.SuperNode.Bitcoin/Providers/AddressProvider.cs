
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Saiive.SuperNode.Bitcoin.Helper;

namespace Saiive.SuperNode.Bitcoin.Providers
{
    internal class AddressProvider : BaseBitcoinProvider, IAddressProvider
    {
        public AddressProvider(ILogger<AddressProvider> logger, IConfiguration config) : base(logger, config)
        {
        }

        private async Task<TransactionDetailModel> GetTransactionDetails(string coin, string network, string txId)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/tx/{txId}/coins");

            var data = await response.Content.ReadAsStringAsync();
            var tx = JsonConvert.DeserializeObject<TransactionDetailModel>(data);
            return tx;
        }

        private async Task<List<TransactionModel>> GetTransactionsInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/address/{address}/txs?limit=1000");

            var data = await response.Content.ReadAsStringAsync();

            if(data == "Rate Limited")
            {
                return new List<TransactionModel>();
            }

            var txs = JsonConvert.DeserializeObject<List<TransactionModel>>(data);

            return txs;

        }

        private async Task<BalanceModel> GetBalanceInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/address/{address}/balance");

            var data = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw new ArgumentException(data);
            }


            var obj = JsonConvert.DeserializeObject<BalanceModel>(data);
            obj.Address = address;

            return obj;

        }

        private async Task<List<AccountModel>> GetAccountInternal(string coin, string network, string address)
        {
            var ret = new List<AccountModel>();

            var nativeBalance = await GetBalanceInternal(coin, network, address);
            if (nativeBalance.Confirmed > 0)
            {
                ret.Add(new AccountModel
                {
                    Address = address,
                    Balance = nativeBalance.Confirmed,
                    Raw = $"{nativeBalance.Confirmed}@{coin}",
                    Token = $"${coin}"
                });
            }


            return ret;

        }


        public async Task<IList<AccountModel>> GetAccount(string network, string address)
        {
            try {
                return await GetAccountInternal("BTC", network, address);
            }
            catch
            {
                return await GetAccountCypher(network, address);
            }
        }

        public async Task<IList<Account>> GetAccount(string network, AddressesBodyRequest addresses)
        {
            var ret = new List<Account>();

            foreach(var address in addresses.Addresses)
            {
                ret.Add(new Account()
                {
                    Address = address,
                    Accounts = await GetAccount(network, address)
                });
            }


            return ret;

        }

        public async Task<IList<AccountModel>> GetAccountCypher(string network, string address)
        {
            var instance = GetInstance(network);
            var balance = await instance.GetBalanceForAddress(address);

            var accountModel = new AccountModel
            {
                Address = address,
                Balance = balance.Balance.ValueLong,
                Raw = $"{balance.Balance.ValueLong}@BTC",
                Token = "BTC"
            };


            return new List<AccountModel> { accountModel };
        }

        public async Task<BalanceModel> GetBalance(string network, string address)
        {
            try
            {
                return await GetBalanceInternal("BTC", network, address);
            }
            catch
            {
                return await GetBalanceCypher(network, address);
            }
        }

        public async Task<BalanceModel> GetBalanceCypher(string network, string address)
        {
            var instance = GetInstance(network);
            var balance = await instance.GetBalanceForAddress(address);

            return new BalanceModel
            {
                Address = address,
                Balance = (ulong)balance.Balance.ValueLong,
                Confirmed = (ulong)balance.Balance.ValueLong,
                Unconfirmed = 0
            };
        }

        public async Task<IList<BalanceModel>> GetBalance(string network, AddressesBodyRequest addresses)
        {
            var ret = new List<BalanceModel>();

            foreach(var address in addresses.Addresses)
            {
                ret.Add(await GetBalance(network, address));
            }

            return ret;
            
        }

        public Task<IList<AccountModel>> GetTotalBalance(string network, string address)
        {
            return GetAccount(network, address);
        }

        public async Task<IDictionary<string, AccountModel>> GetTotalBalance(string network, AddressesBodyRequest addresses)
        {
            var accounts = await GetAccount(network, addresses);
            var ret = new Dictionary<string, AccountModel>();

            foreach(var a in accounts)
            {
                ret.Add(a.Address, a.Accounts.FirstOrDefault());
            }


            return ret;
        }

        public async Task<IList<TransactionModel>> GetTransactions(string network, string address)
        {
            try
            {
                return await GetTransactionsInternal("BTC", network, address);
            }
            catch
            {
                return await GetTransactionsCyphre(network, address);
            }
        }

        public async Task<IList<TransactionModel>> GetTransactionsCyphre(string network, string address)
        {
            var instance = GetInstance(network);

            var transactions = await instance.GetTransactions(address);
            var ret = new List<TransactionModel>();

            foreach (var tx in transactions)
            {
                ret.Add(tx.ToTransactionModel(network, address));
            }

            return ret;
        }

        public async Task<IList<TransactionModel>> GetTransactions(string network, AddressesBodyRequest addresses)
        {
            var ret = new List<TransactionModel>();

            foreach (var address in addresses.Addresses)
            {
                ret.AddRange(await GetTransactionsInternal("BTC", network, address));
            }
            return ret;
        }
        private async Task<List<TransactionModel>> GetUnspentTransactionOutputsInternal(string coin, string network, string address)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/address/{address}?unspent=true&limit=1000");

            var data = await response.Content.ReadAsStringAsync();

            var txs = JsonConvert.DeserializeObject<List<TransactionModel>>(data);
            return txs;


        }

        public async Task<IList<TransactionModel>> GetUnspentTransactionOutput(string network, string address)
        {
            try
            {
                return await GetUnspentTransactionOutputsInternal("BTC", network, address);
            }
            catch
            {

                return await GetUnspentTransactionOutputCypher(network, address);
            }

        }

        public async Task<IList<TransactionModel>> GetUnspentTransactionOutputCypher(string network, string address)
        {
            var ret = new List<TransactionModel>();
            var instance = GetInstance(network);

            var unspent = await instance.GetUnspentTransactionReference(address);

            foreach (var tx in unspent)
            {
                ret.Add(tx.ToTransactionModel(network, address));
            }

            return ret;

        }


        public async Task<IList<TransactionModel>> GetUnspentTransactionOutput(string network, AddressesBodyRequest addresses)
        {
            var ret = new List<TransactionModel>();

            foreach (var address in addresses.Addresses)
            {
                ret.AddRange(await GetUnspentTransactionOutputsInternal("BTC", network, address));
            }
            return ret;
        }

        public async Task<AggregatedAddress> GetAggregatedAddress(string network, string address)
        {
            await Task.CompletedTask;
            return new AggregatedAddress();
        }

        public async Task<IList<AggregatedAddress>> GetAggregatedAddresses(string network, AddressesBodyRequest addresses)
        {
            await Task.CompletedTask;
            return new List<AggregatedAddress>();
        }

        public Task<IList<LoanVault>> GetLoanVaultsForAddress(string network, string address)
        {
            throw new NotImplementedException();
        }

        public Task<IList<LoanVault>> GetLoanVaultsForAddresses(string network, IList<string> addresses)
        {
            throw new NotImplementedException();
        }

        public Task<IList<LoanAuctionHistory>> GetAuctionHistory(string network, string address)
        {
            throw new NotImplementedException();
        }

        public Task<IList<LoanAuctionHistory>> GetAuctionHistory(string network, AddressesBodyRequest addresses)
        {
            throw new NotImplementedException();
        }
    }
}
