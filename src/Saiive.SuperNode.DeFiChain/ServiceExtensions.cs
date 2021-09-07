using Microsoft.Extensions.DependencyInjection;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.DeFiChain.Application;
using Saiive.SuperNode.DeFiChain.Providers;

namespace Saiive.SuperNode.DeFiChain
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDeFiChain(this IServiceCollection services)
        {
            services.AddSingleton<AccountHistoryProvider>();
            services.AddSingleton<AddressTransactionDetailProvider>();
            services.AddSingleton<AddressProvider>();
            services.AddSingleton<BlockProvider>();
            services.AddSingleton<TransactionProvider>();
            services.AddSingleton<PoolPairProvider>();
            services.AddSingleton<TokenProvider>();

            services.AddSingleton<IChainProvider, DeFiChainProvider>();

            services.AddSingleton<ITokenStore, TokenStore>();


            return services;
        }
    }
}
