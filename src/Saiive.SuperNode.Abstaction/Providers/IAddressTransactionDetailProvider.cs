using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface IAddressTransactionDetailProvider
    {
        Task<List<BlockTransactionModel>> GetTransactions(string network, string address);
        Task<List<BlockTransactionModel>> GetTransactions(string network, AddressesBodyRequest addresses);

    }
}
