using System;
using System.Collections.Generic;

namespace Saiive.SuperNode.Abstaction
{
    public class ChainProviderCollection : Dictionary<string, IChainProvider>
    {
        public ChainProviderCollection()
        {
        }


        public IChainProvider GetInstance(string coin)
        {
            if (!ContainsKey(coin))
            {
                throw new ArgumentException("Node is not configured for " + coin);
            }
            return this[coin];
            
        }
    }
}
