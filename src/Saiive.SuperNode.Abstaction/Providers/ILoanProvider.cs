using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface ILoanProvider
    {
        Task<IList<LoanScheme>> GetLoanSchemes(string network);
        Task<LoanScheme> GetLoanScheme(string network, string id);


        Task<IList<LoanCollateral>> GetLoanCollaterals(string network);
        Task<LoanCollateral> GetLoanCollateral(string network, string id);

        Task<IList<LoanToken>> GetLoanTokens(string network);
        Task<LoanToken> GetLoanToken(string network, string id);
    }
}
