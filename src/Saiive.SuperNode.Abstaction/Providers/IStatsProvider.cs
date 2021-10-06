using Saiive.SuperNode.Model;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface IStatsProvider
    {
        Task<StatsModel> GetStats(string network);
    }
}
