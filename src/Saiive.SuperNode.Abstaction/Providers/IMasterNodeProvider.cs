using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface IMasterNodeProvider
    {
        Task<List<Masternode>> ListMasternodes(string network);
        Task<List<Masternode>> ListActiveMasternodes(string network);
    }
}
