using System.Collections;
using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.DataCache
{
    public static class AdminManager
    {
        private static class AdminManagerCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(AdminManager));

            public static void Clear()
            {
                DataCacheManager.Remove(CacheKey);
            }

            public static void Update(AdministratorInfo adminInfo)
            {
                if (adminInfo == null) return;

                var dict = GetDict();

                lock (LockObject)
                {
                    dict[GetDictKeyByUserId(adminInfo.Id)] = adminInfo;
                    dict[GetDictKeyByUserName(adminInfo.UserName)] = adminInfo;
                    if (!string.IsNullOrEmpty(adminInfo.Mobile))
                    {
                        dict[GetDictKeyByMobile(adminInfo.Mobile)] = adminInfo;
                    }
                    if (!string.IsNullOrEmpty(adminInfo.Email))
                    {
                        dict[GetDictKeyByEmail(adminInfo.Email)] = adminInfo;
                    }
                }
            }

            public static void Remove(AdministratorInfo adminInfo)
            {
                if (adminInfo == null) return;

                var dict = GetDict();

                lock (LockObject)
                {
                    dict.Remove(GetDictKeyByUserId(adminInfo.Id));
                    dict.Remove(GetDictKeyByUserName(adminInfo.UserName));
                    if (!string.IsNullOrEmpty(adminInfo.Mobile))
                    {
                        dict.Remove(GetDictKeyByMobile(adminInfo.Mobile));
                    }

                    if (!string.IsNullOrEmpty(adminInfo.Email))
                    {
                        dict.Remove(GetDictKeyByEmail(adminInfo.Email));
                    }
                }
            }

            private static string GetDictKeyByUserId(int userId)
            {
                return $"userId:{userId}";
            }

            private static string GetDictKeyByUserName(string userName)
            {
                return $"userName:{userName}";
            }

            private static string GetDictKeyByMobile(string mobile)
            {
                return $"mobile:{mobile}";
            }

            private static string GetDictKeyByEmail(string email)
            {
                return $"email:{email}";
            }

            public static AdministratorInfo GetCacheByUserId(int userId)
            {
                if (userId <= 0) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByUserId(userId), out AdministratorInfo adminInfo);
                if (adminInfo != null) return adminInfo;

                lock (LockObject)
                {
                    dict.TryGetValue(GetDictKeyByUserId(userId), out adminInfo);

                    if (adminInfo == null)
                    {
                        adminInfo = DataProvider.AdministratorDao.GetByUserId(userId);
                        if (adminInfo != null)
                        {
                            dict[GetDictKeyByUserId(adminInfo.Id)] = adminInfo;
                            dict[GetDictKeyByUserName(adminInfo.UserName)] = adminInfo;
                            if (!string.IsNullOrEmpty(adminInfo.Mobile))
                            {
                                dict[GetDictKeyByMobile(adminInfo.Mobile)] = adminInfo;
                            }
                            if (!string.IsNullOrEmpty(adminInfo.Email))
                            {
                                dict[GetDictKeyByEmail(adminInfo.Email)] = adminInfo;
                            }
                        }
                    }
                }

                return adminInfo;
            }

            public static AdministratorInfo GetCacheByUserName(string userName)
            {
                if (string.IsNullOrEmpty(userName)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByUserName(userName), out AdministratorInfo adminInfo);
                if (adminInfo != null) return adminInfo;

                lock (LockObject)
                {
                    dict.TryGetValue(GetDictKeyByUserName(userName), out adminInfo);

                    if (adminInfo == null)
                    {
                        adminInfo = DataProvider.AdministratorDao.GetByUserName(userName);
                        if (adminInfo != null)
                        {
                            dict[GetDictKeyByUserId(adminInfo.Id)] = adminInfo;
                            dict[GetDictKeyByUserName(adminInfo.UserName)] = adminInfo;
                            if (!string.IsNullOrEmpty(adminInfo.Mobile))
                            {
                                dict[GetDictKeyByMobile(adminInfo.Mobile)] = adminInfo;
                            }
                            if (!string.IsNullOrEmpty(adminInfo.Email))
                            {
                                dict[GetDictKeyByEmail(adminInfo.Email)] = adminInfo;
                            }
                        }
                    }
                }

                return adminInfo;
            }

            public static AdministratorInfo GetCacheByMobile(string mobile)
            {
                if (string.IsNullOrEmpty(mobile)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByMobile(mobile), out AdministratorInfo adminInfo);
                if (adminInfo != null) return adminInfo;

                lock (LockObject)
                {
                    dict.TryGetValue(GetDictKeyByMobile(mobile), out adminInfo);

                    if (adminInfo == null)
                    {
                        adminInfo = DataProvider.AdministratorDao.GetByMobile(mobile);
                        if (adminInfo != null)
                        {
                            dict[GetDictKeyByUserId(adminInfo.Id)] = adminInfo;
                            dict[GetDictKeyByUserName(adminInfo.UserName)] = adminInfo;
                            if (!string.IsNullOrEmpty(adminInfo.Mobile))
                            {
                                dict[GetDictKeyByMobile(adminInfo.Mobile)] = adminInfo;
                            }
                            if (!string.IsNullOrEmpty(adminInfo.Email))
                            {
                                dict[GetDictKeyByEmail(adminInfo.Email)] = adminInfo;
                            }
                        }
                    }
                }

                return adminInfo;
            }

            public static AdministratorInfo GetCacheByEmail(string email)
            {
                if (string.IsNullOrEmpty(email)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByEmail(email), out AdministratorInfo adminInfo);
                if (adminInfo != null) return adminInfo;

                lock (LockObject)
                {
                    dict.TryGetValue(GetDictKeyByEmail(email), out adminInfo);

                    if (adminInfo == null)
                    {
                        adminInfo = DataProvider.AdministratorDao.GetByEmail(email);
                        if (adminInfo != null)
                        {
                            dict[GetDictKeyByUserId(adminInfo.Id)] = adminInfo;
                            dict[GetDictKeyByUserName(adminInfo.UserName)] = adminInfo;
                            if (!string.IsNullOrEmpty(adminInfo.Mobile))
                            {
                                dict[GetDictKeyByMobile(adminInfo.Mobile)] = adminInfo;
                            }
                            if (!string.IsNullOrEmpty(adminInfo.Email))
                            {
                                dict[GetDictKeyByEmail(adminInfo.Email)] = adminInfo;
                            }
                        }
                    }
                }

                return adminInfo;
            }

            private static Dictionary<string, AdministratorInfo> GetDict()
            {
                var retval = DataCacheManager.Get<Dictionary<string, AdministratorInfo>>(CacheKey);
                if (retval != null) return retval;

                lock (LockObject)
                {
                    retval = DataCacheManager.Get<Dictionary<string, AdministratorInfo>>(CacheKey);
                    if (retval == null)
                    {
                        retval = new Dictionary<string, AdministratorInfo>();
                        DataCacheManager.Insert(CacheKey, retval);
                    }
                }

                return retval;
            }
        }

        public static void ClearCache()
        {
            AdminManagerCache.Clear();
        }

        public static void UpdateCache(AdministratorInfo adminInfo)
        {
            AdminManagerCache.Update(adminInfo);
        }

        public static void RemoveCache(AdministratorInfo adminInfo)
        {
            AdminManagerCache.Remove(adminInfo);
        }

        public static AdministratorInfo GetAdminInfoByUserId(int userId)
        {
            return AdminManagerCache.GetCacheByUserId(userId);
        }

        public static AdministratorInfo GetAdminInfoByUserName(string userName)
        {
            return AdminManagerCache.GetCacheByUserName(userName);
        }

        public static AdministratorInfo GetAdminInfoByMobile(string mobile)
        {
            return AdminManagerCache.GetCacheByMobile(mobile);
        }

        public static AdministratorInfo GetAdminInfoByEmail(string email)
        {
            return AdminManagerCache.GetCacheByEmail(email);
        }

        public static AdministratorInfo GetAdminInfoByAccount(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;

            if (StringUtils.IsMobile(account))
            {
                return GetAdminInfoByMobile(account);
            }
            if (StringUtils.IsEmail(account))
            {
                return GetAdminInfoByEmail(account);
            }

            return GetAdminInfoByUserName(account);
        }

        public static string GetDisplayName(string userName, bool isDepartment)
        {
            var adminInfo = GetAdminInfoByUserName(userName);
            if (adminInfo == null) return userName;

            if (!isDepartment) return adminInfo.DisplayName;
            var departmentName = DepartmentManager.GetDepartmentName(adminInfo.DepartmentId);
            return !string.IsNullOrEmpty(departmentName) ? $"{adminInfo.DisplayName}({departmentName})" : adminInfo.DisplayName;
        }

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
    }
}
