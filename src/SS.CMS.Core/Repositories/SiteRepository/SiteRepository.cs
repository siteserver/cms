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
        private readonly string _cacheKey;
        private readonly Repository<SiteInfo> _repository;
        private readonly ISettingsManager _settingsManager;

        public SiteRepository(IDistributedCache cache, ISettingsManager settingsManager)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(SiteRepository));
            _repository = new Repository<SiteInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(SiteInfo siteInfo)
        {
            siteInfo.Taxis = await GetMaxTaxisAsync() + 1;
            siteInfo.Id = await _repository.InsertAsync(siteInfo);

            await _cache.RemoveAsync(_cacheKey);

            return siteInfo.Id;
        }

        public async Task<bool> DeleteAsync(int siteId)
        {
            var siteInfo = await GetSiteInfoAsync(siteId);
            // var list = ChannelManager.GetChannelIdList(siteId);
            // DataProvider.TableStyleRepository.Delete(list, siteInfo.TableName);

            // DataProvider.TagRepository.DeleteTags(siteId);

            // DataProvider.ChannelRepository.DeleteAll(siteId);

            await UpdateParentIdToZeroAsync(siteId);

            await _repository.DeleteAsync(siteId);

            await _cache.RemoveAsync(_cacheKey);
            // ChannelManager.RemoveCacheBySiteId(siteId);
            // Permissions.ClearAllCache();

            return true;
        }

        public async Task<bool> UpdateAsync(SiteInfo siteInfo)
        {
            if (siteInfo.IsRoot)
            {
                await UpdateAllIsRootAsync();
            }

            var updated = await _repository.UpdateAsync(siteInfo);

            if (updated)
            {
                await _cache.RemoveAsync(_cacheKey);
            }

            return updated;
        }

        public async Task UpdateTableNameAsync(int siteId, string tableName)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.TableName, tableName)
                .Where(Attr.Id, siteId)
            );

            await _cache.RemoveAsync(_cacheKey);
        }

        public async Task UpdateParentIdToZeroAsync(int parentId)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.ParentId, 0)
                .Where(Attr.ParentId, parentId)
            );

            await _cache.RemoveAsync(_cacheKey);
        }

        public async Task<IEnumerable<string>> GetLowerSiteDirListThatNotIsRootAsync()
        {
            var list = await _repository.GetAllAsync<string>(Q
                .Select(Attr.SiteDir)
                .WhereNot(Attr.IsRoot, true.ToString()));

            return list.Select(x => x.ToLower());
        }

        public async Task<IEnumerable<string>> GetLowerSiteDirListAsync(int parentId)
        {
            var list = await _repository.GetAllAsync<string>(Q
                    .Select(Attr.SiteDir)
                    .Where(Attr.ParentId, parentId));

            return list.Select(x => x.ToLower());
        }

        public async Task<List<KeyValuePair<int, SiteInfo>>> GetContainerSiteListAsync(string siteName, string siteDir, int startNum, int totalNum, ScopeType scopeType, string orderByString)
        {
            var query = Q.NewQuery();

            SiteInfo siteInfo = null;
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

            var list = new List<KeyValuePair<int, SiteInfo>>();
            var itemIndex = 0;
            var minSiteInfoList = await _repository.GetAllAsync(query);

            foreach (var minSiteInfo in minSiteInfoList)
            {
                list.Add(new KeyValuePair<int, SiteInfo>(itemIndex++, minSiteInfo));
            }

            return list;
        }

        public async Task<int> GetTableCountAsync(string tableName)
        {
            return await _repository.CountAsync(Q.Where(Attr.TableName, tableName));
        }

        public async Task<IEnumerable<int>> GetSiteIdListAsync()
        {
            var cacheInfoList = await GetListCacheAsync();
            return cacheInfoList.Select(x => x.Id).ToList();
        }

        public async Task<List<SiteInfo>> GetSiteInfoListAsync()
        {
            var list = new List<SiteInfo>();
            var siteIdList = await GetSiteIdListAsync();
            foreach (var siteId in siteIdList)
            {
                var siteInfo = await GetEntityCacheAsync(siteId);
                if (siteInfo != null)
                {
                    list.Add(siteInfo);
                }
            }
            return list;
        }

        public async Task<SiteInfo> GetSiteInfoAsync(int siteId)
        {
            if (siteId <= 0) return null;

            return await GetEntityCacheAsync(siteId);
        }

        public async Task<SiteInfo> GetSiteInfoBySiteNameAsync(string siteName)
        {
            var cacheInfoList = await GetListCacheAsync();
            var siteId = cacheInfoList.Where(x => x.SiteName == siteName).Select(x => x.Id).FirstOrDefault();
            if (siteId == 0) return null;

            return await GetEntityCacheAsync(siteId);
        }

        public async Task<SiteInfo> GetSiteInfoByIsRootAsync()
        {
            var cacheInfoList = await GetListCacheAsync();
            var siteId = cacheInfoList.Where(x => x.IsRoot).Select(x => x.Id).FirstOrDefault();
            if (siteId == 0) return null;

            return await GetEntityCacheAsync(siteId);
        }

        public async Task<int> GetSiteIdByIsRootAsync()
        {
            var cacheInfoList = await GetListCacheAsync();
            return cacheInfoList.Where(x => x.IsRoot).Select(x => x.Id).FirstOrDefault();
        }

        public async Task<SiteInfo> GetSiteInfoBySiteDirAsync(string siteDir)
        {
            var cacheInfoList = await GetListCacheAsync();
            var siteId = cacheInfoList.Where(x => StringUtils.EqualsIgnoreCase(x.SiteDir, siteDir)).Select(x => x.Id).FirstOrDefault();
            if (siteId == 0) return null;

            return await GetEntityCacheAsync(siteId);
        }

        public async Task<int> GetSiteIdBySiteDirAsync(string siteDir)
        {
            var cacheInfoList = await GetListCacheAsync();
            return cacheInfoList.Where(x => StringUtils.EqualsIgnoreCase(x.SiteDir, siteDir)).Select(x => x.Id).FirstOrDefault();
        }

        public async Task<List<int>> GetSiteIdListOrderByLevelAsync()
        {
            var retval = new List<int>();

            var siteIdList = await GetSiteIdListAsync();
            var siteInfoList = new List<SiteInfo>();
            var parentWithChildren = new Dictionary<int, List<SiteInfo>>();
            var hqSiteId = 0;
            foreach (var siteId in siteIdList)
            {
                var siteInfo = await GetSiteInfoAsync(siteId);
                if (siteInfo.IsRoot)
                {
                    hqSiteId = siteInfo.Id;
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
                            children = new List<SiteInfo>();
                        }
                        children.Add(siteInfo);
                        parentWithChildren[siteInfo.ParentId] = children;
                    }
                }
            }

            if (hqSiteId > 0)
            {
                retval.Add(hqSiteId);
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

        public async Task<List<string>> GetTableNameListAsync(IPluginManager pluginManager, SiteInfo siteInfo)
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

        public async Task<string> GetSiteNameAsync(SiteInfo siteInfo)
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