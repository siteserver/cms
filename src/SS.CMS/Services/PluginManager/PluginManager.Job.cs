using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Abstractions;

namespace SS.CMS.Services
{
    public partial class PluginManager
    {
        public Dictionary<string, Func<IJobContext, Task>> GetJobs()
        {
            var jobs = new Dictionary<string, Func<IJobContext, Task>>(StringComparer.CurrentCultureIgnoreCase);

            foreach (var plugin in GetPlugins())
            {
                if (plugin.Jobs != null && plugin.Jobs.Count > 0)
                {
                    foreach (var command in plugin.Jobs.Keys)
                    {
                        jobs[command] = plugin.Jobs[command];
                    }
                }
            }

            return jobs;
        }
    }
}
