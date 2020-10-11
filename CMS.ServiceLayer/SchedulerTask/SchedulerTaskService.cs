using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNTScheduler.Core.Contracts;

namespace CMS.ServiceLayer.SchedulerTask
{
    public class SchedulerTaskService : IScheduledTask
    {
        public SchedulerTaskService()
        {

        }

        public bool IsShuttingDown { get; set; }

        public async Task RunAsync()
        {
            if (IsShuttingDown)
                return;

            await Task.Run(function: async () =>
            {
                Console.Write("Do task");
            });
        }
    }
}
