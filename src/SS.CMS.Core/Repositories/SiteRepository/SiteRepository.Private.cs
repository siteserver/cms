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
            public const string Id = nameof(SiteInfo.Id);
            public const string SiteName = nameof(SiteInfo.SiteName);
            public const string SiteDir = nameof(SiteInfo.SiteDir);
            public const string TableName = nameof(SiteInfo.TableName);
            public const string IsRoot = nameof(SiteInfo.IsRoot);
            public const string ParentId = nameof(SiteInfo.ParentId);
            public const string Taxis = nameof(SiteInfo.Taxis);
        }

        private async Task UpdateAllIsRootAsync()
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.IsRoot, false.ToString())
            );

            await _cache.RemoveAsync(_cacheKey);
        }

        private async Task<List<KeyValuePair<int, SiteInfo>>> GetSiteInfoKeyValuePairListToCacheAsync()
        {
            var list = new List<KeyValuePair<int, SiteInfo>>();

            var siteInfoList = await GetSiteInfoListToCacheAsync();
            foreach (var siteInfo in siteInfoList)
            {
                var entry = new KeyValuePair<int, SiteInfo>(siteInfo.Id, siteInfo);
                list.Add(entry);
            }

            return list;
        }

        private async Task<IEnumerable<SiteInfo>> GetSiteInfoListToCacheAsync()
        {
            return await _repository.GetAllAsync(Q.OrderBy(Attr.Taxis, Attr.Id));
        }

        private async Task<int> GetMaxTaxisAsync()
        {
            return await _repository.MaxAsync(Attr.Taxis) ?? 0;
        }

        private void AddSiteIdList(List<int> dataSource, SiteInfo siteInfo, Dictionary<int, List<SiteInfo>> parentWithChildren, int level)
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
                var cacheInfoList = await GetListCacheAsync();
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