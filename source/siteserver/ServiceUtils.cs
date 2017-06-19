using System;
using System.IO;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;

namespace siteserver
{
    public class ServiceUtils
    {
        public static bool IsSiteServerDir
        {
            get
            {
                var physicalApplicationPath = Environment.CurrentDirectory;
                var webConfigPath = PathUtils.Combine(physicalApplicationPath, "web.config");
                var binPath = PathUtils.Combine(physicalApplicationPath, "bin/SiteServer.CMS.dll");

                return FileUtils.IsFileExists(webConfigPath) && FileUtils.IsFileExists(binPath);
            }
        }

        public static bool IsInitialized => BaiRongDataProvider.ConfigDao.IsInitialized();

        public static void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            foreach (var pluginPair in PluginManager.GetPluginsForInterface<IFileSystemWatcher>())
            {
                var watcher = (IFileSystemWatcher) pluginPair.Plugin;
                watcher.OnFileChanged(sender, e);
            }
        }
    }
}
