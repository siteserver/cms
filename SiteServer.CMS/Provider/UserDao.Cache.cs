using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.CMS.DataCache;
using SqlKata;

namespace SiteServer.CMS.Provider
{
    public partial class UserDao
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

        private async Task RemoveCacheAsync(User user)
        {
            if (user == null) return;

            var cacheKey = GetCacheKeyByUserId(user.Id);
            await _cache.RemoveAsync(cacheKey);

            cacheKey = GetCacheKeyByUserName(user.UserName);
            await _cache.RemoveAsync(cacheKey);

            if (!string.IsNullOrEmpty(user.Mobile))
            {
                cacheKey = GetCacheKeyByMobile(user.Mobile);
                await _cache.RemoveAsync(cacheKey);
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                cacheKey = GetCacheKeyByEmail(user.Email);
                await _cache.RemoveAsync(cacheKey);
            }
        }

        public async Task<User> GetByAccountAsync(string account)
        {
            var user = await GetByUserNameAsync(account);
            if (user != null) return user;
            if (StringUtils.IsMobile(account)) return await GetByMobileAsync(account);
            if (StringUtils.IsEmail(account)) return await GetByEmailAsync(account);

            return null;
        }

        public async Task<User> GetByUserIdAsync(int userId)
        {
            if (userId <= 0) return null;

            var cacheKey = GetCacheKeyByUserId(userId);
            return await _cache.GetOrCreateAsync(cacheKey, async options => await _repository.GetAsync(userId));
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;

            var cacheKey = GetCacheKeyByUserName(userName);
            return await _cache.GetOrCreateAsync(cacheKey, async options => await _repository.GetAsync(Q
                .Where(nameof(User.UserName), userName)
            ));
        }

        public async Task<User> GetByMobileAsync(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return null;

            var cacheKey = GetCacheKeyByMobile(mobile);
            return await _cache.GetOrCreateAsync(cacheKey, async options => await _repository.GetAsync(Q
                .Where(nameof(User.Mobile), mobile)
            ));
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var cacheKey = GetCacheKeyByEmail(email);
            return await _cache.GetOrCreateAsync(cacheKey, async options => await _repository.GetAsync(Q
                .Where(nameof(User.Email), email)
            ));
        }

        public async Task<bool> IsIpAddressCachedAsync(string ipAddress)
        {
            var config = await DataProvider.ConfigDao.GetAsync();
            if (config.UserRegistrationMinMinutes == 0 || string.IsNullOrEmpty(ipAddress))
            {
                return true;
            }
            var obj = CacheUtils.Get($"SiteServer.CMS.Provider.UserDao.Insert.IpAddress.{ipAddress}");
            return obj == null;
        }

        public async Task CacheIpAddressAsync(string ipAddress)
        {
            var config = await DataProvider.ConfigDao.GetAsync();
            if (config.UserRegistrationMinMinutes > 0 && !string.IsNullOrEmpty(ipAddress))
            {
                CacheUtils.InsertMinutes($"SiteServer.CMS.Provider.UserDao.Insert.IpAddress.{ipAddress}", ipAddress, config.UserRegistrationMinMinutes);
            }
        }

        public string GetHomeUploadPath(params string[] paths)
        {

            var path = WebUtils.GetSiteFilesPath(DirectoryUtils.SiteFiles.Home, PathUtils.Combine(paths));
            DirectoryUtils.CreateDirectoryIfNotExists(path);
            return path;
        }

        public string GetUserUploadPath(int userId, string relatedPath)
        {
            return GetHomeUploadPath(userId.ToString(), relatedPath);
        }

        public string GetUserUploadFileName(string filePath)
        {
            var dt = DateTime.Now;
            return $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{PathUtils.GetExtension(filePath)}";
        }

        public string GetHomeUploadUrl(params string[] paths)
        {
            return PageUtils.GetSiteFilesUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.Home, PageUtils.Combine(paths)));
        }

        public string DefaultAvatarUrl => GetHomeUploadUrl("default_avatar.png");

        public string GetUserUploadUrl(int userId, string relatedUrl)
        {
            return GetHomeUploadUrl(userId.ToString(), relatedUrl);
        }

        public string GetUserAvatarUrl(User user)
        {
            var imageUrl = user?.AvatarUrl;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.IsProtocolUrl(imageUrl) ? imageUrl : GetUserUploadUrl(user.Id, imageUrl);
            }

            return DefaultAvatarUrl;
        }
    }
}

