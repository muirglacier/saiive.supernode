using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Saiive.SuperNode.Abstaction
{
    public abstract class BaseProvider
    {
        public IConfiguration Config { get; }
       

        protected BaseProvider(ILogger logger, IConfiguration config)
        {
            Config = config;
           
        }
    }
}
