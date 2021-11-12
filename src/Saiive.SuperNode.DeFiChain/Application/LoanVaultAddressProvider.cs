using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Application
{
    internal class LoanVaultAddressProvider : ILoanVaultAddressProvider
    {
        private readonly LoanVaultCache _loanVaultCache;

        public LoanVaultAddressProvider(LoanVaultCache loanVaultCache)
        {
            _loanVaultCache = loanVaultCache;
        }
        public Task<IList<LoanVault>> GetLoanVaultsForAddress(string network, string address)
        {
            return _loanVaultCache.GetLoanVaultsForAddress(network, address);
        }

        public Task<IList<LoanVault>> GetLoanVaultsForAddresses(string network, IList<string> addresses)
        {
            return _loanVaultCache.GetLoanVaultsForAddresses(network, addresses);
        }
    }
}
