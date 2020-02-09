using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Repositories
{
    public partial class AdministratorRepository
    {
        private string GetCacheKeyByUserId(int userId)
        {
            return Caching.GetEntityKey(TableName, "userId", userId.ToString());
        }

        private string GetCacheKeyByUserName(string userName)
        {
            return Caching.GetEntityKey(TableName, "userName", userName);
        }

        private string GetCacheKeyByMobile(string mobile)
        {
            return Caching.GetEntityKey(TableName, "mobile", mobile);
        }

        private string GetCacheKeyByEmail(string email)
        {
            return Caching.GetEntityKey(TableName, "email", email);
        }

        private List<string> GetCacheKeys(Administrator admin)
        {
            if (admin == null) return new List<string>();

            var keys = new List<string>
            {
                GetCacheKeyByUserId(admin.Id), 
                GetCacheKeyByUserName(admin.UserName)
            };

            if (!string.IsNullOrEmpty(admin.Mobile))
            {
                keys.Add(GetCacheKeyByMobile(admin.Mobile));
            }

            if (!string.IsNullOrEmpty(admin.Email))
            {
                keys.Add(GetCacheKeyByEmail(admin.Email));
            }

            return keys;
        }

        public async Task<Administrator> GetByAccountAsync(string account)
        {
            var admin = await GetByUserNameAsync(account);
            if (admin != null) return admin;
            if (StringUtils.IsMobile(account)) return await GetByMobileAsync(account);
            if (StringUtils.IsEmail(account)) return await GetByEmailAsync(account);

            return null;
        }

        public async Task<Administrator> GetByUserIdAsync(int userId)
        {
            if (userId <= 0) return null;

            var cacheKey = GetCacheKeyByUserId(userId);
            return await _repository.GetAsync(userId, Q.CachingGet(cacheKey));
        }

        public async Task<Administrator> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;

            var cacheKey = GetCacheKeyByUserName(userName);
            return await _repository.GetAsync(Q
                .Where(nameof(Administrator.UserName), userName)
                .CachingGet(cacheKey)
            );
        }

        public async Task<Administrator> GetByMobileAsync(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return null;

            var cacheKey = GetCacheKeyByMobile(mobile);
            return await _repository.GetAsync(Q
                .Where(nameof(Administrator.Mobile), mobile)
                .CachingGet(cacheKey)
            );
        }

        public async Task<Administrator> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var cacheKey = GetCacheKeyByEmail(email);
            return await _repository.GetAsync(Q
                .Where(nameof(Administrator.Email), email)
                .CachingGet(cacheKey)
            );
        }

        public async Task<string> GetDisplayNameAsync(int adminId)
        {
            var administrator = await GetByUserIdAsync(adminId);
            return administrator == null ? string.Empty :
                string.IsNullOrEmpty(administrator.DisplayName) ? administrator.UserName : administrator.DisplayName;
        }

        public async Task<List<int>> GetLatestTop10SiteIdListAsync(List<int> siteIdListLatestAccessed, List<int> siteIdListWithPermissions)
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
                var siteIdListOrderByLevel = await DataProvider.SiteRepository.GetSiteIdListOrderByLevelAsync();
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

        public async Task<string> GetRolesAsync(string userName)
        {
            var isConsoleAdministrator = false;
            var isSystemAdministrator = false;
            var roleNameList = new List<string>();
            var roles = await DataProvider.AdministratorsInRolesRepository.GetRolesForUserAsync(userName);
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
                roleNames += Utilities.ToString(roleNameList);
            }
            return roleNames;
        }

        private string GetUploadPath(params string[] paths)
        {
            var path = WebUtils.GetSiteFilesPath(DirectoryUtils.SiteFiles.Administrators, PathUtils.Combine(paths));
            DirectoryUtils.CreateDirectoryIfNotExists(path);
            return path;
        }

        public string GetUserUploadPath(int userId, string relatedPath)
        {
            return GetUploadPath(userId.ToString(), relatedPath);
        }

        public string GetUserUploadFileName(string filePath)
        {
            var dt = DateTime.Now;
            return $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{PathUtils.GetExtension(filePath)}";
        }

        private string GetUploadUrl(params string[] paths)
        {
            return PageUtils.GetSiteFilesUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.Administrators, PageUtils.Combine(paths)));
        }

        public string GetUserUploadUrl(int userId, string relatedUrl)
        {
            return GetUploadUrl(userId.ToString(), relatedUrl);
        }
    }
}
