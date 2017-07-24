using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
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

            var contentTable = plugin as IContentModel;
            if (!string.IsNullOrEmpty(contentTable?.CustomContentTableName) && contentTable.CustomContentTableColumns != null)
            {
                var tableName = contentTable.CustomContentTableName;
                if (!BaiRongDataProvider.DatabaseDao.IsTableExists(tableName))
                {
                    var tableInfo = new AuxiliaryTableInfo(tableName, $"插件内容表：{metadata.DisplayName}", 0,
                        EAuxiliaryTableType.Custom, false, false, false, string.Empty);
                    BaiRongDataProvider.TableCollectionDao.Insert(tableInfo);

                    foreach (var tableColumn in contentTable.CustomContentTableColumns)
                    {
                        if (string.IsNullOrEmpty(tableColumn.AttributeName)) continue;

                        var tableMetadataInfo = new TableMetadataInfo(0, tableName, tableColumn.AttributeName,
                            tableColumn.DataType, tableColumn.DataLength, 0, true);
                        BaiRongDataProvider.TableMetadataDao.Insert(tableMetadataInfo);

                        var tableStyleInfo = TableStyleManager.GetTableStyleInfo(ETableStyle.BackgroundContent, tableName,
                            tableColumn.AttributeName, new List<int> { 0 });
                        tableStyleInfo.DisplayName = tableColumn.DisplayName;
                        tableStyleInfo.InputType = InputTypeUtils.GetValue(tableColumn.InputType);
                        tableStyleInfo.DefaultValue = tableColumn.DefaultValue;
                        tableStyleInfo.Additional.IsValidate = true;
                        tableStyleInfo.Additional.IsRequired = tableColumn.IsRequired;
                        tableStyleInfo.Additional.MinNum = tableColumn.MinNum;
                        tableStyleInfo.Additional.MaxNum = tableColumn.MaxNum;
                        tableStyleInfo.Additional.RegExp = tableColumn.RegExp;
                        tableStyleInfo.Additional.Width = tableColumn.Width;
                        TableStyleManager.Insert(tableStyleInfo, ETableStyle.BackgroundContent);
                    }

                    BaiRongDataProvider.TableMetadataDao.CreateAuxiliaryTable(tableName);
                }
            }

            var table = plugin as ITable;
            if (table?.Tables != null)
            {
                foreach (var tableName in table.Tables.Keys)
                {
                    if (BaiRongDataProvider.DatabaseDao.IsTableExists(tableName)) continue;

                    var tableColumns = table.Tables[tableName];
                    if (tableColumns != null && tableColumns.Count > 0)
                    {
                        BaiRongDataProvider.DatabaseDao.CreatePluginTable(tableName, tableColumns);
                    }
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

        public static T GetHook<T>(string pluginId) where T : IPlugin
        {
            try
            {
                return AllPlugins.GetHook<T>(pluginId);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex, "插件:GetHook");
                return default(T);
            }
        }

        public static List<T> GetHooks<T>() where T : IPlugin
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

            var pairs = AllPlugins.GetEnabledPluginPairs<IContentModel>();
            foreach (var pluginPair in pairs)
            {
                var model = pluginPair.Plugin as IContentModel;

                if (model == null) continue;

                var links = new List<PluginContentLink>();
                if (model.ContentLinks != null)
                {
                    links.AddRange(model.ContentLinks.Select(link => new PluginContentLink
                    {
                        Text = link.Text,
                        Href = PageUtils.GetPluginDirectoryUrl(pluginPair.Metadata.Id, link.Href),
                        Target = link.Target
                    }));
                }
                var tableName = publishmentSystemInfo.AuxiliaryTableForContent;
                var tableType = EAuxiliaryTableType.BackgroundContent;
                if (model.IsCustomContentTable && model.CustomContentTableColumns != null && model.CustomContentTableColumns.Count > 0)
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
