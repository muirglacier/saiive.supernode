using Microsoft.Extensions.DependencyInjection;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Bitcoin.Providers;

namespace Saiive.SuperNode.Bitcoin
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBitcoin(this IServiceCollection services)
        {
            services.AddSingleton<AddressProvider>();
            services.AddSingleton<TransactionProvider>();
            services.AddSingleton<BlockProvider>();

            //services.AddSingleton<AccountHistoryProvider>();
            //services.AddSingleton<AddressTransactionDetailProvider>();



            services.AddSingleton<IChainProvider, BitcoinProvider>();

            return services;
        }
    }
}
