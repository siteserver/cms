using System;
using System.Collections;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core
{
    public static class AdminManager
    {
        private static readonly object LockObject = new object();

        public static AdministratorInfo GetAdminInfo(string userName)
        {
            var ht = GetActiveAdminInfo();

            var adminInfo = ht[userName] as AdministratorInfo;
            if (adminInfo != null) return adminInfo;

            lock (LockObject)
            {
                adminInfo = ht[userName] as AdministratorInfo;

                if (adminInfo == null)
                {
                    adminInfo = DataProvider.AdministratorDao.GetByUserName(userName);
                    ht[userName] = adminInfo;
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
            var retval = CacheUtils.Get<Hashtable>(CacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = CacheUtils.Get<Hashtable>(CacheKey);
                if (retval == null)
                {
                    retval = new Hashtable();
                    CacheUtils.Insert(CacheKey, retval);
                }
            }

            return retval;
        }

        public static bool CreateAdministrator(AdministratorInfo administratorInfo, out string errorMessage)
        {
            try
            {
                administratorInfo.LastActivityDate = DateUtils.SqlMinValue;
                administratorInfo.CreationDate = DateTime.Now;
                administratorInfo.PasswordFormat = EPasswordFormatUtils.GetValue(EPasswordFormat.Encrypted);
                var isCreated = DataProvider.AdministratorDao.Insert(administratorInfo, out errorMessage);
                if (isCreated == false) return false;

                var roles = new[] { EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator) };
                DataProvider.AdministratorsInRolesDao.AddUserToRoles(administratorInfo.UserName, roles);

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public const string AnonymousUserName = "Anonymous";

        public static string GetRolesHtml(string userName)
        {
            var isConsoleAdministrator = false;
            var isSystemAdministrator = false;
            var arraylist = new ArrayList();
            var roles = DataProvider.AdministratorsInRolesDao.GetRolesForUser(userName);
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

        //public static bool HasChannelPermissionIsConsoleAdministrator(string userName)
        //{
        //    var roles = DataProvider.AdministratorsInRolesDao.GetRolesForUser(userName);

        //    return EPredefinedRoleUtils.IsConsoleAdministrator(roles);
        //}

        //public static bool HasChannelPermissionIsSystemAdministrator(string userName)
        //{
        //    var roles = DataProvider.AdministratorsInRolesDao.GetRolesForUser(userName);

        //    return EPredefinedRoleUtils.IsSystemAdministrator(roles);
        //}
    }
}
