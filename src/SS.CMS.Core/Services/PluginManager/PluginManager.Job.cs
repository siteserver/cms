using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Core.Services
{
    public partial class PluginManager
    {
        public Dictionary<string, Func<IJobContext, Task>> GetJobs()
        {
            var jobs = new Dictionary<string, Func<IJobContext, Task>>(StringComparer.CurrentCultureIgnoreCase);

            foreach (var service in Services)
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
