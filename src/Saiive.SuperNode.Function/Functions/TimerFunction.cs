using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saiive.SuperNode.Function.Base;

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

            var tasks = new List<Task>();

            foreach(var job in jobs)
            {
                tasks.Add(job.Run());
            }

            await Task.WhenAll(tasks);
        }
    }
}
