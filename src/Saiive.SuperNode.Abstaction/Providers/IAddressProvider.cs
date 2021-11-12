using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface IAddressProvider
    {
        Task<IList<AccountModel>> GetTotalBalance(string network, string address);
        Task<IDictionary<string, AccountModel>> GetTotalBalance(string network, AddressesBodyRequest addresses);

        Task<AggregatedAddress> GetAggregatedAddress(string network, string address);
        Task<IList<AggregatedAddress>> GetAggregatedAddresses(string network, AddressesBodyRequest addresses);


        Task<BalanceModel> GetBalance(string network, string address);
        Task<IList<BalanceModel>> GetBalance(string network, AddressesBodyRequest addresses);

        Task<IList<AccountModel>> GetAccount(string network, string address);
        Task<IList<Account>> GetAccount(string network, AddressesBodyRequest addresses);

        Task<IList<TransactionModel>> GetTransactions(string network, string address);
        Task<IList<TransactionModel>> GetTransactions(string network, AddressesBodyRequest addresses);

        Task<IList<TransactionModel>> GetUnspentTransactionOutput(string network, string address);
        Task<IList<TransactionModel>> GetUnspentTransactionOutput(string network, AddressesBodyRequest addresses);

        Task<IList<LoanVault>> GetLoanVaultsForAddress(string network, string address);
        Task<IList<LoanVault>> GetLoanVaultsForAddresses(string network, IList<string> addresses);
    }
}
