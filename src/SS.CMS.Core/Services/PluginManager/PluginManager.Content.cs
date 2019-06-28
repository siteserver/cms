using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Models;
using SS.CMS.Core.Plugin;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class PluginManager
    {
        public bool IsContentTable(IService service)
        {
            return !string.IsNullOrEmpty(service.ContentTableName) &&
                    service.ContentTableColumns != null && service.ContentTableColumns.Count > 0;
        }

        public async Task<string> GetContentTableNameAsync(string pluginId)
        {
            foreach (var service in await GetServicesAsync())
            {
                if (service.PluginId == pluginId && IsContentTable(service))
                {
                    return service.ContentTableName;
                }
            }

            return string.Empty;
        }

        public async Task<List<IPackageMetadata>> GetContentModelPluginsAsync()
        {
            var list = new List<IPackageMetadata>();

            foreach (var service in await GetServicesAsync())
            {
                if (IsContentTable(service))
                {
                    list.Add(service.Metadata);
                }
            }

            return list;
        }

        public async Task<List<string>> GetContentTableNameListAsync()
        {
            var list = new List<string>();

            foreach (var service in await GetServicesAsync())
            {
                if (IsContentTable(service))
                {
                    if (!StringUtils.ContainsIgnoreCase(list, service.ContentTableName))
                    {
                        list.Add(service.ContentTableName);
                    }
                }
            }

            return list;
        }

        public async Task<List<IPackageMetadata>> GetAllContentRelatedPluginsAsync(bool includeContentTable)
        {
            var list = new List<IPackageMetadata>();

            foreach (var service in await GetServicesAsync())
            {
                var isContentModel = IsContentTable(service);

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

        public async Task<List<IService>> GetContentPluginsAsync(ChannelInfo channelInfo, bool includeContentTable)
        {
            var list = new List<IService>();
            var pluginIds = TranslateUtils.StringCollectionToStringList(channelInfo.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(channelInfo.ContentModelPluginId))
            {
                pluginIds.Add(channelInfo.ContentModelPluginId);
            }

            foreach (var service in await GetServicesAsync())
            {
                if (!pluginIds.Contains(service.PluginId)) continue;

                if (!includeContentTable && IsContentTable(service)) continue;

                list.Add(service);
            }

            return list;
        }

        public List<string> GetContentPluginIds(ChannelInfo channelInfo)
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

        public async Task<Dictionary<string, Dictionary<string, Func<IContentContext, string>>>> GetContentColumnsAsync(List<string> pluginIds)
        {
            var dict = new Dictionary<string, Dictionary<string, Func<IContentContext, string>>>();
            if (pluginIds == null || pluginIds.Count == 0) return dict;

            foreach (var service in await GetServicesAsync())
            {
                if (!pluginIds.Contains(service.PluginId) || service.ContentColumns == null || service.ContentColumns.Count == 0) continue;

                dict[service.PluginId] = service.ContentColumns;
            }

            return dict;
        }
    }
}
