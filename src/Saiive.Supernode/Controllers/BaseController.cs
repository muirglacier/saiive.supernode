using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction;

namespace Saiive.SuperNode.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        
        protected readonly ILogger Logger;

        protected BaseController(ILogger logger, ChainProviderCollection chainProviderCollection)
        {
            Logger = logger;
            ChainProviderCollection = chainProviderCollection;
        }

        public ChainProviderCollection ChainProviderCollection { get; }
    }
}
