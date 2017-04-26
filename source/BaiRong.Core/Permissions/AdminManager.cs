using System;
using System.Collections;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Permissions
{
    public class AdminManager
    {
        private AdminManager()
        {
        }

        public static AdministratorInfo GetAdminInfo(string userName)
        {
            return GetAdminInfo(userName, false);
        }

        public static AdministratorInfo GetAdminInfo(string userName, bool flush)
        {
            var ht = GetActiveAdminInfo();

            AdministratorInfo adminInfo = null;

            if (!flush)
            {
                adminInfo = ht[userName] as AdministratorInfo;
            }

            if (adminInfo == null)
            {
                adminInfo = BaiRongDataProvider.AdministratorDao.GetByUserName(userName);

                if (adminInfo != null)
                {
                    UpdateAdminInfoCache(ht, adminInfo, userName);
                }
            }

            return adminInfo;
        }

        public static string GetDisplayName(string userName, bool isDepartment)
        {
            var adminInfo = GetAdminInfo(userName);
            if (adminInfo == null) return userName;

            if (!isDepartment) return adminInfo.DisplayName;
            var departmentName = DepartmentManager.GetDepartmentName(adminInfo.DepartmentId);
            return !string.IsNullOrEmpty(departmentName) ? $"{adminInfo.DisplayName}({departmentName})" : adminInfo.DisplayName;
        }

        public static string GetFullName(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var adminInfo = GetAdminInfo(userName);
                if (adminInfo != null)
                {
                    string retval = $"账号：{userName}<br />姓名：{adminInfo.DisplayName}";
                    var departmentName = DepartmentManager.GetDepartmentName(adminInfo.DepartmentId);
                    if (!string.IsNullOrEmpty(departmentName))
                    {
                        retval += $"<br />部门：{departmentName}";
                    }
                    return retval;
                }
                return userName;
            }
            return string.Empty;
        }

        private static void UpdateAdminInfoCache(Hashtable ht, AdministratorInfo adminInfo, string userName)
        {
            lock (ht.SyncRoot)
            {
                ht[userName] = adminInfo;
            }
        }

        public static void RemoveCache(string userName)
        {
            var ht = GetActiveAdminInfo();

            lock (ht.SyncRoot)
            {
                ht.Remove(userName);
            }
        }

        public const string CacheKey = "BaiRong.Core.AdminManager";

        public static void Clear()
        {
            CacheUtils.Remove(CacheKey);
        }

        public static Hashtable GetActiveAdminInfo()
        {
            var ht = CacheUtils.Get(CacheKey) as Hashtable;
            if (ht != null) return ht;

            ht = new Hashtable();
            CacheUtils.Insert(CacheKey, ht, null, CacheUtils.HourFactor * 12);
            return ht;
        }

        public static bool CreateAdministrator(AdministratorInfo administratorInfo, out string errorMessage)
        {
            try
            {
                administratorInfo.LastActivityDate = DateUtils.SqlMinValue;
                administratorInfo.CreationDate = DateTime.Now;
                administratorInfo.PasswordFormat = EPasswordFormat.Encrypted;
                var isCreated = BaiRongDataProvider.AdministratorDao.Insert(administratorInfo, out errorMessage);
                if (isCreated == false) return false;

                var roles = new[] { EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator) };
                BaiRongDataProvider.RoleDao.AddUserToRoles(administratorInfo.UserName, roles);

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public const string AnonymousUserName = "Anonymous";

        public static void VerifyAdministratorPermissions(string administratorName, params string[] permissionArray)
        {
            if (HasAdministratorPermissions(administratorName, permissionArray))
            {
                return;
            }
            PageUtils.Redirect(PageUtils.GetAdminDirectoryUrl(string.Empty));
        }

        private static bool HasAdministratorPermissions(string administratorName, params string[] permissionArray)
        {
            var permissions = PermissionsManager.GetPermissions(administratorName);
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
            var cacheKeyStartStrings = PermissionsManager.GetCackeKeyStartStringList(userName);
            foreach (string cacheKeyStartString in cacheKeyStartStrings)
            {
                CacheUtils.RemoveByStartString(cacheKeyStartString);
                //CacheUtils.Remove(cacheKey);
            }
        }

        public static string GetRolesHtml(string userName)
        {
            var isConsoleAdministrator = false;
            var isSystemAdministrator = false;
            var arraylist = new ArrayList();
            var roles = BaiRongDataProvider.RoleDao.GetRolesForUser(userName);
            foreach (var role in roles)
            {
                if (!EPredefinedRoleUtils.IsPredefinedRole(role))
                {
                    arraylist.Add(role);
                }
                else
                {
                    if (EPredefinedRoleUtils.Equals(EPredefinedRole.ConsoleAdministrator, role))
                    {
                        isConsoleAdministrator = true;
                        break;
                    }
                    if (EPredefinedRoleUtils.Equals(EPredefinedRole.SystemAdministrator, role))
                    {
                        isSystemAdministrator = true;
                        break;
                    }
                }
            }

            var retval = string.Empty;

            if (isConsoleAdministrator)
            {
                retval += EPredefinedRoleUtils.GetText(EPredefinedRole.ConsoleAdministrator);
            }
            else if (isSystemAdministrator)
            {
                retval += EPredefinedRoleUtils.GetText(EPredefinedRole.SystemAdministrator);
            }
            else
            {
                retval += TranslateUtils.ObjectCollectionToString(arraylist);
            }
            return retval;
        }

        public static bool HasChannelPermissionIsConsoleAdministrator(string userName)
        {
            var roles = BaiRongDataProvider.RoleDao.GetRolesForUser(userName);

            return EPredefinedRoleUtils.IsConsoleAdministrator(roles);
        }

        public static bool HasChannelPermissionIsSystemAdministrator(string userName)
        {
            var roles = BaiRongDataProvider.RoleDao.GetRolesForUser(userName);

            return EPredefinedRoleUtils.IsSystemAdministrator(roles);
        }
    }
}
