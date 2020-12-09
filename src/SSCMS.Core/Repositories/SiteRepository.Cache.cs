using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class SiteRepository
    {
        public async Task<List<Site>> GetSitesAsync()
        {
            var sites = new List<Site>();

            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                sites.Add(await GetAsync(summary.Id));
            }
            return sites;
        }

        private async Task<IEnumerable<SiteSummary>> GetCacheListAsync(int parentId)
        {
            var summaries = await GetSummariesAsync();
            return summaries.Where(x => x.ParentId == parentId);
        }

        private async Task<SiteSummary> GetCacheAsync(int siteId)
        {
            var summaries = await GetSummariesAsync();
            return summaries.FirstOrDefault(x => x.Id == siteId);
        }

        public async Task<int> GetParentSiteIdAsync(int siteId)
        {
            var parentSiteId = 0;
            var site = await GetAsync(siteId);
            if (site != null && site.Root == false)
            {
                parentSiteId = site.ParentId;
                if (parentSiteId == 0)
                {
                    parentSiteId = await GetIdByIsRootAsync();
                }
            }
            return parentSiteId;
        }

        public async Task<string> GetTableNameAsync(int siteId)
        {
            var site = await GetAsync(siteId);
            return site?.TableName;
        }

        public async Task<Site> GetSiteBySiteNameAsync(string siteName)
        {
            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (StringUtils.EqualsIgnoreCase(summary.SiteName, siteName))
                {
                    return await GetAsync(summary.Id);
                }
            }
            return null;
        }

        public async Task<Site> GetSiteByIsRootAsync()
        {
            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (summary.Root)
                {
                    return await GetAsync(summary.Id);
                }
            }
            return null;
        }

        public async Task<bool> IsRootExistsAsync()
        {
            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (summary.Root)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<Site> GetSiteByDirectoryAsync(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                var summaries = await GetSummariesAsync();
                foreach (var summary in summaries)
                {
                    if (summary.Root)
                    {
                        return await GetAsync(summary.Id);
                    }
                }
                return null;
            }

            var dirs = ListUtils.GetStringList(directory.Trim('/'), '/');
            var parentId = 0;

            Site site = null;
            foreach (var dir in dirs)
            {
                site = await GetSiteAsync(parentId, dir);
                if (site != null)
                {
                    parentId = site.Id;
                }
                else
                {
                    return null;
                }
            }

            return site;
        }

        private async Task<Site> GetSiteAsync(int parentId, string siteDir)
        {
            var summaries = await GetSummariesAsync(parentId);
            foreach (var summary in summaries)
            {
                if (StringUtils.EqualsIgnoreCase(summary.SiteDir, siteDir))
                {
                    return await GetAsync(summary.Id);
                }
            }
            return null;
        }

        public async Task<List<Cascade<int>>> GetSiteOptionsAsync(int parentId)
        {
            var optionList = new List<Cascade<int>>();
            var cacheList = await GetCacheListAsync(parentId);

            foreach (var cache in cacheList)
            {
                optionList.Add(new Cascade<int>
                {
                    Value = cache.Id,
                    Label = cache.SiteName,
                    Children = await GetSiteOptionsAsync(cache.Id)
                });
            }

            return optionList;
        }

        public async Task<List<int>> GetSiteIdsAsync()
        {
            var summaries = await GetSummariesAsync();
            return summaries.Select(x => x.Id).ToList();
        }

        public async Task<List<int>> GetSiteIdsOrderByLevelAsync()
        {
            var retVal = new List<int>();

            var siteIdList = await GetSiteIdsAsync();
            var siteList = new List<Site>();
            var parentWithChildren = new Hashtable();
            var hqSiteId = 0;
            foreach (var siteId in siteIdList)
            {
                var site = await GetAsync(siteId);
                if (site == null) continue;
                
                if (site.Root)
                {
                    hqSiteId = site.Id;
                }
                else
                {
                    if (site.ParentId == 0)
                    {
                        siteList.Add(site);
                    }
                    else
                    {
                        var children = new List<Site>();
                        if (parentWithChildren.Contains(site.ParentId))
                        {
                            children = (List<Site>)parentWithChildren[site.ParentId];
                        }
                        children.Add(site);
                        parentWithChildren[site.ParentId] = children;
                    }
                }
            }

            if (hqSiteId > 0)
            {
                retVal.Add(hqSiteId);
            }

            var list = siteList.OrderBy(site => site.Taxis == 0 ? int.MaxValue : site.Taxis).ToList();

            foreach (var site in list)
            {
                AddSiteIdList(retVal, site, parentWithChildren, 0);
            }
            return retVal;
        }

        private void AddSiteIdList(List<int> dataSource, Site site, Hashtable parentWithChildren, int level)
        {
            dataSource.Add(site.Id);

            if (parentWithChildren[site.Id] != null)
            {
                var children = (List<Site>)parentWithChildren[site.Id];
                level++;

                var list = children.OrderBy(child => child.Taxis == 0 ? int.MaxValue : child.Taxis).ToList();

                foreach (var subSite in list)
                {
                    AddSiteIdList(dataSource, subSite, parentWithChildren, level);
                }
            }
        }

        public async Task<List<int>> GetSiteIdsAsync(int parentId)
        {
            var siteIdList = new List<int>();

            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (summary.ParentId == parentId)
                {
                    siteIdList.Add(summary.Id);
                }
            }
            return siteIdList;
        }

        public async Task<List<string>> GetSiteTableNamesAsync()
        {
            return await GetTableNamesAsync(true, false);
        }

        public async Task<List<string>> GetAllTableNamesAsync()
        {
            return await GetTableNamesAsync(true, true);
        }

        private async Task<List<string>> GetTableNamesAsync(bool includeSiteTables, bool includePluginTables)
        {
            var tableNames = new List<string>();

            if (includeSiteTables)
            {
                var summaries = await GetSummariesAsync();
                foreach (var summary in summaries)
                {
                    if (!string.IsNullOrEmpty(summary.TableName) && !ListUtils.ContainsIgnoreCase(tableNames, summary.TableName))
                    {
                        tableNames.Add(summary.TableName);
                    }
                }
            }

            if (includePluginTables)
            {
                var pluginTableNames = _settingsManager.GetContentTableNames();
                foreach (var pluginTableName in pluginTableNames)
                {
                    if (!string.IsNullOrEmpty(pluginTableName) && !ListUtils.ContainsIgnoreCase(tableNames, pluginTableName))
                    {
                        tableNames.Add(pluginTableName);
                    }
                }
            }

            return tableNames;
        }

        public async Task<List<string>> GetTableNamesAsync(Site site)
        {
            var tableNames = new List<string> { site.TableName };
            var channelSummaries = await _channelRepository.GetSummariesAsync(site.Id);
            var pluginTableNames = _settingsManager.GetContentTableNames();
            foreach (var summary in channelSummaries)
            {
                if (!string.IsNullOrEmpty(summary.TableName))
                {
                    if (ListUtils.Contains(pluginTableNames, summary.TableName))
                    {
                        tableNames.Add(summary.TableName);
                    }
                }
            }
            return tableNames;
        }

        public async Task<int> GetIdByIsRootAsync()
        {
            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (summary.Root)
                {
                    return summary.Id;
                }
            }

            return 0;
        }

        private async Task<int> GetMaxTaxisAsync()
        {
            var summaries = await GetSummariesAsync();
            return summaries.Select(x => x.Taxis).DefaultIfEmpty().Max();
        }

        public async Task<IList<string>> GetSiteDirsAsync(int parentId)
        {
            var siteDirList = new List<string>();

            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (summary.ParentId == parentId && !summary.Root)
                {
                    siteDirList.Add(summary.SiteDir);
                }
            }

            return siteDirList;
        }

        public async Task<List<Select<int>>> GetSelectsAsync(List<int> includedSiteIds = null)
        {
            var selects = new List<Select<int>>();

            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (includedSiteIds != null && !includedSiteIds.Contains(summary.Id))
                {
                    continue;
                }
                selects.Add(new Select<int>(summary.Id, summary.SiteName));
            }

            return selects;
        }

        private async Task<Site> GetWithChildrenAsync(int siteId, Func<Site, Task<object>> func = null)
        {
            var entity = await GetAsync(siteId);
            if (entity == null) return null;

            object extra = null;
            if (func != null)
            {
                extra = await func(entity);
            }

            var site = entity.Clone<Site>();
            site.Children = await GetSitesWithChildrenAsync(siteId, func);

            if (extra == null) return site;

            var dict = TranslateUtils.ToDictionary(extra);
            foreach (var o in dict)
            {
                site.Set(o.Key, o.Value);
            }

            return site;
        }

        public async Task<List<Site>> GetSitesWithChildrenAsync(int parentId, Func<Site, Task<object>> func = null)
        {
            var list = new List<Site>();

            var summaries = await GetSummariesAsync(parentId);
            foreach (var summary in summaries)
            {
                if (summary == null || summary.Id == 0) continue;

                var site = await GetWithChildrenAsync(summary.Id, func);
                if (site != null)
                {
                    list.Add(site);
                }
            }

            return list;
        }

        private async Task<Cascade<int>> GetCascadeAsync(SiteSummary summary, Func<SiteSummary, object> func = null)
        {
            object extra = null;
            if (func != null)
            {
                extra = func(summary);
            }
            var cascade = new Cascade<int>
            {
                Value = summary.Id,
                Label = summary.SiteName,
                Children = await GetCascadeChildrenAsync(summary.Id, func)
            };
            if (extra == null) return cascade;

            var dict = TranslateUtils.ToDictionary(extra);
            foreach (var o in dict)
            {
                cascade[o.Key] = o.Value;
            }

            return cascade;
        }

        private async Task<Cascade<int>> GetCascadeAsync(SiteSummary summary, Func<SiteSummary, Task<object>> func = null)
        {
            object extra = null;
            if (func != null)
            {
                extra = await func(summary);
            }
            var cascade = new Cascade<int>
            {
                Value = summary.Id,
                Label = summary.SiteName,
                Children = await GetCascadeChildrenAsync(summary.Id, func)
            };
            if (extra == null) return cascade;

            var dict = TranslateUtils.ToDictionary(extra);
            foreach (var o in dict)
            {
                cascade[o.Key] = o.Value;
            }

            return cascade;
        }

        public async Task<List<Cascade<int>>> GetCascadeChildrenAsync(int parentId,
            Func<SiteSummary, object> func)
        {
            var list = new List<Cascade<int>>();

            var summaries = await GetSummariesAsync(parentId);
            foreach (var summary in summaries)
            {
                if (summary == null || summary.Id == 0) continue;
                var site = await GetCascadeAsync(summary, func);
                if (site != null)
                {
                    list.Add(site);
                }
            }

            return list;
        }

        public async Task<List<Cascade<int>>> GetCascadeChildrenAsync(int parentId, Func<SiteSummary, Task<object>> func = null)
        {
            var list = new List<Cascade<int>>();

            var summaries = await GetSummariesAsync(parentId);
            foreach (var summary in summaries)
            {
                if (summary == null || summary.Id == 0) continue;
                var site = await GetCascadeAsync(summary, func);
                if (site != null)
                {
                    list.Add(site);
                }
            }

            return list;
        }
    }
}