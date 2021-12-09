using System.Collections.Generic;
using System.Threading.Tasks;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.DeFiChain.Application
{
    public interface ITokenStore
    {
        Task<TokenModel> GetToken(string network, string tokenName);
        Task<IList<TokenModel>> GetAll(string network);
    }

    public interface IPriceStore
    {
        Task<ActivePrice> GetPrice(string network, string tokenName);
        Task<IList<ActivePrice>> GetAll(string network);
    }
}
