using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Application
{
    internal interface ILoanVaultAddressProvider
    {
        Task<IList<LoanVault>> GetLoanVaultsForAddress(string network, string address);
        Task<IList<LoanVault>> GetLoanVaultsForAddresses(string network, IList<string> addresses);
    }
}
