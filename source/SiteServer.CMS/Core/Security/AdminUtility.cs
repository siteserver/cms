using BaiRong.Core;
using System.Collections.Generic;
using BaiRong.Core.Permissions;

namespace SiteServer.CMS.Core.Security
{
    public class AdminUtility
    {
        public static bool HasWebsitePermissions(string administratorName, int publishmentSystemId, params string[] websitePermissionArray)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            if (ProductPermissionsManager.Current.WebsitePermissionDict.ContainsKey(publishmentSystemId))
            {
                var websitePermissionList = ProductPermissionsManager.Current.WebsitePermissionDict[publishmentSystemId];
                if (websitePermissionList != null && websitePermissionList.Count > 0)
                {
                    foreach (var websitePermission in websitePermissionArray)
                    {
                        if (websitePermissionList.Contains(websitePermission))
                        {
                            return true;
                        }
                    }
                }
            }            

            return false;
        }

        public static void VerifyWebsitePermissions(string administratorName, int publishmentSystemId, params string[] websitePermissionArray)
        {
            if (HasWebsitePermissions(administratorName, publishmentSystemId, websitePermissionArray))
            {
                return;
            }
            RequestBody.AdministratorLogout();
            PageUtils.Redirect(PageUtils.GetAdminDirectoryUrl(string.Empty));
        }

        public static bool HasChannelPermissions(string administratorName, List<string> channelPermissionList, params string[] channelPermissionArray)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            foreach (var channelPermission in channelPermissionArray)
            {
                if (channelPermissionList.Contains(channelPermission))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasChannelPermissions(string administratorName, int publishmentSystemId, int nodeId, params string[] channelPermissionArray)
        {
            if (nodeId == 0) return false;
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            if (ProductPermissionsManager.Current.ChannelPermissionDict.ContainsKey(nodeId) && HasChannelPermissions(administratorName, ProductPermissionsManager.Current.ChannelPermissionDict[nodeId], channelPermissionArray))
            {
                return true;
            }

            var parentNodeId = NodeManager.GetParentId(publishmentSystemId, nodeId);
            return HasChannelPermissions(administratorName, publishmentSystemId, parentNodeId, channelPermissionArray);
        }

        public static bool HasChannelPermissionsIgnoreNodeId(string administratorName, params string[] channelPermissionArray)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            if (HasChannelPermissions(administratorName, ProductPermissionsManager.Current.ChannelPermissionListIgnoreNodeId, channelPermissionArray))
            {
                return true;
            }
            return false;
        }

        public static void VerifyChannelPermissions(string administratorName, int publishmentSystemId, int nodeId, params string[] channelPermissionArray)
        {
            if (HasChannelPermissions(administratorName, publishmentSystemId, nodeId, channelPermissionArray))
            {
                return;
            }
            RequestBody.AdministratorLogout();
            PageUtils.Redirect(PageUtils.GetAdminDirectoryUrl(string.Empty));
        }

        public static bool IsOwningNodeId(string administratorName, int nodeId)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            if (ProductPermissionsManager.Current.OwningNodeIdList.Contains(nodeId))
            {
                return true;
            }
            return false;
        }

        public static bool IsHasChildOwningNodeId(string administratorName, int nodeId)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(nodeId);
            foreach (var theNodeId in nodeIdList)
            {
                if (IsOwningNodeId(administratorName, theNodeId))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsViewContentOnlySelf(string administratorName, int publishmentSystemId, int nodeId)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsConsoleAdministrator || permissions.IsSystemAdministrator)
                return false;
            if (HasChannelPermissions(administratorName, publishmentSystemId, nodeId, AppManager.Cms.Permission.Channel.ContentCheck))
                return false;
            return ConfigManager.SystemConfigInfo.IsViewContentOnlySelf;
        }
    }
}
