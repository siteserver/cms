using System;
using System.Collections.Generic;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Model;
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
                if (string.IsNullOrEmpty(service.ContentTableName) || service.ContentTableColumns == null || service.ContentTableColumns.Count == 0) continue;

                list.Add(service.Metadata);
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
                else if (service.ContentMenus != null && service.ContentMenus.Count > 0)
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

        public static Dictionary<string, List<Menu>> GetContentMenus(ChannelInfo channelInfo)
        {
            if (string.IsNullOrEmpty(channelInfo.ContentRelatedPluginIds) &&
                string.IsNullOrEmpty(channelInfo.ContentModelPluginId))
            {
                return null;
            }

            var dict = new Dictionary<string, List<Menu>>();
            var pluginIds = TranslateUtils.StringCollectionToStringList(channelInfo.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(channelInfo.ContentModelPluginId))
            {
                pluginIds.Add(channelInfo.ContentModelPluginId);
            }

            foreach (var service in PluginManager.Services)
            {
                if (!pluginIds.Contains(service.PluginId) || service.ContentMenus == null || service.ContentMenus.Count == 0) continue;

                dict[service.PluginId] = service.ContentMenus;
            }

            return dict;
        }

        public static Dictionary<string, Dictionary<string, Func<IContentContext, string>>> GetContentColumns(ChannelInfo channelInfo)
        {
            if (string.IsNullOrEmpty(channelInfo.ContentRelatedPluginIds) &&
                string.IsNullOrEmpty(channelInfo.ContentModelPluginId))
            {
                return null;
            }

            var dict = new Dictionary<string, Dictionary<string, Func<IContentContext, string>>>();
            var pluginIds = TranslateUtils.StringCollectionToStringList(channelInfo.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(channelInfo.ContentModelPluginId))
            {
                pluginIds.Add(channelInfo.ContentModelPluginId);
            }

            foreach (var service in PluginManager.Services)
            {
                if (!pluginIds.Contains(service.PluginId) || service.ContentColumns == null || service.ContentColumns.Count == 0) continue;

                dict[service.PluginId] = service.ContentColumns;
            }

            return dict;
        }
    }
}
