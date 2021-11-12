using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Application
{
    public interface ILoanVaultCache
    {
        Task<IList<LoanVault>> GetLoanVaultsForAddress(string network, string address);
        Task<IList<LoanVault>> GetLoanVaultsForAddresses(string network, IList<string> addresses);
    }
}
