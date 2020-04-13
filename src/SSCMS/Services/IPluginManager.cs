using System.Collections.Generic;
using System.Reflection;
using SSCMS.Plugins;

namespace SSCMS.Services
{
    public partial interface IPluginManager
    {
        string DirectoryPath { get; }
        IEnumerable<IPluginMetadata> Plugins { get; }
        IEnumerable<Assembly> Assemblies { get; }
    }
}
