using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface IPoolPairProvider
    {
        Task<Dictionary<string, PoolPairModel>> GetPoolPairs(string network);
        Task<Dictionary<string, PoolPairModel>> GetPoolPairsBySymbolKey(string network);

        Task<Dictionary<string, PoolPairModel>> GetPoolPair(string network, string poolId);
    }
}
