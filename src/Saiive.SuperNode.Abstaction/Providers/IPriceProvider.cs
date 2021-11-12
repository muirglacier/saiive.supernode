using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public enum PriceFeedInterval {
        FIVE_MINUTES = 5 * 60,
        TEN_MINUTES = 10 * 60,
        ONE_HOUR = 60 * 60,
        ONE_DAY = 24 * 60 * 60
    }

    public interface IPriceProvider
    {

        Task<List<StockPrice>> GetPrices(string network);

        Task<StockPrice> GetPrice(string network, string token, string currency);

        Task<List<PriceFeedInfo>> GetFeed(string network, string token, string currency);
        Task<List<PriceFeedInfo>> GetFeedWithInterval(string network, string token, string currency, PriceFeedInterval interval);

        Task<List<OracleInfo>> GetOracles(string network, string token, string currency);
    }
}
