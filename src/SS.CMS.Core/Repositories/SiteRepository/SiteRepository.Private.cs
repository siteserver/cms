using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class SiteRepository
    {
        private static class Attr
        {
            public const string Id = nameof(Site.Id);
            public const string SiteName = nameof(Site.SiteName);
            public const string SiteDir = nameof(Site.SiteDir);
            public const string TableName = nameof(Site.TableName);
            public const string IsRoot = nameof(Site.IsRoot);
            public const string ParentId = nameof(Site.ParentId);
            public const string Taxis = nameof(Site.Taxis);
        }

        private async Task<List<KeyValuePair<int, Site>>> GetSiteInfoKeyValuePairListToCacheAsync()
        {
            var list = new List<KeyValuePair<int, Site>>();

            var siteInfoList = await GetSiteInfoListToCacheAsync();
            foreach (var siteInfo in siteInfoList)
            {
                var entry = new KeyValuePair<int, Site>(siteInfo.Id, siteInfo);
                list.Add(entry);
            }

            return list;
        }

        private async Task<IEnumerable<Site>> GetSiteInfoListToCacheAsync()
        {
            return await _repository.GetAllAsync(Q.OrderBy(Attr.Taxis, Attr.Id));
        }

        private async Task<int> GetMaxTaxisAsync(int parentId)
        {
            return await _repository.MaxAsync(Attr.Taxis, Q.Where(Attr.ParentId, parentId)) ?? 0;
        }

        private void AddSiteIdList(List<int> dataSource, Site siteInfo, Dictionary<int, List<Site>> parentWithChildren, int level)
        {
            dataSource.Add(siteInfo.Id);

            if (parentWithChildren.TryGetValue(siteInfo.Id, out var children))
            {
                level++;

                var list = children.OrderBy(child => child.Taxis == 0 ? int.MaxValue : child.Taxis).ToList();

                foreach (var subSiteInfo in list)
                {
                    AddSiteIdList(dataSource, subSiteInfo, parentWithChildren, level);
                }
            }
        }

        private async Task<List<string>> GetTableNameListAsync(IPluginManager pluginManager, bool includeSiteTables, bool includePluginTables)
        {

            var tableNames = new List<string>();

            if (includeSiteTables)
            {
                var cacheInfoList = await GetCacheListAsync();
                foreach (var cacheInfo in cacheInfoList)
                {
                    if (!string.IsNullOrEmpty(cacheInfo.TableName) && !StringUtils.ContainsIgnoreCase(tableNames, cacheInfo.TableName))
                    {
                        tableNames.Add(cacheInfo.TableName);
                    }
                }
            }

            if (includePluginTables)
            {
                var pluginTableNames = await pluginManager.GetContentTableNameListAsync();
                foreach (var pluginTableName in pluginTableNames)
                {
                    if (!StringUtils.ContainsIgnoreCase(tableNames, pluginTableName))
                    {
                        tableNames.Add(pluginTableName);
                    }
                }
            }

            return tableNames;
        }
    }
}