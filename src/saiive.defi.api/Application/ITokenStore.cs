using System.Threading.Tasks;
using saiive.defi.api.Model;

namespace saiive.defi.api.Application
{
    public interface ITokenStore
    {
        Task<TokenModel> GetToken(string coin, string network, string tokenName);
    }
}
