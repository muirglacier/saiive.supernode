using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Bitcoin.Providers;
using System;

namespace Saiive.SuperNode.Bitcoin
{
    internal class BitcoinProvider : IChainProvider
    {
        public string CoinType => "BTC";

        public BitcoinProvider(AddressProvider addressProvider, TransactionProvider transactionProvider, BlockProvider blockProvider)
        {
            AddressProvider = addressProvider;
            TransactionProvider = transactionProvider;
            BlockProvider = blockProvider;
        }

        public IAccountHistoryProvider AccountHistoryProvider { get; }

        public IAddressProvider AddressProvider { get; }

        public IAddressTransactionDetailProvider AddressTransactionDetailProvider { get; }

        public IBlockProvider BlockProvider { get; }

        public ITransactionProvider TransactionProvider { get; }

        public IPoolPairProvider PoolPairProvider => throw new NotImplementedException();

        public ITokenProvider TokenProvider => throw new NotImplementedException();

        public IMasterNodeProvider MasterNodeProvider => throw new NotImplementedException();

        public IStatsProvider StatsProvider => throw new NotImplementedException();

        public IPriceProvider PriceProvider => throw new NotImplementedException();

        public ILoanProvider LoanProvider => throw new NotImplementedException();
    }
}
