using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface IAccountHistoryProvider
    {
        Task<List<AccountHistory>> GetAccountHistory(string network, string address, string token, string limit, string maxBlockHeight, bool no_rewards);
        Task<List<AccountHistory>> GetTotalBalance(string network, string token, string limit, string maxBlockHeight, bool no_rewards, AddressesBodyRequest addresses);
    }
}
