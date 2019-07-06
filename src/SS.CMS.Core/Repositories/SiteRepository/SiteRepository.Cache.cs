using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Models;
using SS.CMS.Services;
using SS.CMS.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace SS.CMS.Core.Repositories
{
    public partial class SiteRepository
    {
        private async Task<List<KeyValuePair<int, SiteInfo>>> GetSiteInfoKeyValuePairListAsync()
        {
            return await _cache.GetOrCreateAsync<List<KeyValuePair<int, SiteInfo>>>(_cacheKey, async options =>
            {
                var list = await GetSiteInfoKeyValuePairListToCacheAsync();
                var retval = new List<KeyValuePair<int, SiteInfo>>();
                foreach (var pair in list)
                {
                    var siteInfo = pair.Value;
                    if (siteInfo == null) continue;

                    siteInfo.SiteDir = GetSiteDir(list, siteInfo);
                    retval.Add(pair);
                }
                return retval;
            });
        }

        public async Task<List<SiteInfo>> GetSiteInfoListAsync()
        {
            var pairList = await GetSiteInfoKeyValuePairListAsync();
            return pairList.Select(pair => pair.Value).ToList();
        }

        public async Task<SiteInfo> GetSiteInfoAsync(int siteId)
        {
            if (siteId <= 0) return null;

            var list = await GetSiteInfoKeyValuePairListAsync();

            foreach (var pair in list)
            {
                var theSiteId = pair.Key;
                if (theSiteId != siteId) continue;
                var siteInfo = pair.Value;
                return siteInfo;
            }
            return null;
        }

        public async Task<SiteInfo> GetSiteInfoBySiteNameAsync(string siteName)
        {
            var list = await GetSiteInfoKeyValuePairListAsync();

            foreach (var pair in list)
            {
                var siteInfo = pair.Value;
                if (siteInfo == null) continue;

                if (StringUtils.EqualsIgnoreCase(siteInfo.SiteName, siteName))
                {
                    return siteInfo;
                }
            }
            return null;
        }

        public async Task<SiteInfo> GetSiteInfoByIsRootAsync()
        {
            var list = await GetSiteInfoKeyValuePairListAsync();

            foreach (var pair in list)
            {
                var siteInfo = pair.Value;
                if (siteInfo == null) continue;

                if (siteInfo.IsRoot)
                {
                    return siteInfo;
                }
            }
            return null;
        }

        public async Task<SiteInfo> GetSiteInfoByDirectoryAsync(string siteDir)
        {
            var list = await GetSiteInfoKeyValuePairListAsync();

            foreach (var pair in list)
            {
                var siteInfo = pair.Value;
                if (siteInfo == null) continue;

                if (StringUtils.EqualsIgnoreCase(siteInfo.SiteDir, siteDir))
                {
                    return siteInfo;
                }
            }
            return null;
        }

        public async Task<List<int>> GetSiteIdListAsync()
        {
            var pairList = await GetSiteInfoKeyValuePairListAsync();
            var list = new List<int>();
            foreach (var pair in pairList)
            {
                list.Add(pair.Key);
            }
            return list;
        }

        public async Task<List<int>> GetSiteIdListOrderByLevelAsync()
        {
            var retval = new List<int>();

            var siteIdList = await GetSiteIdListAsync();
            var siteInfoList = new List<SiteInfo>();
            var parentWithChildren = new Hashtable();
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
                        var children = new List<SiteInfo>();
                        if (parentWithChildren.Contains(siteInfo.ParentId))
                        {
                            children = (List<SiteInfo>)parentWithChildren[siteInfo.ParentId];
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

        private void AddSiteIdList(List<int> dataSource, SiteInfo siteInfo, Hashtable parentWithChildren, int level)
        {
            dataSource.Add(siteInfo.Id);

            if (parentWithChildren[siteInfo.Id] != null)
            {
                var children = (List<SiteInfo>)parentWithChildren[siteInfo.Id];
                level++;

                var list = children.OrderBy(child => child.Taxis == 0 ? int.MaxValue : child.Taxis).ToList();

                foreach (var subSiteInfo in list)
                {
                    AddSiteIdList(dataSource, subSiteInfo, parentWithChildren, level);
                }
            }
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

        private async Task<List<string>> GetTableNameListAsync(IPluginManager pluginManager, bool includeSiteTables, bool includePluginTables)
        {

            var tableNames = new List<string>();

            if (includeSiteTables)
            {
                var pairList = await GetSiteInfoKeyValuePairListAsync();
                foreach (var pair in pairList)
                {
                    if (!StringUtils.ContainsIgnoreCase(tableNames, pair.Value.TableName))
                    {
                        tableNames.Add(pair.Value.TableName);
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

        private string GetSiteDir(List<KeyValuePair<int, SiteInfo>> listFromDb, SiteInfo siteInfo)
        {
            if (siteInfo == null || siteInfo.IsRoot) return string.Empty;
            if (siteInfo.ParentId != 0)
            {
                SiteInfo parent = null;
                foreach (var pair in listFromDb)
                {
                    var theSiteId = pair.Key;
                    if (theSiteId != siteInfo.ParentId) continue;
                    parent = pair.Value;
                    break;
                }
                return PathUtils.Combine(GetSiteDir(listFromDb, parent), PathUtils.GetDirectoryName(siteInfo.SiteDir, false));
            }
            return PathUtils.GetDirectoryName(siteInfo.SiteDir, false);
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

        public async Task<bool> IsSiteTableAsync(string tableName)
        {
            var pairList = await GetSiteInfoKeyValuePairListAsync();
            return pairList.Any(pair => StringUtils.EqualsIgnoreCase(pair.Value.TableName, tableName) || StringUtils.EqualsIgnoreCase(pair.Value.TableName, tableName));
        }
    }
}
