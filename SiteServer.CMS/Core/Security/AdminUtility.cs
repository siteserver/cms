using SiteServer.Utils;
using System.Collections.Generic;
using SiteServer.CMS.Plugin.Model;

namespace SiteServer.CMS.Core.Security
{
    public class AdminUtility
    {
        public static bool HasSitePermissions(string administratorName, int siteId, params string[] sitePermissions)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            if (ProductPermissionsManager.Current.WebsitePermissionDict.ContainsKey(siteId))
            {
                var websitePermissionList = ProductPermissionsManager.Current.WebsitePermissionDict[siteId];
                if (websitePermissionList != null && websitePermissionList.Count > 0)
                {
                    foreach (var sitePermission in sitePermissions)
                    {
                        if (websitePermissionList.Contains(sitePermission))
                        {
                            return true;
                        }
                    }
                }
            }            

            return false;
        }

        public static void VerifySitePermissions(string administratorName, int siteId, params string[] sitePermissions)
        {
            if (HasSitePermissions(administratorName, siteId, sitePermissions))
            {
                return;
            }
            var request = new RequestContext();
            request.AdminLogout();
            PageUtils.Redirect(PageUtils.GetAdminDirectoryUrl(string.Empty));
        }

        public static bool HasChannelPermissions(string administratorName, List<string> channelPermissionList, params string[] channelPermissions)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            foreach (var channelPermission in channelPermissions)
            {
                if (channelPermissionList.Contains(channelPermission))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasChannelPermissions(string administratorName, int siteId, int nodeId, params string[] channelPermissions)
        {
            if (nodeId == 0) return false;
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            if (ProductPermissionsManager.Current.ChannelPermissionDict.ContainsKey(nodeId) && HasChannelPermissions(administratorName, ProductPermissionsManager.Current.ChannelPermissionDict[nodeId], channelPermissions))
            {
                return true;
            }

            var parentNodeId = ChannelManager.GetParentId(siteId, nodeId);
            return HasChannelPermissions(administratorName, siteId, parentNodeId, channelPermissions);
        }

        public static bool HasChannelPermissionsIgnoreNodeId(string administratorName, params string[] channelPermissions)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            if (HasChannelPermissions(administratorName, ProductPermissionsManager.Current.ChannelPermissionListIgnoreNodeId, channelPermissions))
            {
                return true;
            }
            return false;
        }

        public static void VerifyChannelPermissions(string administratorName, int siteId, int nodeId, params string[] channelPermissions)
        {
            if (HasChannelPermissions(administratorName, siteId, nodeId, channelPermissions))
            {
                return;
            }
            var request = new RequestContext();
            request.AdminLogout();
            PageUtils.Redirect(PageUtils.GetAdminDirectoryUrl(string.Empty));
        }

        public static bool IsOwningNodeId(string administratorName, int nodeId)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            if (ProductPermissionsManager.Current.OwningChannelIdList.Contains(nodeId))
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
            var nodeIdList = DataProvider.ChannelDao.GetIdListForDescendant(nodeId);
            foreach (var theNodeId in nodeIdList)
            {
                if (IsOwningNodeId(administratorName, theNodeId))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsViewContentOnlySelf(string administratorName, int siteId, int nodeId)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsConsoleAdministrator || permissions.IsSystemAdministrator)
                return false;
            if (HasChannelPermissions(administratorName, siteId, nodeId, AppManager.Permissions.Channel.ContentCheck))
                return false;
            return ConfigManager.SystemConfigInfo.IsViewContentOnlySelf;
        }
    }
}
