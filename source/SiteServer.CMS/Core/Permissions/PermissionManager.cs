using System.Collections.Generic;
using BaiRong.Core;

namespace SiteServer.CMS.Core.Permissions
{
	public class PermissionsManager
	{
        private readonly string _userName;
        private AdministratorWithPermissions _permissions;

        private PermissionsManager(string userName)
        {
            _userName = userName;
        }

        //public static AdministratorWithPermissions Current
        //{
        //    get
        //    {
        //        var instance = new PermissionsManager(RequestBody.CurrentAdministratorName);
        //        return instance.Permissions;
        //    }
        //}

	    public static AdministratorWithPermissions GetPermissions(string administratorName)
	    {
            var instance = new PermissionsManager(administratorName);
            return instance.Permissions;
        }

        public AdministratorWithPermissions Permissions
        {
            get
            {
                if (_permissions == null)
                {
                    _permissions = !string.IsNullOrEmpty(_userName) ? new AdministratorWithPermissions(_userName) : AdministratorWithPermissions.GetAnonymousUserWithPermissions();
                }
                return _permissions;
            }
            set { _permissions = value; }
        }

        public static string GetRolesKey(string userName)
        {
            return "User_Roles:" + userName;
        }

        public static string GetPermissionListKey(string userName)
        {
            return "User_PermissionList:" + userName;
        }

        public static string GetWebsitePermissionDictKey(string userName)
        {
            return $"User_WebsitePermissionDict_{userName}";
        }

        public static string GetChannelPermissionDictKey(string userName)
        {
            return $"User_ChannelPermissionDict_{userName}";
        }

        public static string GetChannelPermissionListIgnoreNodeIdKey(string userName)
        {
            return $"User_ChannelPermissionListIgnoreNodeID_{userName}";
        }

        public static string GetPublishmentSystemIdKey(string userName)
        {
            return $"User_PublishmentSystemID_{userName}";
        }

        public static string GetOwningNodeIdListKey(string userName)
        {
            return $"User_OwningNodeIDList_{userName}";
        }

        public static List<string> GetCackeKeyStartStringList(string userName)
        {
            return new List<string>
            {
                GetRolesKey(userName),
                GetPermissionListKey(userName),
                GetWebsitePermissionDictKey(userName),
                GetChannelPermissionDictKey(userName),
                GetChannelPermissionListIgnoreNodeIdKey(userName),
                GetPublishmentSystemIdKey(userName),
                GetOwningNodeIdListKey(userName)
            };
        }

        public static void ClearAllCache()
        {
            var list = GetCackeKeyStartStringList(string.Empty);
            foreach (var cacheKeyStart in list)
            {
                CacheUtils.RemoveByStartString(cacheKeyStart);
            }
        }

        public static void VerifyAdministratorPermissions(string administratorName, params string[] permissionArray)
        {
            if (HasAdministratorPermissions(administratorName, permissionArray))
            {
                return;
            }
            PageUtils.Redirect(PageUtils.GetAdminDirectoryUrl(string.Empty));
        }

        public static bool HasAdministratorPermissions(string administratorName, params string[] permissionArray)
        {
            var permissions = GetPermissions(administratorName);
            if (permissions.IsSystemAdministrator)
            {
                return true;
            }
            var permissionList = permissions.PermissionList;
            foreach (var permission in permissionArray)
            {
                if (permissionList.Contains(permission))
                {
                    return true;
                }
            }

            return false;
        }

        public static void ClearUserCache(string userName)
        {
            var cacheKeyStartStrings = GetCackeKeyStartStringList(userName);
            foreach (var cacheKeyStartString in cacheKeyStartStrings)
            {
                CacheUtils.RemoveByStartString(cacheKeyStartString);
                //CacheUtils.Remove(cacheKey);
            }
        }
    }
}
