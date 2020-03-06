using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IPluginManager
    {
        Task<Dictionary<string, Func<IJobContext, Task>>> GetJobsAsync();
    }
}
