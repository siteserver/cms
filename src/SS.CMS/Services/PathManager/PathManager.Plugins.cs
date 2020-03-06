using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SS.CMS.Abstractions;

namespace SS.CMS.Services
{
    public partial class PathManager
    {
        public string GetPackagesPath(params string[] paths)
        {
            var packagesPath = GetSiteFilesPath(DirectoryUtils.SiteFiles.Packages, PathUtils.Combine(paths));
            DirectoryUtils.CreateDirectoryIfNotExists(packagesPath);
            return packagesPath;
        }

        public string PluginsPath => GetSiteFilesPath(DirectoryUtils.SiteFiles.Plugins);

        public string GetPluginPath(string pluginId, params string[] paths)
        {
            return GetSiteFilesPath(DirectoryUtils.SiteFiles.Plugins, pluginId, PathUtils.Combine(paths));
        }

        public string GetPluginNuspecPath(string pluginId)
        {
            return GetPluginPath(pluginId, pluginId + ".nuspec");
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

            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin");
            //}
            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Debug", "net4.6.1", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin", "Debug");
            //}
            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Debug", "net4.6.1", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin", "Debug");
            //}
            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Debug", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin", "Debug");
            //}
            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Release", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin", "Release");
            //}

            return string.Empty;
        }
    }
}
