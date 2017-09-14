using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Permissions;
using SiteServer.CMS.Model;
using SiteServer.CMS.WeiXin.WeiXinMP;
using SiteServer.Plugin;
using SiteServer.Plugin.Features;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin
{
    public static class PluginCache
    {
        private const string CacheKeyPrefix = "SiteServer.CMS.Core.Plugin.PluginCache.";

        private static void ClearAllCaches()
        {
            CacheUtils.RemoveByStartString(CacheKeyPrefix);
        }

        private static T GetCache<T>(string cacheName) where T : class
        {
            if (CacheUtils.Get(CacheKeyPrefix + cacheName) != null) return CacheUtils.Get(CacheKeyPrefix + cacheName) as T;

            return null;
        }

        private static void SetCache<T>(string cacheName, T obj) where T : class
        {
            CacheUtils.Insert(CacheKeyPrefix + cacheName, obj);
        }

        private static readonly SortedList<string, PluginPair> PluginSortedList = new SortedList<string, PluginPair>();

        private static readonly object AllPluginsLock = new object();

        public static void Remove(string pluginId)
        {
            lock (AllPluginsLock)
            {
                PluginSortedList.Remove(pluginId);
                ClearAllCaches();
            }
        }

        public static void Set(string pluginId, PluginPair pair)
        {
            lock (AllPluginsLock)
            {
                if (PluginSortedList.ContainsKey(pluginId))
                {
                    PluginSortedList[pluginId] = pair;
                }
                else
                {
                    PluginSortedList.Add(pluginId, pair);
                }
                ClearAllCaches();
            }
        }

        public static PluginMetadata GetMetadata(string pluginId)
        {
            lock (AllPluginsLock)
            {
                PluginPair pair;
                if (PluginSortedList.TryGetValue(pluginId, out pair))
                {
                    return pair.Metadata.Copy();
                }
                return null;
            }
        }

        public static void SetMetadata(PluginMetadata metadata)
        {
            lock (AllPluginsLock)
            {
                PluginPair pair;
                if (PluginSortedList.TryGetValue(metadata.Id, out pair))
                {
                    pair.Metadata = metadata;
                }
                ClearAllCaches();
            }
        }

        public static bool IsExists(string pluginId)
        {
            lock (AllPluginsLock)
            {
                return PluginSortedList.ContainsKey(pluginId);
            }
        }

        public static List<PluginPair> AllPluginPairs
        {
            get
            {
                // Use the same synchronization that prevents concurrent modifications
                lock (AllPluginsLock)
                {
                    return
                        PluginSortedList.Values.Where(pluginPair => pluginPair?.Metadata != null && pluginPair.Plugin != null)
                            .ToList();
                }
            }
        }

        public static List<PluginPair> GetEnabledPluginPairs<T>() where T : IPlugin
        {
            lock (AllPluginsLock)
            {
                return
                    PluginSortedList.Values.Where(
                            pair =>
                                pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                                pair.Plugin is T
                        )
                        .ToList();
            }
        }

        public static PluginPair GetEnabledPluginPair<T>(string pluginId) where T : IPlugin
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            lock (AllPluginsLock)
            {
                PluginPair pair;
                var isGet = PluginSortedList.TryGetValue(pluginId, out pair);
                if (isGet && pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                    pair.Plugin is T)
                {
                    return pair;
                }
                return null;
            }
        }

        public static List<PluginPair> GetEnabledPluginPairs<T1, T2>()
        {
            lock (AllPluginsLock)
            {
                return
                    PluginSortedList.Values.Where(
                            pair =>
                                pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                                (pair.Plugin is T1 || pair.Plugin is T2)
                        )
                        .ToList();
            }
        }

        public static List<PluginMetadata> GetEnabledPluginMetadatas<T>() where T : IPlugin
        {
            lock (AllPluginsLock)
            {
                return
                    PluginSortedList.Values.Where(
                        pair =>
                            pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                            pair.Plugin is T
                    ).Select(pluginPair => pluginPair.Metadata).ToList();
            }
        }

        public static PluginMetadata GetEnabledPluginMetadata<T>(string pluginId) where T : IPlugin
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            lock (AllPluginsLock)
            {
                PluginPair pair;
                var isGet = PluginSortedList.TryGetValue(pluginId, out pair);
                if (isGet && pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                    pair.Plugin is T)
                {
                    return pair.Metadata;
                }
                return null;
            }
        }

        public static T GetEnabledFeature<T>(string pluginId) where T : IPlugin
        {
            if (string.IsNullOrEmpty(pluginId)) return default(T);

            lock (AllPluginsLock)
            {
                PluginPair pair;
                var isGet = PluginSortedList.TryGetValue(pluginId, out pair);
                if (isGet && pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                    pair.Plugin is T)
                {
                    return (T) pair.Plugin;
                }
                return default(T);
            }
        }

        public static List<T> GetEnabledFeatures<T>() where T : IPlugin
        {
            lock (AllPluginsLock)
            {
                var pairs = PluginSortedList.Values.Where(
                        pair =>
                            pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                            pair.Plugin is T
                    )
                    .ToList();
                return pairs.Select(pluginPair => (T)pluginPair.Plugin).ToList();
            }
        }

        public static List<PermissionConfig> GetTopPermissions()
        {
            var permissions = GetCache<List<PermissionConfig>>(nameof(GetTopPermissions));
            if (permissions != null) return permissions;

            permissions = new List<PermissionConfig>();
            foreach (var pluginPair in GetEnabledPluginPairs<IMenu>())
            {
                var feature = (IMenu)pluginPair.Plugin;
                var menu = feature.GlobalMenu;
                if (menu != null)
                {
                    permissions.Add(new PermissionConfig(pluginPair.Metadata.Id, $"系统管理 -> {pluginPair.Metadata.DisplayName}（插件）"));
                }
            }

            SetCache(nameof(GetTopPermissions), permissions);

            return permissions;
        }

        public static List<PermissionConfig> GetSitePermissions(int siteId)
        {
            var pairs = GetEnabledPluginPairs<IMenu>();
            var permissions = new List<PermissionConfig>();

            foreach (var pluginPair in pairs)
            {
                var feature = (IMenu)pluginPair.Plugin;
                var menu = feature.Menu;
                if (menu != null)
                {
                    permissions.Add(new PermissionConfig(pluginPair.Metadata.Id, $"{pluginPair.Metadata.DisplayName}（插件）"));
                }
            }

            return permissions;
        }

        public static Dictionary<string, PluginMenu> GetTopMenus()
        {
            var menus = GetCache<Dictionary<string, PluginMenu>>(nameof(GetTopMenus));
            if (menus != null) return menus;

            menus = new Dictionary<string, PluginMenu>();

            var pairs = GetEnabledPluginPairs<IMenu>();
            if (pairs != null && pairs.Count > 0)
            {
                var apiUrl = PageUtils.GetApiUrl();

                foreach (var pluginPair in pairs)
                {
                    var feature = (IMenu)pluginPair.Plugin;
                    var metadataMenu = feature.GlobalMenu?.Invoke();
                    if (metadataMenu == null) continue;

                    var pluginMenu = PluginUtils.GetMenu(pluginPair.Metadata.Id, metadataMenu, apiUrl, 0, 0);

                    menus.Add(pluginPair.Metadata.Id, pluginMenu);
                }
            }

            SetCache(nameof(GetTopMenus), menus);

            return menus;
        }

        public static Dictionary<string, PluginMenu> GetSiteMenus(int siteId)
        {
            var pairs = GetEnabledPluginPairs<IMenu>();
            if (pairs == null || pairs.Count == 0) return null;

            var menus = new Dictionary<string, PluginMenu>();

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
            var apiUrl = PageUtility.GetInnerApiUrl(publishmentSystemInfo);

            foreach (var pluginPair in pairs)
            {
                var feature = (IMenu)pluginPair.Plugin;

                var metadataMenu = feature.Menu?.Invoke(siteId);
                if (metadataMenu == null) continue;
                var pluginMenu = PluginUtils.GetMenu(pluginPair.Metadata.Id, metadataMenu, apiUrl, siteId, 0);

                menus.Add(pluginPair.Metadata.Id, pluginMenu);
            }

            return menus;
        }

        //public static List<ContentModelInfo> GetAllContentModels(PublishmentSystemInfo publishmentSystemInfo)
        //{
        //    var cacheName = nameof(GetAllContentModels) + publishmentSystemInfo.PublishmentSystemId;
        //    var contentModels = GetCache<List<ContentModelInfo>>(cacheName);
        //    if (contentModels != null) return contentModels;

        //    contentModels = new List<ContentModelInfo>();

        //    foreach (var pluginPair in GetEnabledPluginPairs<IContentModel>())
        //    {
        //        var model = pluginPair.Plugin as IContentModel;

        //        if (model == null) continue;

        //        var tableName = publishmentSystemInfo.AuxiliaryTableForContent;
        //        var tableType = EAuxiliaryTableType.BackgroundContent;
        //        if (model.ContentTableColumns != null && model.ContentTableColumns.Count > 0)
        //        {
        //            tableName = pluginPair.Metadata.Id;
        //            tableType = EAuxiliaryTableType.Custom;
        //        }

        //        contentModels.Add(new ContentModelInfo(
        //            pluginPair.Metadata.Id,
        //            pluginPair.Metadata.Id,
        //            $"插件：{pluginPair.Metadata.DisplayName}",
        //            tableName,
        //            tableType,
        //            PageUtils.GetPluginDirectoryUrl(pluginPair.Metadata.Id, pluginPair.Metadata.Icon))
        //        );
        //    }

        //    SetCache(cacheName, contentModels);

        //    return contentModels;
        //}

        public static List<PluginMetadata> GetAllChannels(bool includeContentTable)
        {
            var list = new List<PluginMetadata>();

            var pairs = GetEnabledPluginPairs<IChannel>();
            foreach (var pluginPair in pairs)
            {
                if (!includeContentTable && pluginPair.Plugin is IContentTable) continue;

                list.Add(pluginPair.Metadata);
            }

            return list;
        }

        public static List<PluginMetadata> GetChannels(NodeInfo nodeInfo, bool includeContentTable)
        {
            var list = new List<PluginMetadata>();
            var pluginIds = TranslateUtils.StringCollectionToStringList(nodeInfo.Additional.PluginIds);
            if (!string.IsNullOrEmpty(nodeInfo.ContentModelId))
            {
                pluginIds.Add(nodeInfo.ContentModelId);
            }

            var pairs = GetEnabledPluginPairs<IChannel>();
            foreach (var pluginPair in pairs)
            {
                var pluginId = pluginPair.Metadata.Id;
                if (!pluginIds.Contains(pluginId)) continue;

                if (!includeContentTable && pluginPair.Plugin is IContentTable) continue;

                list.Add(pluginPair.Metadata);
            }

            return list;
        }

        public static Dictionary<string, IChannel> GetChannelFeatures(NodeInfo nodeInfo)
        {
            var dict = new Dictionary<string, IChannel>();
            var pluginIds = TranslateUtils.StringCollectionToStringList(nodeInfo.Additional.PluginIds);
            if (!string.IsNullOrEmpty(nodeInfo.ContentModelId))
            {
                pluginIds.Add(nodeInfo.ContentModelId);
            }

            var pairs = GetEnabledPluginPairs<IChannel>();
            foreach (var pluginPair in pairs)
            {
                var pluginId = pluginPair.Metadata.Id;
                if (!pluginIds.Contains(pluginId)) continue;

                var feature = (IChannel) pluginPair.Plugin;

                dict[pluginId] = feature;
            }

            return dict;
        }

        //public static List<ContentModelInfo> GetAllContentModels(PublishmentSystemInfo publishmentSystemInfo)
        //{
        //    var cacheName = nameof(GetAllContentModels) + publishmentSystemInfo.PublishmentSystemId;
        //    var contentModels = GetCache<List<ContentModelInfo>>(cacheName);
        //    if (contentModels != null) return contentModels;

        //    contentModels = new List<ContentModelInfo>();

        //    foreach (var pluginPair in GetEnabledPluginPairs<IContentModel>())
        //    {
        //        var model = pluginPair.Plugin as IContentModel;

        //        if (model == null) continue;

        //        var links = new List<PluginContentLink>();
        //        if (model.ContentLinks != null)
        //        {
        //            links.AddRange(model.ContentLinks.Select(link => new PluginContentLink
        //            {
        //                Text = link.Text,
        //                Href = PageUtils.GetPluginDirectoryUrl(pluginPair.Metadata.Id, link.Href),
        //                Target = link.Target
        //            }));
        //        }
        //        var tableName = publishmentSystemInfo.AuxiliaryTableForContent;
        //        var tableType = EAuxiliaryTableType.BackgroundContent;
        //        if (model.IsCustomContentTable && model.CustomContentTableColumns != null && model.CustomContentTableColumns.Count > 0)
        //        {
        //            tableName = pluginPair.Metadata.Id;
        //            tableType = EAuxiliaryTableType.Custom;
        //        }

        //        contentModels.Add(new ContentModelInfo(
        //            pluginPair.Metadata.Id,
        //            pluginPair.Metadata.Id,
        //            $"插件：{pluginPair.Metadata.DisplayName}",
        //            tableName,
        //            tableType,
        //            PageUtils.GetPluginDirectoryUrl(pluginPair.Metadata.Id, pluginPair.Metadata.Icon),
        //            links)
        //        );
        //    }

        //    SetCache(cacheName, contentModels);

        //    return contentModels;
        //}

        public static Dictionary<string, Func<PluginParseContext, string>> GetParses()
        {
            var elementsToParse = GetCache<Dictionary<string, Func<PluginParseContext, string>>>(nameof(GetParses));
            if (elementsToParse != null) return elementsToParse;

            elementsToParse = new Dictionary<string, Func<PluginParseContext, string>>();

            var plugins = GetEnabledFeatures<IParse>();
            if (plugins != null && plugins.Count > 0)
            {
                foreach (var plugin in plugins)
                {
                    if (plugin.ElementsToParse != null && plugin.ElementsToParse.Count > 0)
                    {
                        foreach (var elementName in plugin.ElementsToParse.Keys)
                        {
                            elementsToParse[elementName.ToLower()] = plugin.ElementsToParse[elementName];
                        }
                    }
                }
            }

            SetCache(nameof(GetParses), elementsToParse);

            return elementsToParse;
        }

        public static List<Func<PluginRenderContext, string>> GetRenders()
        {
            var renders = GetCache<List<Func<PluginRenderContext, string>>>(nameof(GetRenders));
            if (renders != null) return renders;

            renders = new List<Func<PluginRenderContext, string>>();

            var plugins = GetEnabledFeatures<IRender>();
            if (plugins != null && plugins.Count > 0)
            {
                foreach (var plugin in plugins)
                {
                    if (plugin.Render != null)
                    {
                        renders.Add(plugin.Render);
                    }
                }
            }

            SetCache(nameof(GetRenders), renders);

            return renders;
        }

        public static List<Action<object, FileSystemEventArgs>> GetFileSystemChangedActions()
        {
            var actions = GetCache<List<Action<object, FileSystemEventArgs>>>(nameof(GetFileSystemChangedActions));
            if (actions != null) return actions;

            actions = new List<Action<object, FileSystemEventArgs>>();

            var plugins = GetEnabledFeatures<IFileSystem>();
            if (plugins != null && plugins.Count > 0)
            {
                foreach (var plugin in plugins)
                {
                    if (plugin.OnFileSystemChanged != null)
                    {
                        actions.Add(plugin.OnFileSystemChanged);
                    }
                }
            }

            SetCache(nameof(GetFileSystemChangedActions), actions);

            return actions;
        }

        public static List<Action<EventArgs>> GetPageAdminPreLoadActions()
        {
            var actions = GetCache<List<Action<EventArgs>>>(nameof(GetPageAdminPreLoadActions));
            if (actions != null) return actions;

            actions = new List<Action<EventArgs>>();

            var plugins = GetEnabledFeatures<IPageAdmin>();
            if (plugins != null && plugins.Count > 0)
            {
                foreach (var plugin in plugins)
                {
                    if (plugin.OnPageAdminPreLoad != null)
                    {
                        actions.Add(plugin.OnPageAdminPreLoad);
                    }
                }
            }

            SetCache(nameof(GetPageAdminPreLoadActions), actions);

            return actions;
        }

        public static List<Action<EventArgs>> GetPageAdminLoadCompleteActions()
        {
            var actions = GetCache<List<Action<EventArgs>>>(nameof(GetPageAdminLoadCompleteActions));
            if (actions != null) return actions;

            actions = new List<Action<EventArgs>>();

            var plugins = GetEnabledFeatures<IPageAdmin>();
            if (plugins != null && plugins.Count > 0)
            {
                foreach (var plugin in plugins)
                {
                    if (plugin.OnPageAdminLoadComplete != null)
                    {
                        actions.Add(plugin.OnPageAdminLoadComplete);
                    }
                }
            }

            SetCache(nameof(GetPageAdminLoadCompleteActions), actions);

            return actions;
        }
    }
}
