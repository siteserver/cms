using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class PathManager
    {
        public string GetPackagesPath(params string[] paths)
        {
            var packagesPath = GetContentRootPath(DirectoryUtils.Packages, PathUtils.Combine(paths));
            DirectoryUtils.CreateDirectoryIfNotExists(packagesPath);
            return packagesPath;
        }

        public string PluginsPath => _pluginManager.DirectoryPath;

        public string GetPluginPath(string pluginId, params string[] paths)
        {
            return PathUtils.Combine(PluginsPath, pluginId, PathUtils.Combine(paths));
        }

        public string GetPluginDllDirectoryPath(string pluginId)
        {
            var fileName = pluginId + ".dll";

            var filePaths = Directory.GetFiles(GetPluginPath(pluginId, "Bin"), fileName, SearchOption.AllDirectories);

            var dict = new Dictionary<DateTime, string>();
            foreach (var filePath in filePaths)
            {
                var lastModifiedDate = File.GetLastWriteTime(filePath);
                dict[lastModifiedDate] = filePath;
            }

            if (dict.Count > 0)
            {
                var filePath = dict.OrderByDescending(x => x.Key).First().Value;
                return Path.GetDirectoryName(filePath);
            }

            return string.Empty;
        }
    }
}
