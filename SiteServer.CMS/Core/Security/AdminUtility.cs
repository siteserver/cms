using SiteServer.Utils;
using System.Collections.Generic;

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
            var request = new Request();
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

        public static bool HasChannelPermissions(string administratorName, int siteId, int channelId, params string[] channelPermissions)
        {
            if (channelId == 0) return false;
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            if (ProductPermissionsManager.Current.ChannelPermissionDict.ContainsKey(channelId) && HasChannelPermissions(administratorName, ProductPermissionsManager.Current.ChannelPermissionDict[channelId], channelPermissions))
            {
                return true;
            }

            var parentChannelId = ChannelManager.GetParentId(siteId, channelId);
            return HasChannelPermissions(administratorName, siteId, parentChannelId, channelPermissions);
        }

        public static bool HasChannelPermissionsIgnoreChannelId(string administratorName, params string[] channelPermissions)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            if (HasChannelPermissions(administratorName, ProductPermissionsManager.Current.ChannelPermissionListIgnoreChannelId, channelPermissions))
            {
                return true;
            }
            return false;
        }

        public static void VerifyChannelPermissions(string administratorName, int siteId, int channelId, params string[] channelPermissions)
        {
            if (HasChannelPermissions(administratorName, siteId, channelId, channelPermissions))
            {
                return;
            }
            var request = new Request();
            request.AdminLogout();
            PageUtils.Redirect(PageUtils.GetAdminDirectoryUrl(string.Empty));
        }

        public static bool IsOwningChannelId(string administratorName, int channelId)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            if (ProductPermissionsManager.Current.OwningChannelIdList.Contains(channelId))
            {
                return true;
            }
            return false;
        }

        public static bool IsHasChildOwningChannelId(string administratorName, int channelId)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            var channelIdList = DataProvider.ChannelDao.GetIdListForDescendant(channelId);
            foreach (var theChannelId in channelIdList)
            {
                if (IsOwningChannelId(administratorName, theChannelId))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsViewContentOnlySelf(string administratorName, int siteId, int channelId)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
            if (permissions.IsConsoleAdministrator || permissions.IsSystemAdministrator)
                return false;
            if (HasChannelPermissions(administratorName, siteId, channelId, ConfigManager.Permissions.Channel.ContentCheck))
                return false;
            return ConfigManager.SystemConfigInfo.IsViewContentOnlySelf;
        }
    }
}
