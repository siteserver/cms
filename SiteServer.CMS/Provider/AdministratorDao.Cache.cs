using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public partial class AdministratorDao
    {
        private string GetCacheKeyByUserId(int userId)
        {
            return _cache.GetEntityKey(this, "userId", userId.ToString());
        }

        private string GetCacheKeyByUserName(string userName)
        {
            return _cache.GetEntityKey(this, "userName", userName);
        }

        private string GetCacheKeyByMobile(string mobile)
        {
            return _cache.GetEntityKey(this, "mobile", mobile);
        }

        private string GetCacheKeyByEmail(string email)
        {
            return _cache.GetEntityKey(this, "email", email);
        }

        private async Task RemoveCacheAsync(Administrator admin)
        {
            if (admin == null) return;

            var cacheKey = GetCacheKeyByUserId(admin.Id);
            await _cache.RemoveAsync(cacheKey);

            cacheKey = GetCacheKeyByUserName(admin.UserName);
            await _cache.RemoveAsync(cacheKey);

            if (!string.IsNullOrEmpty(admin.Mobile))
            {
                cacheKey = GetCacheKeyByMobile(admin.Mobile);
                await _cache.RemoveAsync(cacheKey);
            }

            if (!string.IsNullOrEmpty(admin.Email))
            {
                cacheKey = GetCacheKeyByEmail(admin.Email);
                await _cache.RemoveAsync(cacheKey);
            }
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
            return await _cache.GetOrCreateAsync(cacheKey, async options => await _repository.GetAsync(userId));
        }

        public async Task<Administrator> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;

            var cacheKey = GetCacheKeyByUserName(userName);
            return await _cache.GetOrCreateAsync(cacheKey, async options => await _repository.GetAsync(Q
                .Where(nameof(Administrator.UserName), userName)
            ));
        }

        public async Task<Administrator> GetByMobileAsync(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return null;

            var cacheKey = GetCacheKeyByMobile(mobile);
            return await _cache.GetOrCreateAsync(cacheKey, async options => await _repository.GetAsync(Q
                .Where(nameof(Administrator.Mobile), mobile)
            ));
        }

        public async Task<Administrator> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var cacheKey = GetCacheKeyByEmail(email);
            return await _cache.GetOrCreateAsync(cacheKey, async options => await _repository.GetAsync(Q
                .Where(nameof(Administrator.Email), email)
            ));
        }

        public async Task<string> GetDisplayNameAsync(string userName)
        {
            var administrator = await GetByUserNameAsync(userName);
            return administrator == null ? userName :
                string.IsNullOrEmpty(administrator.DisplayName) ? userName : administrator.DisplayName;
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
                var siteIdListOrderByLevel = await DataProvider.SiteDao.GetSiteIdListOrderByLevelAsync();
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
            var roles = await DataProvider.AdministratorsInRolesDao.GetRolesForUserAsync(userName);
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
