using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache
{
    public static class SiteManager
    {
        private static class SiteManagerCache
        {
            private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(SiteManager));

            //private static readonly FileWatcherClass FileWatcher;

            //static SiteManagerCache()
            //{
            //    FileWatcher = new FileWatcherClass(FileWatcherClass.Site);
            //    FileWatcher.OnFileChange += FileWatcher_OnFileChange;
            //}

            //private static void FileWatcher_OnFileChange(object sender, EventArgs e)
            //{
            //    CacheManager.Remove(CacheKey);
            //}

            public static void Clear()
            {
                DataCacheManager.Remove(CacheKey);
                //FileWatcher.UpdateCacheFile();
            }

            public static async Task<List<KeyValuePair<int, Site>>> GetSiteKeyValuePairListAsync()
            {
                var retVal = DataCacheManager.Get<List<KeyValuePair<int, Site>>>(CacheKey);
                if (retVal != null) return retVal;

                retVal = DataCacheManager.Get<List<KeyValuePair<int, Site>>>(CacheKey);
                if (retVal == null)
                {
                    retVal = await DataProvider.SiteDao.GetSiteKeyValuePairListAsync();
                    //retVal = new List<KeyValuePair<int, Site>>();
                    //foreach (var pair in list)
                    //{
                    //    var site = pair.Value;
                    //    if (site == null) continue;

                    //    site.Children = GetChildren(list, site.Id);
                    //    retVal.Add(pair);
                    //}

                    DataCacheManager.Insert(CacheKey, retVal);
                }

                return retVal;
            }
        }

        public static void ClearCache()
        {
            SiteManagerCache.Clear();
        }

        public static async Task<List<Site>> GetSiteListAsync()
        {
            var pairList = await SiteManagerCache.GetSiteKeyValuePairListAsync();
            return pairList.Select(pair => pair.Value).ToList();
        }

        //private static List<Site> GetChildren(List<KeyValuePair<int, Site>> siteKeyValuePairListAsync, int parentId)
        //{
        //    return siteKeyValuePairListAsync.Where(pair => pair.Value.ParentId == parentId).Select(pair => pair.Value).ToList();
        //}

        public static async Task<Site> GetSiteAsync(int siteId)
        {
            if (siteId <= 0) return null;

            var pairList = await SiteManagerCache.GetSiteKeyValuePairListAsync();
            return pairList.Where(pair => pair.Key == siteId).Select(pair => pair.Value).FirstOrDefault();
        }

        public static async Task<string> GetTableNameAsync(int siteId)
        {
            var site = await GetSiteAsync(siteId);
            return site?.TableName;
        }

        public static async Task<Site> GetSiteBySiteNameAsync(string siteName)
        {
            var list = await SiteManagerCache.GetSiteKeyValuePairListAsync();

            foreach (var pair in list)
            {
                var site = pair.Value;
                if (site == null) continue;

                if (StringUtils.EqualsIgnoreCase(site.SiteName, siteName))
                {
                    return site;
                }
            }
            return null;
        }

        public static async Task<Site> GetSiteByIsRootAsync()
        {
            var list = await SiteManagerCache.GetSiteKeyValuePairListAsync();

            foreach (var pair in list)
            {
                var site = pair.Value;
                if (site == null) continue;

                if (site.Root)
                {
                    return site;
                }
            }
            return null;
        }

        public static async Task<bool> IsRootExistsAsync()
        {
            var list = await SiteManagerCache.GetSiteKeyValuePairListAsync();

            foreach (var pair in list)
            {
                var site = pair.Value;
                if (site == null) continue;

                if (site.Root)
                {
                    return true;
                }
            }
            return false;
        }

        public static async Task<Site> GetSiteByDirectoryAsync(string siteDir)
        {
            var list = await SiteManagerCache.GetSiteKeyValuePairListAsync();

            foreach (var pair in list)
            {
                var site = pair.Value;
                if (site == null) continue;

                if (StringUtils.EqualsIgnoreCase(site.SiteDir, siteDir))
                {
                    return site;
                }
            }
            return null;
        }

        public static async Task<List<int>> GetSiteIdListAsync()
        {
            var pairList = await SiteManagerCache.GetSiteKeyValuePairListAsync();
            var list = new List<int>();
            foreach (var pair in pairList)
            {
                list.Add(pair.Key);
            }
            return list;
        }

        public static async Task<List<int>> GetSiteIdListAsync(int parentId)
        {
            var pairList = await SiteManagerCache.GetSiteKeyValuePairListAsync();
            var list = new List<int>();
            foreach (var pair in pairList)
            {
                if (pair.Value.ParentId == parentId)
                {
                    list.Add(pair.Key);
                }
            }
            return list;
        }

        public static async Task<List<int>> GetSiteIdListOrderByLevelAsync()
        {
            var retVal = new List<int>();

            var siteIdList = await GetSiteIdListAsync();
            var siteList = new List<Site>();
            var parentWithChildren = new Hashtable();
            var hqSiteId = 0;
            foreach (var siteId in siteIdList)
            {
                var site = await GetSiteAsync(siteId);
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

        private static void AddSiteIdList(List<int> dataSource, Site site, Hashtable parentWithChildren, int level)
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

        public static async Task GetAllParentSiteIdListAsync(List<int> parentSiteIds, List<int> siteIdCollection, int siteId)
        {
            var site = await GetSiteAsync(siteId);
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

        public static async Task<bool> IsExistsAsync(int siteId)
        {
            if (siteId == 0) return false;
            return await GetSiteAsync(siteId) != null;
        }

        public static async Task<List<string>> GetSiteTableNamesAsync()
        {
            return await GetTableNameListAsync(true, false);
        }

        public static async Task<List<string>> GetAllTableNameListAsync()
        {
            return await GetTableNameListAsync(true, true);
        }

        private static async Task<List<string>> GetTableNameListAsync(bool includeSiteTables, bool includePluginTables)
        {
            
            var tableNames = new List<string>();

            if (includeSiteTables)
            {
                var pairList = await SiteManagerCache.GetSiteKeyValuePairListAsync();
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

        public static async Task<List<string>> GetTableNameListAsync(Site site)
        {
            var tableNames = new List<string>{ site.TableName };
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

        //public static ETableStyle GetTableStyle(Site site, string tableName)
        //{
        //    var tableStyle = ETableStyle.Custom;

        //    if (StringUtils.EqualsIgnoreCase(tableName, site.AuxiliaryTableForContent))
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

        public static async Task<int> GetSiteLevelAsync(int siteId)
        {
            var level = 0;
            var site = await GetSiteAsync(siteId);
            if (site.ParentId != 0)
            {
                level++;
                level += await GetSiteLevelAsync(site.ParentId);
            }
            return level;
        }

        public static async Task AddListItemsAsync(ListControl listControl)
        {
            var siteIdList = await GetSiteIdListAsync();
            var mySystemInfoList = new List<Site>();
            var parentWithChildren = new Hashtable();
            Site hqSite = null;
            foreach (var siteId in siteIdList)
            {
                var site = await GetSiteAsync(siteId);
                if (site.Root)
                {
                    hqSite = site;
                }
                else
                {
                    if (site.ParentId == 0)
                    {
                        mySystemInfoList.Add(site);
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
            if (hqSite != null)
            {
                AddListItem(listControl, hqSite, parentWithChildren, 0);
            }
            foreach (var site in mySystemInfoList)
            {
                AddListItem(listControl, site, parentWithChildren, 0);
            }
        }

        private static void AddListItem(ListControl listControl, Site site, Hashtable parentWithChildren, int level)
        {
            var padding = string.Empty;
            for (var i = 0; i < level; i++)
            {
                padding += "　";
            }
            if (level > 0)
            {
                padding += "└ ";
            }

            if (parentWithChildren[site.Id] != null)
            {
                var children = (List<Site>)parentWithChildren[site.Id];
                listControl.Items.Add(new ListItem(padding + site.SiteName + $"({children.Count})", site.Id.ToString()));
                level++;
                foreach (Site subSite in children)
                {
                    AddListItem(listControl, subSite, parentWithChildren, level);
                }
            }
            else
            {
                listControl.Items.Add(new ListItem(padding + site.SiteName, site.Id.ToString()));
            }
        }

        public static async Task<int> GetParentSiteIdAsync(int siteId)
        {
            var parentSiteId = 0;
            var site = await GetSiteAsync(siteId);
            if (site != null && site.Root == false)
            {
                parentSiteId = site.ParentId;
                if (parentSiteId == 0)
                {
                    parentSiteId = await DataProvider.SiteDao.GetIdByIsRootAsync();
                }
            }
            return parentSiteId;
        }

        //public static List<int> GetWritingSiteIdList(PermissionsImpl permissionsImpl)
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

        public static async Task<string> GetSiteNameAsync(Site site)
        {
            var padding = string.Empty;

            var level = await GetSiteLevelAsync(site.Id);
            string psLogo;
            if (site.Root)
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

            return $"{padding}<img align='absbottom' border='0' src='{psLogo}'/>&nbsp;{site.SiteName}";
        }

        public static async Task<bool> IsSiteTableAsync(string tableName)
        {
            var pairList = await SiteManagerCache.GetSiteKeyValuePairListAsync();
            return pairList.Any(pair => StringUtils.EqualsIgnoreCase(pair.Value.TableName, tableName) || StringUtils.EqualsIgnoreCase(pair.Value.TableName, tableName));
        }
    }
}
