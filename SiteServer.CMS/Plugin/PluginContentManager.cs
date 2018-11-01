using System;
using System.Collections.Generic;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Plugin
{
    public static class PluginContentManager
    {
        public static List<IMetadata> GetContentModelPlugins()
        {
            var list = new List<IMetadata>();

            foreach (var service in PluginManager.Services)
            {
                if (PluginContentTableManager.IsContentTable(service))
                {
                    list.Add(service.Metadata);
                }
            }

            return list;
        }

        public static List<string> GetContentTableNameList()
        {
            var list = new List<string>();

            foreach (var service in PluginManager.Services)
            {
                if (PluginContentTableManager.IsContentTable(service))
                {
                    if (!StringUtils.ContainsIgnoreCase(list, service.ContentTableName))
                    {
                        list.Add(service.ContentTableName);
                    }
                }
            }

            return list;
        }

        public static List<IMetadata> GetAllContentRelatedPlugins(bool includeContentTable)
        {
            var list = new List<IMetadata>();

            foreach (var service in PluginManager.Services)
            {
                var isContentModel = PluginContentTableManager.IsContentTable(service);

                if (!includeContentTable && isContentModel) continue;

                if (isContentModel)
                {
                    list.Add(service.Metadata);
                }
                else if (service.ContentMenuFuncs != null)
                {
                    list.Add(service.Metadata);
                }
                else if (service.ContentColumns != null && service.ContentColumns.Count > 0)
                {
                    list.Add(service.Metadata);
                }
            }

            return list;
        }

        public static List<ServiceImpl> GetContentPlugins(ChannelInfo channelInfo, bool includeContentTable)
        {
            var list = new List<ServiceImpl>();
            var pluginIds = TranslateUtils.StringCollectionToStringList(channelInfo.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(channelInfo.ContentModelPluginId))
            {
                pluginIds.Add(channelInfo.ContentModelPluginId);
            }

            foreach (var service in PluginManager.Services)
            {
                if (!pluginIds.Contains(service.PluginId)) continue;

                if (!includeContentTable && PluginContentTableManager.IsContentTable(service)) continue;

                list.Add(service);
            }

            return list;
        }

        public static List<string> GetContentPluginIds(ChannelInfo channelInfo)
        {
            if (string.IsNullOrEmpty(channelInfo.ContentRelatedPluginIds) &&
                string.IsNullOrEmpty(channelInfo.ContentModelPluginId))
            {
                return null;
            }

            var pluginIds = TranslateUtils.StringCollectionToStringList(channelInfo.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(channelInfo.ContentModelPluginId))
            {
                pluginIds.Add(channelInfo.ContentModelPluginId);
            }

            return pluginIds;
        }

        public static Dictionary<string, Dictionary<string, Func<IContentContext, string>>> GetContentColumns(List<string> pluginIds)
        {
            var dict = new Dictionary<string, Dictionary<string, Func<IContentContext, string>>>();
            if (pluginIds == null || pluginIds.Count == 0) return dict;

            foreach (var service in PluginManager.Services)
            {
                if (!pluginIds.Contains(service.PluginId) || service.ContentColumns == null || service.ContentColumns.Count == 0) continue;

                dict[service.PluginId] = service.ContentColumns;
            }

            return dict;
        }
    }
}
