using System;
using System.Collections.Generic;
using System.Linq;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Permissions;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Features;

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

        public static bool IsExists(string pluginId)
        {
            lock (AllPluginsLock)
            {
                return PluginSortedList.ContainsKey(pluginId);
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

        public static T GetFeature<T>(string pluginId) where T : IPlugin
        {
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

        public static List<T> GetFeatures<T>() where T : IPlugin
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
                var menu = feature.GetTopMenu();
                if (menu != null)
                {
                    permissions.Add(new PermissionConfig(pluginPair.Metadata.Id, $"插件（{pluginPair.Metadata.DisplayName}）"));
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

                    var metadataMenu = feature.GetTopMenu();
                    if (metadataMenu == null) continue;

                    var menu = PluginUtils.GetMenu(pluginPair.Metadata.Id, metadataMenu, apiUrl, 0, 0);

                    menus.Add(pluginPair.Metadata.Id, menu);
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
            var contentModels = GetCache<List<ContentModelInfo>>(cacheName);
            if (contentModels != null) return contentModels;

            contentModels = new List<ContentModelInfo>();

            foreach (var pluginPair in GetEnabledPluginPairs<IContentModel>())
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

            SetCache(cacheName, contentModels);

            return contentModels;
        }

        public static Dictionary<string, Func<PluginParserContext, string>> GetParsers()
        {
            var parsers = GetCache<Dictionary<string, Func<PluginParserContext, string>>>(nameof(GetParsers));
            if (parsers != null) return parsers;

            parsers = new Dictionary<string, Func<PluginParserContext, string>>();

            var plugins = GetFeatures<IParser>();
            if (plugins != null && plugins.Count > 0)
            {
                foreach (var plugin in plugins)
                {
                    if (plugin.ElementNames == null || plugin.ElementNames.Count == 0) continue;

                    foreach (var elementName in plugin.ElementNames)
                    {
                        parsers[elementName.ToLower()] = plugin.Parse;
                    }
                }
            }

            SetCache(nameof(GetParsers), parsers);

            return parsers;
        }
    }
}
