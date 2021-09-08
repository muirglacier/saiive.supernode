
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Bitcoin.Helper;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Bitcoin.Providers
{
    internal class AddressProvider : BaseBitcoinProvider, IAddressProvider
    {
        public AddressProvider(ILogger<AddressProvider> logger, IConfiguration config) : base(logger, config)
        {
        }

        public async Task<IList<AccountModel>> GetAccount(string network, string address)
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

        public async Task<BalanceModel> GetBalance(string network, string address)
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
            var instance = GetInstance(network);

            var transactions = await instance.GetTransactions(address);
            var ret = new List<TransactionModel>();

            foreach(var tx in transactions)
            {
                ret.Add(tx.ToTransactionModel(network, address));
            }

            return ret;
        }

        public async Task<IList<TransactionModel>> GetTransactions(string network, AddressesBodyRequest addresses)
        {
            var instance = GetInstance(network);
            var ret = new List<TransactionModel>();

            foreach (var address in addresses.Addresses)
            {
                var transactions = await instance.GetTransactions(address);
                foreach (var tx in transactions)
                {
                    ret.Add(tx.ToTransactionModel(network, address));
                }
            }
            return ret;
        }

        public async Task<IList<TransactionModel>> GetUnspentTransactionOutput(string network, string address)
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
                var transactions = await GetUnspentTransactionOutput(network, address);
                ret.AddRange(transactions);
            }
            return ret;
        }
    }
}
