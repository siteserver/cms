using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Core.Common;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class SiteRepository : ISiteRepository
    {
        private readonly IDistributedCache _cache;
        private readonly Repository<Site> _repository;
        private readonly ISettingsManager _settingsManager;

        public SiteRepository(IDistributedCache cache, ISettingsManager settingsManager)
        {
            _cache = cache;
            _repository = new Repository<Site>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Site siteInfo)
        {
            siteInfo.Taxis = await GetMaxTaxisAsync() + 1;
            await _repository.InsertAsync(siteInfo);

            await RemoveCacheListAsync();

            return siteInfo.Id;
        }

        public async Task<bool> DeleteAsync(int siteId)
        {
            var children = await GetCacheListAsync(siteId);
            if (children.Count > 0) return false;

            var siteInfo = await GetSiteInfoAsync(siteId);
            // var list = ChannelManager.GetChannelIdList(siteId);
            // DataProvider.TableStyleRepository.Delete(list, siteInfo.TableName);

            // DataProvider.TagRepository.DeleteTags(siteId);

            // DataProvider.ChannelRepository.DeleteAll(siteId);

            await _repository.DeleteAsync(siteId);

            await RemoveCacheListAsync();
            await RemoveCacheEntityAsync(siteId);

            return true;
        }

        public async Task<bool> UpdateAsync(Site siteInfo)
        {
            if (siteInfo.IsRoot)
            {
                await _repository.UpdateAsync(Q
                    .Set(Attr.IsRoot, false.ToString())
                );
            }

            var updated = await _repository.UpdateAsync(siteInfo);

            await RemoveCacheListAsync();
            await RemoveCacheEntityAsync(siteInfo.Id);

            return updated;
        }

        public async Task UpdateTableNameAsync(int siteId, string tableName)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.TableName, tableName)
                .Where(Attr.Id, siteId)
            );

            await RemoveCacheListAsync();
            await RemoveCacheEntityAsync(siteId);
        }

        public async Task<List<string>> GetSiteDirListAsync(int parentId)
        {
            var children = await GetCacheListAsync(parentId);
            if (parentId == 0)
            {
                var rootSite = children.Where(x => x.IsRoot).FirstOrDefault();
                if (rootSite != null)
                {
                    children.AddRange(await GetCacheListAsync(rootSite.Id));
                }
            }

            return children.Select(x => x.SiteDir).ToList();
        }

        public async Task<List<KeyValuePair<int, Site>>> GetContainerSiteListAsync(string siteName, string siteDir, int startNum, int totalNum, ScopeType scopeType, string orderByString)
        {
            var query = Q.NewQuery();

            Site siteInfo = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                siteInfo = await GetSiteInfoBySiteNameAsync(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                siteInfo = await GetSiteInfoBySiteDirAsync(siteDir);
            }

            if (siteInfo != null)
            {
                query.Where(Attr.ParentId, siteInfo.Id);
            }
            else
            {
                if (scopeType == ScopeType.Children)
                {
                    query.Where(Attr.ParentId, 0).Where(Attr.IsRoot, false);
                }
                else if (scopeType == ScopeType.Descendant)
                {
                    query.Where(Attr.IsRoot, false);
                }
            }

            query.OrderByDesc(Attr.IsRoot).OrderBy(Attr.ParentId).OrderByDesc(Attr.Taxis).OrderBy(Attr.Id);

            query.Offset(startNum - 1).Limit(totalNum);

            var list = new List<KeyValuePair<int, Site>>();
            var itemIndex = 0;
            var minSiteInfoList = await _repository.GetAllAsync(query);

            foreach (var minSiteInfo in minSiteInfoList)
            {
                list.Add(new KeyValuePair<int, Site>(itemIndex++, minSiteInfo));
            }

            return list;
        }

        public async Task<int> GetTableCountAsync(string tableName)
        {
            var cacheList = await GetCacheListAsync();
            return cacheList.Where(x => x.TableName == tableName).Count();
        }

        public async Task<IEnumerable<int>> GetSiteIdListAsync()
        {
            var cacheList = await GetCacheListAsync();
            return cacheList.Select(x => x.Id).ToList();
        }

        public async Task<IList<Site>> GetSiteInfoListAsync()
        {
            var list = new List<Site>();
            var siteIdList = await GetSiteIdListAsync();
            foreach (var siteId in siteIdList)
            {
                var siteInfo = await GetCacheEntityAsync(siteId);
                if (siteInfo != null)
                {
                    list.Add(siteInfo);
                }
            }
            return list;
        }

        public async Task<IList<Site>> GetSiteInfoListAsync(int parentId)
        {
            var list = new List<Site>();

            var cacheList = await GetCacheListAsync(parentId);
            foreach (var cache in cacheList)
            {
                var siteInfo = await GetCacheEntityAsync(cache.Id);
                if (siteInfo != null)
                {
                    siteInfo.Children = await GetSiteInfoListAsync(siteInfo.Id);
                    list.Add(siteInfo);
                }
            }

            return list;
        }

        public async Task<Site> GetSiteInfoAsync(int siteId)
        {
            if (siteId <= 0) return null;

            return await GetCacheEntityAsync(siteId);
        }

        public async Task<Site> GetSiteInfoBySiteNameAsync(string siteName)
        {
            var cacheList = await GetCacheListAsync();
            var siteId = cacheList.Where(x => x.SiteName == siteName).Select(x => x.Id).FirstOrDefault();
            if (siteId == 0) return null;

            return await GetCacheEntityAsync(siteId);
        }

        public async Task<Site> GetSiteInfoByIsRootAsync()
        {
            var cacheList = await GetCacheListAsync();
            var siteId = cacheList.Where(x => x.IsRoot).Select(x => x.Id).FirstOrDefault();
            if (siteId == 0) return null;

            return await GetCacheEntityAsync(siteId);
        }

        public async Task<int> GetSiteIdByIsRootAsync()
        {
            var cacheList = await GetCacheListAsync();
            return cacheList.Where(x => x.IsRoot).Select(x => x.Id).FirstOrDefault();
        }

        public async Task<Site> GetSiteInfoBySiteDirAsync(string siteDir)
        {
            var cacheList = await GetCacheListAsync();
            var siteId = cacheList.Where(x => StringUtils.EqualsIgnoreCase(x.SiteDir, siteDir)).Select(x => x.Id).FirstOrDefault();
            if (siteId == 0) return null;

            return await GetCacheEntityAsync(siteId);
        }

        public async Task<int> GetSiteIdBySiteDirAsync(string siteDir)
        {
            var cacheList = await GetCacheListAsync();
            return cacheList.Where(x => StringUtils.EqualsIgnoreCase(x.SiteDir, siteDir)).Select(x => x.Id).FirstOrDefault();
        }

        public async Task<List<int>> GetSiteIdListOrderByLevelAsync()
        {
            var retval = new List<int>();

            var siteIdList = await GetSiteIdListAsync();
            var siteInfoList = new List<Site>();
            var parentWithChildren = new Dictionary<int, List<Site>>();
            var rootSiteId = 0;
            foreach (var siteId in siteIdList)
            {
                var siteInfo = await GetSiteInfoAsync(siteId);
                if (siteInfo.IsRoot)
                {
                    rootSiteId = siteInfo.Id;
                }
                else
                {
                    if (siteInfo.ParentId == 0)
                    {
                        siteInfoList.Add(siteInfo);
                    }
                    else
                    {
                        if (!parentWithChildren.TryGetValue(siteInfo.ParentId, out var children))
                        {
                            children = new List<Site>();
                        }
                        children.Add(siteInfo);
                        parentWithChildren[siteInfo.ParentId] = children;
                    }
                }
            }

            if (rootSiteId > 0)
            {
                retval.Add(rootSiteId);
            }

            var list = siteInfoList.OrderBy(siteInfo => siteInfo.Taxis == 0 ? int.MaxValue : siteInfo.Taxis).ToList();

            foreach (var siteInfo in list)
            {
                AddSiteIdList(retval, siteInfo, parentWithChildren, 0);
            }
            return retval;
        }

        public async Task GetAllParentSiteIdListAsync(List<int> parentSiteIds, List<int> siteIdCollection, int siteId)
        {
            var siteInfo = await GetSiteInfoAsync(siteId);
            var parentSiteId = -1;
            foreach (var psId in siteIdCollection)
            {
                if (psId != siteInfo.ParentId) continue;
                parentSiteId = psId;
                break;
            }
            if (parentSiteId == -1) return;

            parentSiteIds.Add(parentSiteId);
            await GetAllParentSiteIdListAsync(parentSiteIds, siteIdCollection, parentSiteId);
        }

        public async Task<bool> IsExistsAsync(int siteId)
        {
            if (siteId == 0) return false;
            return await GetSiteInfoAsync(siteId) != null;
        }

        public async Task<List<string>> GetSiteTableNamesAsync(IPluginManager pluginManager)
        {
            return await GetTableNameListAsync(pluginManager, true, false);
        }

        public async Task<List<string>> GetAllTableNameListAsync(IPluginManager pluginManager)
        {
            return await GetTableNameListAsync(pluginManager, true, true);
        }

        public async Task<List<string>> GetTableNameListAsync(IPluginManager pluginManager, Site siteInfo)
        {
            var tableNames = new List<string> { siteInfo.TableName };
            var pluginTableNames = await pluginManager.GetContentTableNameListAsync();
            foreach (var pluginTableName in pluginTableNames)
            {
                if (!StringUtils.ContainsIgnoreCase(tableNames, pluginTableName))
                {
                    tableNames.Add(pluginTableName);
                }
            }
            return tableNames;
        }

        //public ETableStyle GetTableStyle(SiteInfo siteInfo, string tableName)
        //{
        //    var tableStyle = ETableStyle.Custom;

        //    if (StringUtils.EqualsIgnoreCase(tableName, siteInfo.AuxiliaryTableForContent))
        //    {
        //        tableStyle = ETableStyle.BackgroundContent;
        //    }
        //    else if (StringUtils.EqualsIgnoreCase(tableName, DataProvider.SiteDao.TableName))
        //    {
        //        tableStyle = ETableStyle.Site;
        //    }
        //    else if (StringUtils.EqualsIgnoreCase(tableName, DataProvider.ChannelDao.TableName))
        //    {
        //        tableStyle = ETableStyle.Channel;
        //    }
        //    //else if (StringUtils.EqualsIgnoreCase(tableName, DataProvider.InputContentDao.TableName))
        //    //{
        //    //    tableStyle = ETableStyle.InputContent;
        //    //}
        //    return tableStyle;
        //}

        public async Task<int> GetSiteLevelAsync(int siteId)
        {
            var level = 0;
            var siteInfo = await GetSiteInfoAsync(siteId);
            if (siteInfo.ParentId != 0)
            {
                level++;
                level += await GetSiteLevelAsync(siteInfo.ParentId);
            }
            return level;
        }

        public async Task<int> GetParentSiteIdAsync(int siteId)
        {
            var parentSiteId = 0;
            var siteInfo = await GetSiteInfoAsync(siteId);
            if (siteInfo != null && siteInfo.IsRoot == false)
            {
                parentSiteId = siteInfo.ParentId;
                if (parentSiteId == 0)
                {
                    parentSiteId = await GetSiteIdByIsRootAsync();
                }
            }
            return parentSiteId;
        }



        //public List<int> GetWritingSiteIdList(PermissionsImpl permissionsImpl)
        //{
        //    var siteIdList = new List<int>();

        //    if (!string.IsNullOrEmpty(permissionsImpl.UserName))
        //    {
        //        if (permissionsImpl.IsConsoleAdministrator || permissionsImpl.IsSystemAdministrator)//如果是超级管理员或站点管理员
        //        {
        //            foreach (var siteId in permissionsImpl.SiteIdList)
        //            {
        //                if (!siteIdList.Contains(siteId))
        //                {
        //                    siteIdList.Add(siteId);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var siteId in permissionsImpl.SiteIdList)
        //            {
        //                if (!siteIdList.Contains(siteId))
        //                {
        //                    var channelIdCollection = DataProvider.SitePermissionsDao.GetAllPermissionList(permissionsImpl.Roles, siteId, true);
        //                    if (channelIdCollection.Count > 0)
        //                    {
        //                        siteIdList.Add(siteId);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return siteIdList;
        //}

        public async Task<string> GetSiteNameAsync(Site siteInfo)
        {
            var padding = string.Empty;

            var level = await GetSiteLevelAsync(siteInfo.Id);
            string psLogo;
            if (siteInfo.IsRoot)
            {
                psLogo = "siteHQ.gif";
            }
            else
            {
                psLogo = "site.gif";
                if (level > 0 && level < 10)
                {
                    psLogo = $"subsite{level + 1}.gif";
                }
            }
            psLogo = SiteServerAssets.GetIconUrl("tree/" + psLogo);

            for (var i = 0; i < level; i++)
            {
                padding += "　";
            }
            if (level > 0)
            {
                padding += "└ ";
            }

            return $"{padding}<img align='absbottom' border='0' src='{psLogo}'/>&nbsp;{siteInfo.SiteName}";
        }
    }
}