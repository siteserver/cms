using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(AdminManager));

            public static void Clear()
            {
                DataCacheManager.Remove(CacheKey);
            }

            public static void Update(Administrator adminInfo)
            {
                if (adminInfo == null) return;

                var dict = GetDict();

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

            public static void Remove(Administrator adminInfo)
            {
                if (adminInfo == null) return;

                var dict = GetDict();

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

            public static async Task<Administrator> GetCacheByUserIdAsync(int userId)
            {
                if (userId <= 0) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByUserId(userId), out Administrator adminInfo);
                if (adminInfo != null) return adminInfo;

                dict.TryGetValue(GetDictKeyByUserId(userId), out adminInfo);

                if (adminInfo == null)
                {
                    adminInfo = await DataProvider.AdministratorDao.GetByIdAsync(userId);
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

                return adminInfo;
            }

            public static async Task<Administrator> GetCacheByUserNameAsync(string userName)
            {
                if (string.IsNullOrEmpty(userName)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByUserName(userName), out Administrator adminInfo);
                if (adminInfo != null) return adminInfo;

                dict.TryGetValue(GetDictKeyByUserName(userName), out adminInfo);

                if (adminInfo == null)
                {
                    adminInfo = await DataProvider.AdministratorDao.GetByUserNameAsync(userName);
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

                return adminInfo;
            }

            public static async Task<Administrator> GetCacheByMobileAsync(string mobile)
            {
                if (string.IsNullOrEmpty(mobile)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByMobile(mobile), out Administrator adminInfo);
                if (adminInfo != null) return adminInfo;

                dict.TryGetValue(GetDictKeyByMobile(mobile), out adminInfo);

                if (adminInfo == null)
                {
                    adminInfo = await DataProvider.AdministratorDao.GetByMobileAsync(mobile);
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

                return adminInfo;
            }

            public static async Task<Administrator> GetCacheByEmailAsync(string email)
            {
                if (string.IsNullOrEmpty(email)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByEmail(email), out Administrator adminInfo);
                if (adminInfo != null) return adminInfo;

                dict.TryGetValue(GetDictKeyByEmail(email), out adminInfo);

                if (adminInfo == null)
                {
                    adminInfo = await DataProvider.AdministratorDao.GetByEmailAsync(email);
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

                return adminInfo;
            }

            private static Dictionary<string, Administrator> GetDict()
            {
                var dict = DataCacheManager.Get<Dictionary<string, Administrator>>(CacheKey);
                if (dict != null) return dict;

                dict = DataCacheManager.Get<Dictionary<string, Administrator>>(CacheKey);
                if (dict == null)
                {
                    dict = new Dictionary<string, Administrator>();
                    DataCacheManager.Insert(CacheKey, dict);
                }

                return dict;
            }
        }

        public static void ClearCache()
        {
            AdminManagerCache.Clear();
        }

        public static void UpdateCache(Administrator adminInfo)
        {
            AdminManagerCache.Update(adminInfo);
        }

        public static void RemoveCache(Administrator adminInfo)
        {
            AdminManagerCache.Remove(adminInfo);
        }

        public static async Task<Administrator> GetAdminInfoByUserIdAsync(int userId)
        {
            return await AdminManagerCache.GetCacheByUserIdAsync(userId);
        }

        public static async Task<Administrator> GetAdminInfoByUserNameAsync(string userName)
        {
            return await AdminManagerCache.GetCacheByUserNameAsync(userName);
        }

        public static async Task<Administrator> GetAdminInfoByMobileAsync(string mobile)
        {
            return await AdminManagerCache.GetCacheByMobileAsync(mobile);
        }

        public static async Task<Administrator> GetAdminInfoByEmailAsync(string email)
        {
            return await AdminManagerCache.GetCacheByEmailAsync(email);
        }

        public static async Task<Administrator> GetAdminInfoByAccountAsync(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;

            if (StringUtils.IsMobile(account))
            {
                return await GetAdminInfoByMobileAsync(account);
            }
            if (StringUtils.IsEmail(account))
            {
                return await GetAdminInfoByEmailAsync(account);
            }

            return await GetAdminInfoByUserNameAsync(account);
        }

        public static async Task<List<int>> GetLatestTop10SiteIdListAsync(List<int> siteIdListLatestAccessed, List<int> siteIdListWithPermissions)
        {
            var siteIdList = new List<int>();

            foreach (var siteId in siteIdListLatestAccessed)
            {
                if (siteIdList.Count >= 10) break;
                if (!siteIdList.Contains(siteId) && siteIdListWithPermissions.Contains(siteId))
                {
                    siteIdList.Add(siteId);
                }
            }

            if (siteIdList.Count < 10)
            {
                var siteIdListOrderByLevel = await SiteManager.GetSiteIdListOrderByLevelAsync();
                foreach (var siteId in siteIdListOrderByLevel)
                {
                    if (siteIdList.Count >= 5) break;
                    if (!siteIdList.Contains(siteId) && siteIdListWithPermissions.Contains(siteId))
                    {
                        siteIdList.Add(siteId);
                    }
                }
            }

            return siteIdList;
        }

        public static async Task<string> GetDisplayNameAsync(string userName)
        {
            var adminInfo = await GetAdminInfoByUserNameAsync(userName);
            return adminInfo == null ? userName : adminInfo.DisplayName;
        }

        public static string GetRoles(string userName)
        {
            var isConsoleAdministrator = false;
            var isSystemAdministrator = false;
            var roleNameList = new List<string>();
            var roles = DataProvider.AdministratorsInRolesDao.GetRolesForUser(userName);
            foreach (var role in roles)
            {
                if (!EPredefinedRoleUtils.IsPredefinedRole(role))
                {
                    roleNameList.Add(role);
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

            var roleNames = string.Empty;

            if (isConsoleAdministrator)
            {
                roleNames += EPredefinedRoleUtils.GetText(EPredefinedRole.ConsoleAdministrator);
            }
            else if (isSystemAdministrator)
            {
                roleNames += EPredefinedRoleUtils.GetText(EPredefinedRole.SystemAdministrator);
            }
            else
            {
                roleNames += TranslateUtils.ObjectCollectionToString(roleNameList);
            }
            return roleNames;
        }

        public static string GetUploadPath(params string[] paths)
        {
            var path = PathUtils.GetSiteFilesPath(DirectoryUtils.SiteFiles.Administrators, PathUtils.Combine(paths));
            DirectoryUtils.CreateDirectoryIfNotExists(path);
            return path;
        }

        public static string GetUserUploadPath(int userId, string relatedPath)
        {
            return GetUploadPath(userId.ToString(), relatedPath);
        }

        public static string GetUserUploadFileName(string filePath)
        {
            var dt = DateTime.Now;
            return $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{PathUtils.GetExtension(filePath)}";
        }

        public static string GetUploadUrl(params string[] paths)
        {
            return PageUtils.GetSiteFilesUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.Administrators, PageUtils.Combine(paths)));
        }

        public static string GetUserUploadUrl(int userId, string relatedUrl)
        {
            return GetUploadUrl(userId.ToString(), relatedUrl);
        }
    }
}
