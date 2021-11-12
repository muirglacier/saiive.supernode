using Saiive.SuperNode.Abstaction.Providers;

namespace Saiive.SuperNode.Abstaction
{
    public interface IChainProvider
    {
        string CoinType { get; }

        IAccountHistoryProvider AccountHistoryProvider { get; }
        IAddressProvider AddressProvider { get; }
        IAddressTransactionDetailProvider AddressTransactionDetailProvider { get; }
        IBlockProvider BlockProvider { get; }
        ITransactionProvider TransactionProvider { get; }

        IPoolPairProvider PoolPairProvider { get; }

        ITokenProvider TokenProvider { get; }

        IMasterNodeProvider MasterNodeProivder { get; }

        IStatsProvider StatsProvider { get; }

        IPriceProvider PriceProvider { get; }

        ILoanProvider LoanProvider { get; }
    }
}
