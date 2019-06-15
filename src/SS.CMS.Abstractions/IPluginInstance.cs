using System;
using System.Collections.Generic;
using System.Text;

namespace SS.CMS.Abstractions
{
    public interface IPluginInstance
    {
        string Id { get; }

        IPackageMetadata Metadata { get; }

        PluginBase Plugin { get; }

        IService Service { get; }

        long InitTime { get; }

        string ErrorMessage { get; }

        bool IsRunnable { get; set; }

        bool IsDisabled { get; set; }

        int Taxis { get; set; }
    }
}
