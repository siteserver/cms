using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaiRong.Core;
using BaiRong.Core.IO;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Net;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Permissions;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Hooks;

namespace SiteServer.CMS.Plugin
{
    /// <summary>
    /// The entry for managing SiteServer plugins
    /// </summary>
    public static class PluginManager
    {
        public static PluginEnvironment Environment { get; private set; }

        private static FileSystemWatcher _watcher;

        public static void Load(PluginEnvironment environment)
        {
            Environment = environment;

            try
            {
                var pluginsPath = PathUtils.GetPluginsPath();
                if (!Directory.Exists(pluginsPath))
                {
                    Directory.CreateDirectory(pluginsPath);
                }

                Parallel.ForEach(DirectoryUtils.GetDirectoryPaths(pluginsPath), PluginUtils.ActivePlugin);

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
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex, "载入插件时报错");
            }
        }

        private static void Watcher_EventHandler(object sender, FileSystemEventArgs e)
        {
            var fullPath = e.FullPath.ToLower();
            if (!fullPath.EndsWith(PluginUtils.PluginConfigName) && !fullPath.EndsWith(".dll")) return;

            try
            {
                _watcher.EnableRaisingEvents = false;
                PluginUtils.OnConfigOrDllChanged(sender, e);
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

        public static void DeactiveAndRemove(PluginPair pluginPair)
        {
            pluginPair.Plugin.Deactive(pluginPair.Context);
            AllPlugins.Remove(pluginPair.Metadata.Id);
        }

        public static bool ActiveAndAdd(PluginMetadata metadata, IPlugin plugin)
        {
            if (metadata == null || plugin == null) return false;
            
            var s = Stopwatch.StartNew();

            var context = new PluginContext(Environment, metadata, new PublicApiInstance(metadata));
            plugin.Active(context);

            var contentTable = plugin as IContentTable;
            if (contentTable != null)
            {
                if (!BaiRongDataProvider.TableCollectionDao.IsTableExists(metadata.Id))
                {
                    var tableName = metadata.Id;
                    var tableInfo = new AuxiliaryTableInfo(tableName, $"插件内容表：{metadata.DisplayName}", 0,
                        EAuxiliaryTableType.Custom, false, false, true, string.Empty);
                    BaiRongDataProvider.TableCollectionDao.Insert(tableInfo);
                    foreach (var tableColumn in contentTable.ContentTableColumns)
                    {
                        var tableMetadataInfo = new TableMetadataInfo(0, tableName, tableColumn.AttributeName,
                            tableColumn.DataType, tableColumn.DataLength, 0, true);
                        BaiRongDataProvider.TableMetadataDao.Insert(tableMetadataInfo);
                    }
                    BaiRongDataProvider.TableMetadataDao.CreateAuxiliaryTable(tableName);
                }
                else
                {
                    
                }
            }

            var milliseconds = s.ElapsedMilliseconds;

            metadata.InitTime = milliseconds;

            var pair = new PluginPair(context, plugin);

            AllPlugins.Set(metadata.Id, pair);
            return true;
        }

        public static bool Install(string pluginId, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(pluginId)) return false;

            try
            {
                var zipFilePath = PathUtility.GetSiteTemplatesPath(pluginId + ".zip");
                FileUtils.DeleteFileIfExists(zipFilePath);

                var downloadUrl = PluginUtils.GetDownloadUrl(pluginId);
                WebClientUtils.SaveRemoteFileToLocal(downloadUrl, zipFilePath);

                var directoryPath = PathUtils.GetPluginsPath(pluginId);
                ZipUtils.UnpackFiles(zipFilePath, directoryPath);

                var jsonPath = PathUtils.Combine(directoryPath, PluginUtils.PluginConfigName);
                if (!FileUtils.IsFileExists(jsonPath))
                {
                    errorMessage = $"插件配置文件{PluginUtils.PluginConfigName}不存在";
                    return false;
                }

                var plugin = PluginUtils.GetMetadataFromJson(directoryPath);
                if (plugin == null)
                {
                    errorMessage = "插件配置文件不正确";
                    return false;
                }

                if (IsExists(plugin.Id))
                {
                    errorMessage = "插件已存在";
                    return false;
                    //errorMessage = $"Do you want to update following plugin?{Environment.NewLine}{Environment.NewLine}" +
                    //          $"Name: {plugin.Name}{Environment.NewLine}" +
                    //          $"Old Version: {existingPlugin.Metadata.Version}" +
                    //          $"{Environment.NewLine}New Version: {plugin.Version}" +
                    //          $"{Environment.NewLine}Author: {plugin.Author}";
                }

                ZipUtils.UnpackFiles(zipFilePath, PathUtils.GetPluginsPath(pluginId));

                FileUtils.DeleteFileIfExists(zipFilePath);
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }

            return true;
        }

        public static PluginMetadata Delete(string pluginId)
        {
            var metadata = AllPlugins.GetMetadata(pluginId);
            if (metadata != null)
            {
                if (DirectoryUtils.DeleteDirectoryIfExists(metadata.DirectoryPath))
                {
                    AllPlugins.Remove(pluginId);
                }
            }
            Thread.Sleep(1200);
            return metadata;
        }

        public static PluginMetadata Enable(string pluginId)
        {
            var metadata = AllPlugins.GetMetadata(pluginId);
            if (metadata != null)
            {
                metadata.Disabled = false;
                AllPlugins.SetMetadata(metadata);
                PluginUtils.SaveMetadataToJson(metadata);
            }
            Thread.Sleep(1200);
            return metadata;
        }

        public static PluginMetadata Disable(string pluginId)
        {
            var metadata = AllPlugins.GetMetadata(pluginId);
            if (metadata != null)
            {
                metadata.Disabled = true;
                AllPlugins.SetMetadata(metadata);
                PluginUtils.SaveMetadataToJson(metadata);
            }
            Thread.Sleep(1200);
            return metadata;
        }

        public static List<PluginPair> GetAllPluginPairs()
        {
            return AllPlugins.AllPluginPairs;
        }

        public static bool IsExists(string pluginId)
        {
            return AllPlugins.IsExists(pluginId);
        }

        public static T GetHook<T>(string pluginId) where T : IHooks
        {
            return AllPlugins.GetHook<T>(pluginId);
        }

        public static List<T> GetHooks<T>() where T : IHooks
        {
            return AllPlugins.GetHooks<T>();
        }

        public static List<PermissionConfig> GetTopPermissions()
        {
            var pairs = AllPlugins.GetEnabledPluginPairs<IMenu>();
            var permissions = new List<PermissionConfig>();

            foreach (var pluginPair in pairs)
            {
                var feature = (IMenu)pluginPair.Plugin;
                var menu = feature.GetTopMenu();
                if (menu != null)
                {
                    permissions.Add(new PermissionConfig(pluginPair.Metadata.Id, $"插件（{pluginPair.Metadata.DisplayName}）"));
                }
            }

            return permissions;
        }

        public static List<PermissionConfig> GetSitePermissions(int siteId)
        {
            var pairs = AllPlugins.GetEnabledPluginPairs<IMenu>();
            var permissions = new List<PermissionConfig>();

            foreach (var pluginPair in pairs)
            {
                var feature = (IMenu)pluginPair.Plugin;
                var menu = feature.GetSiteMenu(siteId);
                if (menu != null)
                {
                    permissions.Add(new PermissionConfig(pluginPair.Metadata.Id, $"插件（{pluginPair.Metadata.DisplayName}）"));
                }
            }

            return permissions;
        }

        public static Dictionary<string, PluginMenu> GetTopMenus()
        {
            var pairs = AllPlugins.GetEnabledPluginPairs<IMenu>();
            if (pairs == null || pairs.Count == 0) return null;

            var menus = new Dictionary<string, PluginMenu>();

            var apiUrl = PageUtils.GetApiUrl();

            foreach (var pluginPair in pairs)
            {
                var feature = (IMenu)pluginPair.Plugin;

                var metadataMenu = feature.GetTopMenu();
                if (metadataMenu == null) continue;

                var menu = PluginUtils.GetMenu(pluginPair.Metadata.Id, metadataMenu, apiUrl, 0, 0);

                menus.Add(pluginPair.Metadata.Id, menu);
            }

            return menus;
        }

        public static Dictionary<string, PluginMenu> GetSiteMenus(int siteId)
        {
            var pairs = AllPlugins.GetEnabledPluginPairs<IMenu>();
            if (pairs == null || pairs.Count == 0) return null;

            var menus = new Dictionary<string, PluginMenu>();

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
            var apiUrl = PageUtility.GetApiUrl(publishmentSystemInfo);

            foreach (var pluginPair in pairs)
            {
                var feature = (IMenu)pluginPair.Plugin;

                var metadataMenu = feature.GetSiteMenu(siteId);
                if (metadataMenu == null) continue;
                var menu = PluginUtils.GetMenu(pluginPair.Metadata.Id, metadataMenu, apiUrl, siteId, 0);

                menus.Add(pluginPair.Metadata.Id, menu);
            }

            return menus;
        }

        public static List<ContentModelInfo> GetAllContentModels(PublishmentSystemInfo publishmentSystemInfo)
        {
            var cacheName = nameof(GetAllContentModels) + publishmentSystemInfo.PublishmentSystemId;
            var contentModels = PluginCache.GetCache<List<ContentModelInfo>>(cacheName);
            if (contentModels != null) return contentModels;

            contentModels = new List<ContentModelInfo>();

            var pairs = AllPlugins.GetEnabledPluginPairs<IContentExtra, IContentTable>();
            foreach (var pluginPair in pairs)
            {
                var extra = pluginPair.Plugin as IContentExtra;
                var table = pluginPair.Plugin as IContentTable;

                var links = new List<PluginContentLink>();
                if (extra?.ContentLinks != null)
                {
                    links.AddRange(extra.ContentLinks.Select(link => new PluginContentLink
                    {
                        Text = link.Text,
                        Href = PageUtils.GetPluginDirectoryUrl(pluginPair.Metadata.Id, link.Href),
                        Target = link.Target
                    }));
                }
                var tableName = publishmentSystemInfo.AuxiliaryTableForContent;
                var tableType = EAuxiliaryTableType.BackgroundContent;
                if (table != null)
                {
                    tableName = pluginPair.Metadata.Id;
                    tableType = EAuxiliaryTableType.Custom;
                }

                contentModels.Add(new ContentModelInfo(
                    pluginPair.Metadata.Id,
                    pluginPair.Metadata.Id,
                    $"插件：{pluginPair.Metadata.DisplayName}",
                    tableName,
                    tableType,
                    PageUtils.GetPluginDirectoryUrl(pluginPair.Metadata.Id, pluginPair.Metadata.Icon),
                    links)
                );
            }

            PluginCache.SetCache(cacheName, contentModels);

            return contentModels;
        }
    }
}
