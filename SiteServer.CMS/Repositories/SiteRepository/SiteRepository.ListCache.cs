using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Datory.Caching;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Plugin;

namespace SiteServer.CMS.Repositories
{
    public partial class SiteRepository
    {
        private const string CacheTypeAll = "all";
        private const string CacheTypeOptions = "options";

        private async Task<List<Cache>> GetCacheListAsync()
        {
            var cacheKey = CacheManager.GetListKey(TableName, CacheTypeAll);
            return await _repository.Cache.GetOrCreateAsync(cacheKey, async () =>
            {
                var sites = await _repository.GetAllAsync(Q
                    .Select(nameof(Site.Id), nameof(Site.SiteName), nameof(Site.SiteDir), nameof(Site.TableName), nameof(Site.IsRoot), nameof(Site.ParentId), nameof(Site.Taxis))
                    .OrderBy(nameof(Site.Taxis), nameof(Site.Id))
                );

                var cacheList = new List<Cache>();
                Cache rootCache = null;
                foreach (var site in sites)
                {
                    var cache = new Cache
                    {
                        Id = site.Id,
                        SiteName = site.SiteName,
                        SiteDir = site.SiteDir,
                        TableName = site.TableName,
                        Root = site.Root,
                        ParentId = site.ParentId,
                        Taxis = site.Taxis
                    };

                    if (cache.Root)
                    {
                        rootCache = cache;
                    }
                    else
                    {
                        cacheList.Add(cache);
                    }
                }
                if (rootCache != null)
                {
                    cacheList.Insert(0, rootCache);
                }

                return cacheList;
            });
        }

        public async Task<List<Cascade<int>>> GetSiteOptionsAsync(int parentId)
        {
            var cacheKey = CacheManager.GetListKey(TableName, CacheTypeOptions);
            return await _repository.Cache.GetOrCreateAsync(cacheKey, async () =>
            {
                var optionList = new List<Cascade<int>>();
                var siteList = await GetCacheListAsync(parentId);

                foreach (var site in siteList)
                {
                    optionList.Add(new Cascade<int>(site.Id, site.SiteName, await GetSiteOptionsAsync(site.Id)));
                }

                return optionList;
            });
        }

        [Serializable]
        private class Cache
        {
            public int Id { get; set; }
            public string SiteName { get; set; }
            public string SiteDir { get; set; }
            public string TableName { get; set; }
            public bool Root { get; set; }
            public int ParentId { get; set; }
            public int Taxis { get; set; }
        }

        private async Task RemoveCacheListAsync()
        {
            await _repository.Cache.RemoveAsync(CacheManager.GetListKey(TableName, CacheTypeAll));
            await _repository.Cache.RemoveAsync(CacheManager.GetListKey(TableName, CacheTypeOptions));
        }

        private async Task<List<Cache>> GetCacheListAsync(int parentId)
        {
            var cacheList = await GetCacheListAsync();
            return cacheList.Where(x => x.ParentId == parentId).ToList();
        }

        public async Task<List<Site>> GetSiteListAsync()
        {
            var cacheList = await GetCacheListAsync();
            var sites = new List<Site>();
            foreach (var cache in cacheList)
            {
                sites.Add(await GetAsync(cache.Id));
            }
            return sites;
        }

        public async Task<List<int>> GetSiteIdListAsync()
        {
            var cacheList = await GetCacheListAsync();
            return cacheList.Select(x => x.Id).ToList();
        }

        public async Task<List<int>> GetSiteIdListOrderByLevelAsync()
        {
            var retVal = new List<int>();

            var siteIdList = await GetSiteIdListAsync();
            var siteList = new List<Site>();
            var parentWithChildren = new Hashtable();
            var hqSiteId = 0;
            foreach (var siteId in siteIdList)
            {
                var site = await GetAsync(siteId);
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

        public async Task GetAllParentSiteIdListAsync(List<int> parentSiteIds, List<int> siteIdCollection, int siteId)
        {
            var site = await GetAsync(siteId);
            var parentSiteId = -1;
            foreach (var psId in siteIdCollection)
            {
                if (psId != site.ParentId) continue;
                parentSiteId = psId;
                break;
            }
            if (parentSiteId == -1) return;

            parentSiteIds.Add(parentSiteId);
            await GetAllParentSiteIdListAsync(parentSiteIds, siteIdCollection, parentSiteId);
        }

        public async Task<List<int>> GetSiteIdListAsync(int parentId)
        {
            var list = await GetCacheListAsync();
            var siteIdList = new List<int>();
            foreach (var cache in list)
            {
                if (cache.ParentId == parentId)
                {
                    siteIdList.Add(cache.Id);
                }
            }
            return siteIdList;
        }

        public async Task<List<string>> GetSiteTableNamesAsync()
        {
            return await GetTableNameListAsync(true, false);
        }

        public async Task<List<string>> GetAllTableNameListAsync()
        {
            return await GetTableNameListAsync(true, true);
        }

        private async Task<List<string>> GetTableNameListAsync(bool includeSiteTables, bool includePluginTables)
        {

            var tableNames = new List<string>();

            if (includeSiteTables)
            {
                var list = await GetCacheListAsync();
                foreach (var cache in list)
                {
                    if (!StringUtils.ContainsIgnoreCase(tableNames, cache.TableName))
                    {
                        tableNames.Add(cache.TableName);
                    }
                }
            }

            if (includePluginTables)
            {
                var pluginTableNames = await PluginContentManager.GetContentTableNameListAsync();
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

        public async Task<List<string>> GetTableNameListAsync(Site site)
        {
            var tableNames = new List<string> { site.TableName };
            var pluginTableNames = await PluginContentManager.GetContentTableNameListAsync();
            foreach (var pluginTableName in pluginTableNames)
            {
                if (!StringUtils.ContainsIgnoreCase(tableNames, pluginTableName))
                {
                    tableNames.Add(pluginTableName);
                }
            }
            return tableNames;
        }

        public async Task<int> GetIdByIsRootAsync()
        {
            var list = await GetCacheListAsync();
            foreach (var cache in list)
            {
                if (cache.Root)
                {
                    return cache.Id;
                }
            }

            return 0;
        }

        private async Task<int> GetMaxTaxisAsync()
        {
            var list = await GetCacheListAsync();
            return list.Max(x => x.Taxis);
        }

        public async Task<IList<string>> GetSiteDirListAsync(int parentId)
        {
            var list = await GetCacheListAsync();
            var siteDirList = new List<string>();
            foreach (var cache in list)
            {
                if (cache.ParentId == parentId && !cache.Root)
                {
                    siteDirList.Add(cache.SiteDir);
                }
            }

            return siteDirList;
        }

        /// <summary>
        /// 得到所有系统文件夹的列表，以小写表示。
        /// </summary>
        public async Task<IEnumerable<string>> GetLowerSiteDirListAsync(int parentId)
        {
            var list = await GetCacheListAsync(parentId);
            return list.Select(x => StringUtils.ToLower(x.SiteDir));
        }

        public async Task<int> GetIdBySiteDirAsync(string siteDir)
        {
            var list = await GetCacheListAsync();
            foreach (var cache in list)
            {
                if (StringUtils.EqualsIgnoreCase(cache.SiteDir, siteDir))
                {
                    return cache.Id;
                }
            }

            return 0;
        }

        //private void AddListItem(ListControl listControl, Site site, Hashtable parentWithChildren, int level)
        //{
        //    var padding = string.Empty;
        //    for (var i = 0; i < level; i++)
        //    {
        //        padding += "　";
        //    }
        //    if (level > 0)
        //    {
        //        padding += "└ ";
        //    }

        //    if (parentWithChildren[site.Id] != null)
        //    {
        //        var children = (List<Site>)parentWithChildren[site.Id];
        //        listControl.Items.Add(new ListItem(padding + site.SiteName + $"({children.Count})", site.Id.ToString()));
        //        level++;
        //        foreach (Site subSite in children)
        //        {
        //            AddListItem(listControl, subSite, parentWithChildren, level);
        //        }
        //    }
        //    else
        //    {
        //        listControl.Items.Add(new ListItem(padding + site.SiteName, site.Id.ToString()));
        //    }
        //}
    }
}