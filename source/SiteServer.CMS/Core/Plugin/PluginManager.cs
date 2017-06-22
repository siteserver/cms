using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BaiRong.Core;
using SiteServer.Plugin;
using System.Threading;
using BaiRong.Core.Model;
using SiteServer.CMS.Core.Permissions;

namespace SiteServer.CMS.Core.Plugin
{
    /// <summary>
    /// The entry for managing SiteServer plugins
    /// </summary>
    public static class PluginManager
    {
        /// <summary>
        /// Directories that will hold SiteServer plugin directory
        /// </summary>

        public static List<PluginPair> AllPlugins { get; private set; }

        private static FileSystemWatcher _watcher;

        public static void LoadPlugins()
        {
            AllPlugins = new List<PluginPair>();

            var pluginsPath = PathUtils.GetPluginsPath();
            if (!Directory.Exists(pluginsPath))
            {
                Directory.CreateDirectory(pluginsPath);
            }

            Parallel.ForEach(DirectoryUtils.GetDirectoryPaths(pluginsPath), PluginUtils.AddPlugin);

            _watcher = new FileSystemWatcher
            {
                Path = pluginsPath,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                IncludeSubdirectories = true
            };
            _watcher.Created += Watcher_EventHandler;
            _watcher.Changed += Watcher_EventHandler;
            _watcher.Deleted += Watcher_EventHandlerDelete;
            _watcher.Renamed += Watcher_EventHandler;
            _watcher.EnableRaisingEvents = true;
        }

        private static void Watcher_EventHandler(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.ToLower().EndsWith(PluginUtils.PluginConfigName)) return;

            try
            {
                _watcher.EnableRaisingEvents = false;
                PluginUtils.OnConfigChanged(sender, e);
            }
            finally
            {
                _watcher.EnableRaisingEvents = true;
            }
        }

        private static void Watcher_EventHandlerDelete(object sender, FileSystemEventArgs e)
        {
            if (!PathUtils.IsDirectoryPath(e.FullPath)) return;

            try
            {
                _watcher.EnableRaisingEvents = false;
                PluginUtils.OnDirectoryDeleted(sender, e);
            }
            finally
            {
                _watcher.EnableRaisingEvents = true;
            }
        }

        public static void Install(string path)
        {
            if (File.Exists(path))
            {
                string tempFoler = Path.Combine(Path.GetTempPath(), "SiteServer\\plugins");
                if (Directory.Exists(tempFoler))
                {
                    Directory.Delete(tempFoler, true);
                }
                PluginUtils.UnZip(path, tempFoler, true);

                string iniPath = Path.Combine(tempFoler, "plugin.json");
                if (!File.Exists(iniPath))
                {
                    //MessageBox.Show("Install failed: plugin config is missing");
                    return;
                }

                PluginMetadata plugin = PluginUtils.GetMetadataFromJson(tempFoler);
                if (plugin == null || plugin.Name == null)
                {
                    //MessageBox.Show("Install failed: plugin config is invalid");
                    return;
                }

                string pluginFolerPath = PathUtils.GetSiteFilesPath("Plugins");

                string newPluginName = plugin.Name
                    .Replace("/", "_")
                    .Replace("\\", "_")
                    .Replace(":", "_")
                    .Replace("<", "_")
                    .Replace(">", "_")
                    .Replace("?", "_")
                    .Replace("*", "_")
                    .Replace("|", "_")
                    + "-" + Guid.NewGuid();
                string newPluginPath = Path.Combine(pluginFolerPath, newPluginName);
                string content = $"Do you want to install following plugin?{Environment.NewLine}{Environment.NewLine}" +
                                 $"Name: {plugin.Name}{Environment.NewLine}" +
                                 $"Version: {plugin.Version}{Environment.NewLine}" +
                                 $"Author: {plugin.Author}";
                PluginPair existingPlugin = PluginManager.GetPluginForId(plugin.Id);

                if (existingPlugin != null)
                {
                    content = $"Do you want to update following plugin?{Environment.NewLine}{Environment.NewLine}" +
                              $"Name: {plugin.Name}{Environment.NewLine}" +
                              $"Old Version: {existingPlugin.Metadata.Version}" +
                              $"{Environment.NewLine}New Version: {plugin.Version}" +
                              $"{Environment.NewLine}Author: {plugin.Author}";
                }

                if (existingPlugin != null && Directory.Exists(existingPlugin.Metadata.DirectoryPath))
                {
                    //when plugin is in use, we can't delete them. That's why we need to make plugin folder a random name
                    File.Create(Path.Combine(existingPlugin.Metadata.DirectoryPath, "NeedDelete.txt")).Close();
                }

                PluginUtils.UnZip(path, newPluginPath, true);
                Directory.Delete(tempFoler, true);

                //exsiting plugins may be has loaded by application,
                //if we try to delelte those kind of plugins, we will get a  error that indicate the
                //file is been used now.
                //current solution is to restart SiteServer. Ugly.
                //if (MainWindow.Initialized)
                //{
                //    Plugins.Initialize();
                //}
                //PluginManager.Api.RestarApp();
            }
        }

        public static PluginPair Delete(string pluginId)
        {
            var pluginPair = GetPluginForId(pluginId);
            if (DirectoryUtils.DeleteDirectoryIfExists(pluginPair.Metadata.DirectoryPath))
            {
                AllPlugins.Remove(pluginPair);
            }
            Thread.Sleep(1200);
            return pluginPair;
        }

        public static PluginPair Enable(string pluginId)
        {
            var pluginPair = GetPluginForId(pluginId);
            pluginPair.Metadata.Disabled = false;
            PluginUtils.SaveMetadataToJson(pluginPair.Metadata);
            Thread.Sleep(1200);
            return pluginPair;
        }

        public static PluginPair Disable(string pluginId)
        {
            var pluginPair = GetPluginForId(pluginId);
            pluginPair.Metadata.Disabled = true;
            PluginUtils.SaveMetadataToJson(pluginPair.Metadata);
            Thread.Sleep(1200);
            return pluginPair;
        }

        /// <summary>
        /// get specified plugin, return null if not found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static PluginPair GetPluginForId(string id)
        {
            return AllPlugins.FirstOrDefault(o => o.Metadata.Id == id);
        }

        public static IEnumerable<PluginPair> GetPluginsForInterface<T>() where T : IFeatures
        {
            return AllPlugins.Where(pluginPair => pluginPair.Plugin is T).ToList();
        }

        public static List<PermissionConfig> GetAllPermissions()
        {
            var permissions = new List<PermissionConfig>();

            foreach (var pluginPair in AllPlugins)
            {
                if (pluginPair.Metadata.Disabled || pluginPair.Metadata.Permissions == null) continue;

                foreach (var permissionsKey in pluginPair.Metadata.Permissions.Keys)
                {
                    var name = permissionsKey;
                    var text = pluginPair.Metadata.Permissions[permissionsKey];
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(text)) continue;

                    permissions.Add(new PermissionConfig(PluginUtils.GetPermissionName(pluginPair.Metadata.Id, name), pluginPair.Metadata.Name + "->" + text));
                }
            }

            return permissions;
        }

        public static List<PluginMenu> GetAllMenus(string topId, int siteId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
            var apiUrl = PageUtility.GetApiUrl(publishmentSystemInfo);

            var menus = new List<PluginMenu>();
            foreach (var pluginPair in AllPlugins)
            {
                if (pluginPair.Metadata.Disabled || pluginPair.Metadata.Menus == null) continue;

                var i = 1;
                foreach (var metadataMenu in pluginPair.Metadata.Menus)
                {
                    if (!StringUtils.EqualsIgnoreCase(metadataMenu.TopId, topId)) continue;

                    var menu = PluginUtils.GetMenu(pluginPair.Metadata.Id, metadataMenu, apiUrl, siteId, i++);

                    menus.Add(menu);
                }
            }

            return menus;
        }
    }
}
