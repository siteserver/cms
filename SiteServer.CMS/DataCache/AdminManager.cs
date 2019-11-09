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

            public static void Update(Administrator administrator)
            {
                if (administrator == null) return;

                var dict = GetDict();

                dict[GetDictKeyByUserId(administrator.Id)] = administrator;
                dict[GetDictKeyByUserName(administrator.UserName)] = administrator;
                if (!string.IsNullOrEmpty(administrator.Mobile))
                {
                    dict[GetDictKeyByMobile(administrator.Mobile)] = administrator;
                }
                if (!string.IsNullOrEmpty(administrator.Email))
                {
                    dict[GetDictKeyByEmail(administrator.Email)] = administrator;
                }
            }

            public static void Remove(Administrator administrator)
            {
                if (administrator == null) return;

                var dict = GetDict();

                dict.Remove(GetDictKeyByUserId(administrator.Id));
                dict.Remove(GetDictKeyByUserName(administrator.UserName));
                if (!string.IsNullOrEmpty(administrator.Mobile))
                {
                    dict.Remove(GetDictKeyByMobile(administrator.Mobile));
                }

                if (!string.IsNullOrEmpty(administrator.Email))
                {
                    dict.Remove(GetDictKeyByEmail(administrator.Email));
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

                dict.TryGetValue(GetDictKeyByUserId(userId), out var administrator);
                if (administrator != null) return administrator;

                dict.TryGetValue(GetDictKeyByUserId(userId), out administrator);

                if (administrator == null)
                {
                    administrator = await DataProvider.AdministratorDao.GetByIdAsync(userId);
                    if (administrator != null)
                    {
                        dict[GetDictKeyByUserId(administrator.Id)] = administrator;
                        dict[GetDictKeyByUserName(administrator.UserName)] = administrator;
                        if (!string.IsNullOrEmpty(administrator.Mobile))
                        {
                            dict[GetDictKeyByMobile(administrator.Mobile)] = administrator;
                        }
                        if (!string.IsNullOrEmpty(administrator.Email))
                        {
                            dict[GetDictKeyByEmail(administrator.Email)] = administrator;
                        }
                    }
                }

                return administrator;
            }

            public static async Task<Administrator> GetCacheByUserNameAsync(string userName)
            {
                if (string.IsNullOrEmpty(userName)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByUserName(userName), out Administrator administrator);
                if (administrator != null) return administrator;

                dict.TryGetValue(GetDictKeyByUserName(userName), out administrator);

                if (administrator == null)
                {
                    administrator = await DataProvider.AdministratorDao.GetByUserNameAsync(userName);
                    if (administrator != null)
                    {
                        dict[GetDictKeyByUserId(administrator.Id)] = administrator;
                        dict[GetDictKeyByUserName(administrator.UserName)] = administrator;
                        if (!string.IsNullOrEmpty(administrator.Mobile))
                        {
                            dict[GetDictKeyByMobile(administrator.Mobile)] = administrator;
                        }
                        if (!string.IsNullOrEmpty(administrator.Email))
                        {
                            dict[GetDictKeyByEmail(administrator.Email)] = administrator;
                        }
                    }
                }

                return administrator;
            }

            public static async Task<Administrator> GetCacheByMobileAsync(string mobile)
            {
                if (string.IsNullOrEmpty(mobile)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByMobile(mobile), out Administrator administrator);
                if (administrator != null) return administrator;

                dict.TryGetValue(GetDictKeyByMobile(mobile), out administrator);

                if (administrator == null)
                {
                    administrator = await DataProvider.AdministratorDao.GetByMobileAsync(mobile);
                    if (administrator != null)
                    {
                        dict[GetDictKeyByUserId(administrator.Id)] = administrator;
                        dict[GetDictKeyByUserName(administrator.UserName)] = administrator;
                        if (!string.IsNullOrEmpty(administrator.Mobile))
                        {
                            dict[GetDictKeyByMobile(administrator.Mobile)] = administrator;
                        }
                        if (!string.IsNullOrEmpty(administrator.Email))
                        {
                            dict[GetDictKeyByEmail(administrator.Email)] = administrator;
                        }
                    }
                }

                return administrator;
            }

            public static async Task<Administrator> GetCacheByEmailAsync(string email)
            {
                if (string.IsNullOrEmpty(email)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByEmail(email), out Administrator administrator);
                if (administrator != null) return administrator;

                dict.TryGetValue(GetDictKeyByEmail(email), out administrator);

                if (administrator == null)
                {
                    administrator = await DataProvider.AdministratorDao.GetByEmailAsync(email);
                    if (administrator != null)
                    {
                        dict[GetDictKeyByUserId(administrator.Id)] = administrator;
                        dict[GetDictKeyByUserName(administrator.UserName)] = administrator;
                        if (!string.IsNullOrEmpty(administrator.Mobile))
                        {
                            dict[GetDictKeyByMobile(administrator.Mobile)] = administrator;
                        }
                        if (!string.IsNullOrEmpty(administrator.Email))
                        {
                            dict[GetDictKeyByEmail(administrator.Email)] = administrator;
                        }
                    }
                }

                return administrator;
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

        public static void UpdateCache(Administrator administrator)
        {
            AdminManagerCache.Update(administrator);
        }

        public static void RemoveCache(Administrator administrator)
        {
            AdminManagerCache.Remove(administrator);
        }

        public static async Task<Administrator> GetByUserIdAsync(int userId)
        {
            return await AdminManagerCache.GetCacheByUserIdAsync(userId);
        }

        public static async Task<Administrator> GetByUserNameAsync(string userName)
        {
            return await AdminManagerCache.GetCacheByUserNameAsync(userName);
        }

        public static async Task<Administrator> GetByMobileAsync(string mobile)
        {
            return await AdminManagerCache.GetCacheByMobileAsync(mobile);
        }

        public static async Task<Administrator> GetByEmailAsync(string email)
        {
            return await AdminManagerCache.GetCacheByEmailAsync(email);
        }

        public static async Task<Administrator> GetByAccountAsync(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;

            if (StringUtils.IsMobile(account))
            {
                return await GetByMobileAsync(account);
            }
            if (StringUtils.IsEmail(account))
            {
                return await GetByEmailAsync(account);
            }

            return await GetByUserNameAsync(account);
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
            var administrator = await GetByUserNameAsync(userName);
            return administrator == null ? userName : administrator.DisplayName;
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

        private static string GetUploadPath(params string[] paths)
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

        private static string GetUploadUrl(params string[] paths)
        {
            return PageUtils.GetSiteFilesUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.Administrators, PageUtils.Combine(paths)));
        }

        public static string GetUserUploadUrl(int userId, string relatedUrl)
        {
            return GetUploadUrl(userId.ToString(), relatedUrl);
        }
    }
}
