using System.Collections.Generic;
using System.Threading.Tasks;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.Cache
{
    public interface IMasterNodeCache
    {
        Task<List<Masternode>> GetMasterNodes(string network, string coin);
    }
}
