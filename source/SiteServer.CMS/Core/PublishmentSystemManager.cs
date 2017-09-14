using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.IO;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core
{
    public static class PublishmentSystemManager
    {
        private const string CacheFileName = "PublishmentSystemCache.txt";

        static PublishmentSystemManager()
        {
            var fileWatcher = new FileWatcherClass(PathUtility.GetCacheFilePath(CacheFileName));
            fileWatcher.OnFileChange += fileWatcher_OnFileChange;
        }

        private static void fileWatcher_OnFileChange(object sender, EventArgs e)
        {
            CacheUtils.Remove(CacheKey);
        }

        public static PublishmentSystemInfo GetPublishmentSystemInfo(int publishmentSystemId)
        {
            if (publishmentSystemId <= 0) return null;

            var list = GetPublishmentSystemInfoKeyValuePairList();

            foreach (var pair in list)
            {
                var thePublishmentSystemId = pair.Key;
                if (thePublishmentSystemId != publishmentSystemId) continue;
                var publishmentSystemInfo = pair.Value;
                return publishmentSystemInfo;
            }
            return null;
        }

        public static PublishmentSystemInfo GetPublishmentSystemInfoBySiteName(string publishmentSystemName)
        {
            var list = GetPublishmentSystemInfoKeyValuePairList();

            foreach (var pair in list)
            {
                var publishmentSystemInfo = pair.Value;
                if (publishmentSystemInfo == null) continue;

                if (StringUtils.EqualsIgnoreCase(publishmentSystemInfo.PublishmentSystemName, publishmentSystemName))
                {
                    return publishmentSystemInfo;
                }
            }
            return null;
        }

        public static PublishmentSystemInfo GetPublishmentSystemInfoByDirectory(string publishmentSystemDir)
        {
            var list = GetPublishmentSystemInfoKeyValuePairList();

            foreach (var pair in list)
            {
                var publishmentSystemInfo = pair.Value;
                if (publishmentSystemInfo == null) continue;

                if (StringUtils.EqualsIgnoreCase(publishmentSystemInfo.PublishmentSystemDir, publishmentSystemDir))
                {
                    return publishmentSystemInfo;
                }
            }
            return null;
        }

        public static List<int> GetPublishmentSystemIdList()
        {
            var pairList = GetPublishmentSystemInfoKeyValuePairList();
            var list = new List<int>();
            foreach (var pair in pairList)
            {
                list.Add(pair.Key);
            }
            return list;
        }

        public static List<int> GetPublishmentSystemIdListOrderByLevel()
        {
            var retval = new List<int>();

            var publishmentSystemIdList = GetPublishmentSystemIdList();
            var publishmentSystemInfoList = new List<PublishmentSystemInfo>();
            var parentWithChildren = new Hashtable();
            var hqPublishmentSystemId = 0;
            foreach (var publishmentSystemId in publishmentSystemIdList)
            {
                var publishmentSystemInfo = GetPublishmentSystemInfo(publishmentSystemId);
                if (publishmentSystemInfo.IsHeadquarters)
                {
                    hqPublishmentSystemId = publishmentSystemInfo.PublishmentSystemId;
                }
                else
                {
                    if (publishmentSystemInfo.ParentPublishmentSystemId == 0)
                    {
                        publishmentSystemInfoList.Add(publishmentSystemInfo);
                    }
                    else
                    {
                        var children = new List<PublishmentSystemInfo>();
                        if (parentWithChildren.Contains(publishmentSystemInfo.ParentPublishmentSystemId))
                        {
                            children = (List<PublishmentSystemInfo>)parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId];
                        }
                        children.Add(publishmentSystemInfo);
                        parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId] = children;
                    }
                }
            }

            if (hqPublishmentSystemId > 0)
            {
                retval.Add(hqPublishmentSystemId);
            }
            foreach (var publishmentSystemInfo in publishmentSystemInfoList)
            {
                AddPublishmentSystemIdList(retval, publishmentSystemInfo, parentWithChildren, 0);
            }
            return retval;
        }

        private static void AddPublishmentSystemIdList(List<int> dataSource, PublishmentSystemInfo publishmentSystemInfo, Hashtable parentWithChildren, int level)
        {
            dataSource.Add(publishmentSystemInfo.PublishmentSystemId);

            if (parentWithChildren[publishmentSystemInfo.PublishmentSystemId] != null)
            {
                var children = (List<PublishmentSystemInfo>)parentWithChildren[publishmentSystemInfo.PublishmentSystemId];
                level++;
                foreach (var subSiteInfo in children)
                {
                    AddPublishmentSystemIdList(dataSource, subSiteInfo, parentWithChildren, level);
                }
            }
        }

        public static void GetAllParentPublishmentSystemIdList(List<int> parentPublishmentSystemIDs, List<int> publishmentSystemIdCollection, int publishmentSystemId)
        {
            var publishmentSystemInfo = GetPublishmentSystemInfo(publishmentSystemId);
            var parentPublishmentSystemId = -1;
            foreach (var psId in publishmentSystemIdCollection)
            {
                if (psId != publishmentSystemInfo.ParentPublishmentSystemId) continue;
                parentPublishmentSystemId = psId;
                break;
            }
            if (parentPublishmentSystemId == -1) return;

            parentPublishmentSystemIDs.Add(parentPublishmentSystemId);
            GetAllParentPublishmentSystemIdList(parentPublishmentSystemIDs, publishmentSystemIdCollection, parentPublishmentSystemId);
        }

        public static bool IsExists(int publishmentSystemId)
        {
            if (publishmentSystemId == 0) return false;
            return GetPublishmentSystemInfo(publishmentSystemId) != null;
        }

        public static List<string> GetAuxiliaryTableNameList(PublishmentSystemInfo publishmentSystemInfo)
        {
            return new List <string>
            {
                publishmentSystemInfo.AuxiliaryTableForContent
            };
        }

        public static ETableStyle GetTableStyle(PublishmentSystemInfo publishmentSystemInfo, string tableName)
        {
            var tableStyle = ETableStyle.Custom;

            if (StringUtils.EqualsIgnoreCase(tableName, publishmentSystemInfo.AuxiliaryTableForContent))
            {
                tableStyle = ETableStyle.BackgroundContent;
            }
            else if (StringUtils.EqualsIgnoreCase(tableName, DataProvider.PublishmentSystemDao.TableName))
            {
                tableStyle = ETableStyle.Site;
            }
            else if (StringUtils.EqualsIgnoreCase(tableName, DataProvider.NodeDao.TableName))
            {
                tableStyle = ETableStyle.Channel;
            }
            else if (StringUtils.EqualsIgnoreCase(tableName, DataProvider.InputContentDao.TableName))
            {
                tableStyle = ETableStyle.InputContent;
            }
            return tableStyle;
        }

        public static int GetPublishmentSystemLevel(int publishmentSystemId)
        {
            var level = 0;
            var publishmentSystemInfo = GetPublishmentSystemInfo(publishmentSystemId);
            if (publishmentSystemInfo.ParentPublishmentSystemId != 0)
            {
                level++;
                level += GetPublishmentSystemLevel(publishmentSystemInfo.ParentPublishmentSystemId);
            }
            return level;
        }

        public static void AddListItems(ListControl listControl)
        {
            var publishmentSystemIdList = GetPublishmentSystemIdList();
            var mySystemInfoList = new List<PublishmentSystemInfo>();
            var parentWithChildren = new Hashtable();
            PublishmentSystemInfo hqPublishmentSystemInfo = null;
            foreach (var publishmentSystemId in publishmentSystemIdList)
            {
                var publishmentSystemInfo = GetPublishmentSystemInfo(publishmentSystemId);
                if (publishmentSystemInfo.IsHeadquarters)
                {
                    hqPublishmentSystemInfo = publishmentSystemInfo;
                }
                else
                {
                    if (publishmentSystemInfo.ParentPublishmentSystemId == 0)
                    {
                        mySystemInfoList.Add(publishmentSystemInfo);
                    }
                    else
                    {
                        var children = new List<PublishmentSystemInfo>();
                        if (parentWithChildren.Contains(publishmentSystemInfo.ParentPublishmentSystemId))
                        {
                            children = (List<PublishmentSystemInfo>)parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId];
                        }
                        children.Add(publishmentSystemInfo);
                        parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId] = children;
                    }
                }
            }
            if (hqPublishmentSystemInfo != null)
            {
                AddListItem(listControl, hqPublishmentSystemInfo, parentWithChildren, 0);
            }
            foreach (var publishmentSystemInfo in mySystemInfoList)
            {
                AddListItem(listControl, publishmentSystemInfo, parentWithChildren, 0);
            }
        }

        private static void AddListItem(ListControl listControl, PublishmentSystemInfo publishmentSystemInfo, Hashtable parentWithChildren, int level)
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

            if (parentWithChildren[publishmentSystemInfo.PublishmentSystemId] != null)
            {
                var children = (List<PublishmentSystemInfo>)parentWithChildren[publishmentSystemInfo.PublishmentSystemId];
                listControl.Items.Add(new ListItem(padding + publishmentSystemInfo.PublishmentSystemName + $"({children.Count})", publishmentSystemInfo.PublishmentSystemId.ToString()));
                level++;
                foreach (PublishmentSystemInfo subSiteInfo in children)
                {
                    AddListItem(listControl, subSiteInfo, parentWithChildren, level);
                }
            }
            else
            {
                listControl.Items.Add(new ListItem(padding + publishmentSystemInfo.PublishmentSystemName, publishmentSystemInfo.PublishmentSystemId.ToString()));
            }
        }

        public static int GetParentPublishmentSystemId(int publishmentSystemId)
        {
            var parentPublishmentSystemId = 0;
            var publishmentSystemInfo = GetPublishmentSystemInfo(publishmentSystemId);
            if (publishmentSystemInfo != null && publishmentSystemInfo.IsHeadquarters == false)
            {
                parentPublishmentSystemId = publishmentSystemInfo.ParentPublishmentSystemId;
                if (parentPublishmentSystemId == 0)
                {
                    parentPublishmentSystemId = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();
                }
            }
            return parentPublishmentSystemId;
        }

        public static void ClearCache()
        {
            CacheUtils.Remove(CacheKey);
            CacheUtils.UpdateTemporaryCacheFile(CacheFileName);
        }

        private static readonly object LockObject = new object();

        public static List<KeyValuePair<int, PublishmentSystemInfo>> GetPublishmentSystemInfoKeyValuePairList()
        {
            var retval = CacheUtils.Get<List<KeyValuePair<int, PublishmentSystemInfo>>>(CacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = CacheUtils.Get<List<KeyValuePair<int, PublishmentSystemInfo>>>(CacheKey);
                if (retval == null)
                {
                    var list = DataProvider.PublishmentSystemDao.GetPublishmentSystemInfoKeyValuePairList();
                    retval = new List<KeyValuePair<int, PublishmentSystemInfo>>();
                    foreach (var pair in list)
                    {
                        var publishmentSystemInfo = pair.Value;
                        if (publishmentSystemInfo == null) continue;

                        publishmentSystemInfo.PublishmentSystemDir = GetPublishmentSystemDir(list, publishmentSystemInfo);
                        retval.Add(pair);
                    }

                    CacheUtils.Insert(CacheKey, retval);
                }
            }

            return retval;
        }

        private static string GetPublishmentSystemDir(List<KeyValuePair<int, PublishmentSystemInfo>> listFromDb, PublishmentSystemInfo publishmentSystemInfo)
        {
            if (publishmentSystemInfo == null || publishmentSystemInfo.IsHeadquarters) return string.Empty;
            if (publishmentSystemInfo.ParentPublishmentSystemId != 0)
            {
                PublishmentSystemInfo parent = null;
                foreach (var pair in listFromDb)
                {
                    var thePublishmentSystemId = pair.Key;
                    if (thePublishmentSystemId != publishmentSystemInfo.ParentPublishmentSystemId) continue;
                    parent = pair.Value;
                    break;
                }
                return PathUtils.Combine(GetPublishmentSystemDir(listFromDb, parent), PathUtils.GetDirectoryName(publishmentSystemInfo.PublishmentSystemDir));
            }
            return PathUtils.GetDirectoryName(publishmentSystemInfo.PublishmentSystemDir);
        }

        /****************** Cache *********************/

        private const string CacheKey = "SiteServer.CMS.Core.PublishmentSystemManager";

        public static List<PublishmentSystemInfo> GetWritingPublishmentSystemInfoList(string adminUserName)
        {
            var publishmentSystemInfoList = new List<PublishmentSystemInfo>();

            if (!string.IsNullOrEmpty(adminUserName))
            {
                var publishmentSystemIdList = new List<int>();

                if (AdminManager.HasChannelPermissionIsConsoleAdministrator(adminUserName) || AdminManager.HasChannelPermissionIsSystemAdministrator(adminUserName))//如果是超级管理员或站点管理员
                {
                    var ps = new ProductAdministratorWithPermissions(adminUserName);
                    foreach (var itemForPsid in ps.WebsitePermissionDict.Keys)
                    {
                        if (!publishmentSystemIdList.Contains(itemForPsid))
                        {
                            var publishmentSystemInfo = GetPublishmentSystemInfo(itemForPsid);
                            publishmentSystemInfoList.Add(publishmentSystemInfo);
                            publishmentSystemIdList.Add(itemForPsid);
                        }
                    }
                }
                else
                {
                    var roles = BaiRongDataProvider.RoleDao.GetRolesForUser(adminUserName);
                    var ps = new ProductAdministratorWithPermissions(adminUserName);
                    foreach (var itemForPsid in ps.WebsitePermissionDict.Keys)
                    {
                        if (!publishmentSystemIdList.Contains(itemForPsid))
                        {
                            var nodeIdCollection = DataProvider.SystemPermissionsDao.GetAllPermissionList(roles, itemForPsid, true);
                            var publishmentSystemInfo = GetPublishmentSystemInfo(itemForPsid);
                            if (nodeIdCollection.Count > 0)
                            {
                                publishmentSystemInfoList.Add(publishmentSystemInfo);
                                publishmentSystemIdList.Add(itemForPsid);
                            }
                        }
                    }
                }
            }

            return publishmentSystemInfoList;
        }

        public static List<NodeInfo> GetWritingNodeInfoList(string adminUserName, int publishmentSystemId)
        {
            var nodeInfoList = new List<NodeInfo>();
            if (!string.IsNullOrEmpty(adminUserName))
            {
                if (AdminManager.HasChannelPermissionIsConsoleAdministrator(adminUserName) || AdminManager.HasChannelPermissionIsSystemAdministrator(adminUserName))//如果是超级管理员或站点管理员
                {
                    var nodeList = DataProvider.NodeDao.GetNodeInfoListByPublishmentSystemId(publishmentSystemId, string.Empty);
                    foreach (var nodeInfo in nodeList)
                    {
                        if (nodeInfo != null)
                        {
                            nodeInfoList.Add(nodeInfo);
                        }
                    }
                }
                else
                {
                    var ps = new ProductAdministratorWithPermissions(adminUserName);
                    ICollection nodeIdCollection = ps.ChannelPermissionDict.Keys;
                    foreach (int nodeId in nodeIdCollection)
                    {
                        var nodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(nodeId);
                        nodeIdList.Insert(0, nodeId);

                        foreach (int ownNodeId in nodeIdList)
                        {
                            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, ownNodeId);
                            if (nodeInfo != null)
                            {
                                nodeInfoList.Add(nodeInfo);
                            }
                        }
                    }
                }
            }
            return nodeInfoList;
        }

    }
}
