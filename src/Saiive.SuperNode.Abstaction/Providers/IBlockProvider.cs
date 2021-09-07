using Saiive.SuperNode.Model;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface IBlockProvider
    {
        Task<BlockModel> GetCurrentBlock(string network, int height);
        Task<BlockModel> GetCurrentHeight(string network);
    }
}
