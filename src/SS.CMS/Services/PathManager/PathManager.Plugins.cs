using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Services
{
    public partial class PathManager
    {
        public string GetPackagesPath(params string[] paths)
        {
            var packagesPath = GetContentRootPath(DirectoryUtils.Packages, PathUtils.Combine(paths));
            DirectoryUtils.CreateDirectoryIfNotExists(packagesPath);
            return packagesPath;
        }

        public string PluginsPath => GetContentRootPath(DirectoryUtils.Plugins);

        public string GetPluginPath(string pluginId, params string[] paths)
        {
            return GetContentRootPath(DirectoryUtils.Plugins, pluginId, PathUtils.Combine(paths));
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

        public string ParsePluginUrl(string pluginId, string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (PageUtils.IsProtocolUrl(url)) return url;

            if (StringUtils.StartsWith(url, "~/"))
            {
                return GetWebRootUrl(url.Substring(1));
            }

            if (StringUtils.StartsWith(url, "@/"))
            {
                return GetAdminUrl(url.Substring(1));
            }

            return GetWebRootUrl(PageUtils.Combine(DirectoryUtils.Plugins, pluginId, url));
        }
    }
}
