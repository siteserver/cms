using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SSCMS;

namespace SSCMS
{
    public partial interface IPathManager
    {
        string GetPackagesPath(params string[] paths);

        string PluginsPath { get; }

        string GetPluginPath(string pluginId, params string[] paths);

        string GetPluginNuspecPath(string pluginId);

        string GetPluginDllDirectoryPath(string pluginId);
    }
}
