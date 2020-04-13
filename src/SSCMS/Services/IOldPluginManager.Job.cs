using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IOldPluginManager
    {
        Dictionary<string, Func<IJobContext, Task>> GetJobs();
    }
}
