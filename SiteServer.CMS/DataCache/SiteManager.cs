using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache
{
    public static class SiteManager
    {
        private static class SiteManagerCache
        {
            private static readonly object LockObject = new object();
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

            public static List<KeyValuePair<int, SiteInfo>> GetSiteInfoKeyValuePairList()
            {
                var retval = DataCacheManager.Get<List<KeyValuePair<int, SiteInfo>>>(CacheKey);
                if (retval != null) return retval;

                lock (LockObject)
                {
                    retval = DataCacheManager.Get<List<KeyValuePair<int, SiteInfo>>>(CacheKey);
                    if (retval == null)
                    {
                        var list = DataProvider.SiteDao.GetSiteInfoKeyValuePairList();
                        retval = new List<KeyValuePair<int, SiteInfo>>();
                        foreach (var pair in list)
                        {
                            var siteInfo = pair.Value;
                            if (siteInfo == null) continue;

                            siteInfo.SiteDir = GetSiteDir(list, siteInfo);
                            retval.Add(pair);
                        }

                        DataCacheManager.Insert(CacheKey, retval);
                    }
                }

                return retval;
            }
        }

        public static void ClearCache()
        {
            SiteManagerCache.Clear();
        }

        public static List<SiteInfo> GetSiteInfoList()
        {
            var pairList = SiteManagerCache.GetSiteInfoKeyValuePairList();
            return pairList.Select(pair => pair.Value).ToList();
        }

        public static SiteInfo GetSiteInfo(int siteId)
        {
            if (siteId <= 0) return null;

            var list = SiteManagerCache.GetSiteInfoKeyValuePairList();

            foreach (var pair in list)
            {
                var theSiteId = pair.Key;
                if (theSiteId != siteId) continue;
                var siteInfo = pair.Value;
                return siteInfo;
            }
            return null;
        }

        public static SiteInfo GetSiteInfoBySiteName(string siteName)
        {
            var list = SiteManagerCache.GetSiteInfoKeyValuePairList();

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

        public static SiteInfo GetSiteInfoByIsRoot()
        {
            var list = SiteManagerCache.GetSiteInfoKeyValuePairList();

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

        public static SiteInfo GetSiteInfoByDirectory(string siteDir)
        {
            var list = SiteManagerCache.GetSiteInfoKeyValuePairList();

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

        public static List<int> GetSiteIdList()
        {
            var pairList = SiteManagerCache.GetSiteInfoKeyValuePairList();
            var list = new List<int>();
            foreach (var pair in pairList)
            {
                list.Add(pair.Key);
            }
            return list;
        }

        public static List<int> GetSiteIdListOrderByLevel()
        {
            var retval = new List<int>();

            var siteIdList = GetSiteIdList();
            var siteInfoList = new List<SiteInfo>();
            var parentWithChildren = new Hashtable();
            var hqSiteId = 0;
            foreach (var siteId in siteIdList)
            {
                var siteInfo = GetSiteInfo(siteId);
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

        private static void AddSiteIdList(List<int> dataSource, SiteInfo siteInfo, Hashtable parentWithChildren, int level)
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

        public static void GetAllParentSiteIdList(List<int> parentSiteIds, List<int> siteIdCollection, int siteId)
        {
            var siteInfo = GetSiteInfo(siteId);
            var parentSiteId = -1;
            foreach (var psId in siteIdCollection)
            {
                if (psId != siteInfo.ParentId) continue;
                parentSiteId = psId;
                break;
            }
            if (parentSiteId == -1) return;

            parentSiteIds.Add(parentSiteId);
            GetAllParentSiteIdList(parentSiteIds, siteIdCollection, parentSiteId);
        }

        public static bool IsExists(int siteId)
        {
            if (siteId == 0) return false;
            return GetSiteInfo(siteId) != null;
        }

        public static List<string> GetSiteTableNames()
        {
            return GetTableNameList(true, false);
        }

        public static List<string> GetAllTableNameList()
        {
            return GetTableNameList(true, true);
        }

        private static List<string> GetTableNameList(bool includeSiteTables, bool includePluginTables)
        {
            
            var tableNames = new List<string>();

            if (includeSiteTables)
            {
                var pairList = SiteManagerCache.GetSiteInfoKeyValuePairList();
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
                var pluginTableNames = PluginContentManager.GetContentTableNameList();
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

        public static List<string> GetTableNameList(SiteInfo siteInfo)
        {
            var tableNames = new List<string>{ siteInfo.TableName };
            var pluginTableNames = PluginContentManager.GetContentTableNameList();
            foreach (var pluginTableName in pluginTableNames)
            {
                if (!StringUtils.ContainsIgnoreCase(tableNames, pluginTableName))
                {
                    tableNames.Add(pluginTableName);
                }
            }
            return tableNames;
        }

        //public static ETableStyle GetTableStyle(SiteInfo siteInfo, string tableName)
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

        public static int GetSiteLevel(int siteId)
        {
            var level = 0;
            var siteInfo = GetSiteInfo(siteId);
            if (siteInfo.ParentId != 0)
            {
                level++;
                level += GetSiteLevel(siteInfo.ParentId);
            }
            return level;
        }

        public static void AddListItems(ListControl listControl)
        {
            var siteIdList = GetSiteIdList();
            var mySystemInfoList = new List<SiteInfo>();
            var parentWithChildren = new Hashtable();
            SiteInfo hqSiteInfo = null;
            foreach (var siteId in siteIdList)
            {
                var siteInfo = GetSiteInfo(siteId);
                if (siteInfo.IsRoot)
                {
                    hqSiteInfo = siteInfo;
                }
                else
                {
                    if (siteInfo.ParentId == 0)
                    {
                        mySystemInfoList.Add(siteInfo);
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
            if (hqSiteInfo != null)
            {
                AddListItem(listControl, hqSiteInfo, parentWithChildren, 0);
            }
            foreach (var siteInfo in mySystemInfoList)
            {
                AddListItem(listControl, siteInfo, parentWithChildren, 0);
            }
        }

        private static void AddListItem(ListControl listControl, SiteInfo siteInfo, Hashtable parentWithChildren, int level)
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

            if (parentWithChildren[siteInfo.Id] != null)
            {
                var children = (List<SiteInfo>)parentWithChildren[siteInfo.Id];
                listControl.Items.Add(new ListItem(padding + siteInfo.SiteName + $"({children.Count})", siteInfo.Id.ToString()));
                level++;
                foreach (SiteInfo subSiteInfo in children)
                {
                    AddListItem(listControl, subSiteInfo, parentWithChildren, level);
                }
            }
            else
            {
                listControl.Items.Add(new ListItem(padding + siteInfo.SiteName, siteInfo.Id.ToString()));
            }
        }

        public static int GetParentSiteId(int siteId)
        {
            var parentSiteId = 0;
            var siteInfo = GetSiteInfo(siteId);
            if (siteInfo != null && siteInfo.IsRoot == false)
            {
                parentSiteId = siteInfo.ParentId;
                if (parentSiteId == 0)
                {
                    parentSiteId = DataProvider.SiteDao.GetIdByIsRoot();
                }
            }
            return parentSiteId;
        }

        private static string GetSiteDir(List<KeyValuePair<int, SiteInfo>> listFromDb, SiteInfo siteInfo)
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

        public static string GetSiteName(SiteInfo siteInfo)
        {
            var padding = string.Empty;

            var level = GetSiteLevel(siteInfo.Id);
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

        public static bool IsSiteTable(string tableName)
        {
            var pairList = SiteManagerCache.GetSiteInfoKeyValuePairList();
            return pairList.Any(pair => StringUtils.EqualsIgnoreCase(pair.Value.TableName, tableName) || StringUtils.EqualsIgnoreCase(pair.Value.TableName, tableName));
        }
    }
}
