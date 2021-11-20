using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface IOracleProvider
    {
        Task<IList<OracleData>> GetOralces(string network);

        Task<IList<OraclePriceFeedData>> GetPriceFeedInfos(string network, string oracleId, string priceFeed);
    }
}
