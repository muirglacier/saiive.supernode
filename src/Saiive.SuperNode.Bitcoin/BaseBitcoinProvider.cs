using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.BlockCypher.Core;
using Saiive.SuperNode.Abstaction;

namespace Saiive.SuperNode.Bitcoin
{
    internal class BaseBitcoinProvider : BaseProvider
    {
        

        public BaseBitcoinProvider(ILogger logger, IConfiguration config) : base(logger, config) 
        {
        }

        public Blockcypher GetInstance(string network)
        {
            var endpoint = Endpoint.BtcMain;

            if(network.ToLower() == "testnet")
            {
                endpoint = Endpoint.BtcTest3;
            }
            var b = new Blockcypher(Config["BLOCKCYHPER_API_KEY"], endpoint);
            b.EnsureSuccessStatusCode = true;
            return b;
        }
    }
}
