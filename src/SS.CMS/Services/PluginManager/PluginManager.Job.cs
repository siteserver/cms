using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Abstractions;

namespace SS.CMS.Services
{
    public partial class PluginManager
    {
        public async Task<Dictionary<string, Func<IJobContext, Task>>> GetJobsAsync()
        {
            var jobs = new Dictionary<string, Func<IJobContext, Task>>(StringComparer.CurrentCultureIgnoreCase);

            foreach (var service in await GetServicesAsync())
            {
                if (service.Jobs != null && service.Jobs.Count > 0)
                {
                    foreach (var command in service.Jobs.Keys)
                    {
                        jobs[command] = service.Jobs[command];
                    }
                }
            }

            return jobs;
        }
    }
}
