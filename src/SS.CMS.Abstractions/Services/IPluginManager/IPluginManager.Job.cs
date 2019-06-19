using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Services.IPluginManager
{
    public partial interface IPluginManager
    {
        Dictionary<string, Func<IJobContext, Task>> GetJobs();
    }
}
