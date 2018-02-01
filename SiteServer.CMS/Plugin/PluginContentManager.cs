using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Plugin
{
    public class PluginContentManager
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
                else if (service.ContentLinks != null && service.ContentLinks.Count > 0)
                {
                    list.Add(service.Metadata);
                }
            }

            return list;
        }

        public static List<PluginService> GetContentPlugins(ChannelInfo channelInfo, bool includeContentTable)
        {
            var list = new List<PluginService>();
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

        public static Dictionary<string, List<HyperLink>> GetContentLinks(ChannelInfo nodeInfo)
        {
            if (string.IsNullOrEmpty(nodeInfo.ContentRelatedPluginIds) &&
                string.IsNullOrEmpty(nodeInfo.ContentModelPluginId))
            {
                return null;
            }

            var dict = new Dictionary<string, List<HyperLink>>();
            var pluginIds = TranslateUtils.StringCollectionToStringList(nodeInfo.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(nodeInfo.ContentModelPluginId))
            {
                pluginIds.Add(nodeInfo.ContentModelPluginId);
            }

            foreach (var service in PluginManager.Services)
            {
                if (!pluginIds.Contains(service.PluginId) || service.ContentLinks == null || service.ContentLinks.Count == 0) continue;

                dict[service.PluginId] = service.ContentLinks;
            }

            return dict;
        }

        //public static Dictionary<string, Dictionary<string, Func<int, int, IAttributes, string>>> GetContentFormCustomized(ChannelInfo nodeInfo)
        //{
        //    if (string.IsNullOrEmpty(nodeInfo.ContentRelatedPluginIds) &&
        //        string.IsNullOrEmpty(nodeInfo.ContentModelPluginId))
        //    {
        //        return null;
        //    }

        //    var dict = new Dictionary<string, Dictionary<string, Func<int, int, IAttributes, string>>>();

        //    var pluginIds = TranslateUtils.StringCollectionToStringList(nodeInfo.ContentRelatedPluginIds);
        //    if (!string.IsNullOrEmpty(nodeInfo.ContentModelPluginId))
        //    {
        //        pluginIds.Add(nodeInfo.ContentModelPluginId);
        //    }

        //    foreach (var service in PluginManager.Services)
        //    {
        //        if (!pluginIds.Contains(service.PluginId) || service.ContentFormCustomized == null || service.ContentFormCustomized.Count == 0) continue;

        //        dict[service.PluginId] = service.ContentFormCustomized;
        //    }

        //    return dict;
        //}
    }
}
