using Saiive.SuperNode.Abstaction.Providers;

namespace Saiive.SuperNode.Abstaction
{
    public interface IChainProvider
    {
        public string CoinType { get; }

        public IAccountHistoryProvider AccountHistoryProvider { get; }
        public IAddressProvider AddressProvider { get; }
        public IAddressTransactionDetailProvider AddressTransactionDetailProvider { get; }
        public IBlockProvider BlockProvider { get; }
        public ITransactionProvider TransactionProvider { get; }

        public IPoolPairProvider PoolPairProvider { get; }

        public ITokenProvider TokenProvider { get; }
    }
}
