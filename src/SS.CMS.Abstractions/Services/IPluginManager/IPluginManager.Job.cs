using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Abstractions;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IPluginManager
    {
        Dictionary<string, Func<IJobContext, Task>> GetJobs();
    }
}
