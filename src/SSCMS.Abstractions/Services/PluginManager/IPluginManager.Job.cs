using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Abstractions
{
    public partial interface IPluginManager
    {
        Dictionary<string, Func<IJobContext, Task>> GetJobs();
    }
}
