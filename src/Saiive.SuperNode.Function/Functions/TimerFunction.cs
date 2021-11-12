using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Function.Functions
{
    public class TimerFunction
    {
        private readonly IServiceProvider _serviceProvider;

        public TimerFunction(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [FunctionName("TimerFunction")]
        public async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            var jobs = _serviceProvider.GetServices<IPeriodicJob>();
            foreach (var job in jobs)
            {
                await job.Run();
            }
        }
    }
}
