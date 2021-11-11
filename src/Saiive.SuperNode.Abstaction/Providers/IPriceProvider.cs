using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface IPriceProvider
    {
        Task<List<StockPrice>> GetPrices(string network);
    }
}
