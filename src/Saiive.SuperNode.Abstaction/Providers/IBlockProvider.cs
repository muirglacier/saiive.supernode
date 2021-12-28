using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface IBlockProvider
    {
        Task<BlockModel> GetBlockByHeightOrHash(string network, string hash);
        Task<BlockModel> GetCurrentHeight(string network);

        Task<List<BlockModel>> GetLatestBlocks(string network);
    }
}
