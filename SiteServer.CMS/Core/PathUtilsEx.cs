using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public static class PathUtilsEx
    {
        public static string GetCurrentPagePath()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.PhysicalPath;
            }
            return string.Empty;
        }

        public static string MapPath(string virtualPath)
        {
            virtualPath = PathUtils.RemovePathInvalidChar(virtualPath);
            string retval;
            if (!string.IsNullOrEmpty(virtualPath))
            {
                if (virtualPath.StartsWith("~"))
                {
                    virtualPath = virtualPath.Substring(1);
                }
                virtualPath = PageUtils.Combine("~", virtualPath);
            }
            else
            {
                virtualPath = "~/";
            }
            if (HttpContext.Current != null)
            {
                retval = HttpContext.Current.Server.MapPath(virtualPath);
            }
            else
            {
                var rootPath = WebConfigUtils.PhysicalApplicationPath;

                virtualPath = !string.IsNullOrEmpty(virtualPath) ? virtualPath.Substring(2) : string.Empty;
                retval = PathUtils.Combine(rootPath, virtualPath);
            }

            if (retval == null) retval = string.Empty;
            return retval.Replace("/", "\\");
        }

        public static string GetSiteFilesPath(params string[] paths)
        {
            return MapPath(PathUtils.Combine("~/" + DirectoryUtils.SiteFiles.DirectoryName, PathUtils.Combine(paths)));
        }

        public static string PluginsPath => GetSiteFilesPath(DirectoryUtils.SiteFiles.Plugins);

        public static string GetPluginPath(string pluginId, params string[] paths)
        {
            return GetSiteFilesPath(DirectoryUtils.SiteFiles.Plugins, pluginId, PathUtils.Combine(paths));
        }

        public static string GetPluginNuspecPath(string pluginId)
        {
            return GetPluginPath(pluginId, pluginId + ".nuspec");
        }

        public static string GetPluginDllDirectoryPath(string pluginId)
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

        public static string GetPackagesPath(params string[] paths)
        {
            var packagesPath = GetSiteFilesPath(DirectoryUtils.SiteFiles.Packages, PathUtils.Combine(paths));
            DirectoryUtils.CreateDirectoryIfNotExists(packagesPath);
            return packagesPath;
        }

        public static string GetMenusPath(params string[] paths)
        {
            return PathUtils.Combine(SiteServerAssets.GetPath("menus"), PathUtils.Combine(paths));
        }
    }
}
