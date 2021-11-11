using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Providers;

namespace Saiive.SuperNode.DeFiChain
{
    internal class DeFiChainProvider : IChainProvider
    {
        public string CoinType => "DFI";

        public DeFiChainProvider(AccountHistoryProvider accountHistoryProvider, 
            AddressProvider addressProvider, 
            AddressTransactionDetailProvider addressTransactionDetailProvider, 
            BlockProvider blockProvider, 
            TransactionProvider transactionProvider, 
            PoolPairProvider poolPairProvider,
            TokenProvider tokenProvider,
            MasterNodeProvider masterNodeProvider,
            StatsProvider statsProvider,
            PriceProvider priceProvider)
        {
            AccountHistoryProvider = accountHistoryProvider;
            AddressProvider = addressProvider;
            AddressTransactionDetailProvider = addressTransactionDetailProvider;
            BlockProvider = blockProvider;
            TransactionProvider = transactionProvider;
            PoolPairProvider = poolPairProvider;
            TokenProvider = tokenProvider;
            MasterNodeProvider = masterNodeProvider;
            StatsProvider = statsProvider;
            PriceProvider = priceProvider;
        }

        public IAccountHistoryProvider AccountHistoryProvider { get; }

        public IAddressProvider AddressProvider { get; }

        public IAddressTransactionDetailProvider AddressTransactionDetailProvider { get; }

        public IBlockProvider BlockProvider { get; }

        public ITransactionProvider TransactionProvider { get; }

        public IPoolPairProvider PoolPairProvider { get; }
        public ITokenProvider TokenProvider { get; }
        public MasterNodeProvider MasterNodeProvider { get; }
        public IMasterNodeProvider MasterNodeProivder { get; }

        public IStatsProvider StatsProvider { get; }

        public IPriceProvider PriceProvider { get; }
    }
}
