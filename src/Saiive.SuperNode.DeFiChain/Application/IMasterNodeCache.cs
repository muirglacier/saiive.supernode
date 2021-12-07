using System.Collections.Generic;
using System.Threading.Tasks;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.DeFiChain.Application
{
    public interface IMasterNodeCache
    {
        Task<List<Masternode>> GetMasterNodes(string network);

        Task<bool> IsMasternodeStillAlive(string network, string ownerAddress, string txId);
    }
}
