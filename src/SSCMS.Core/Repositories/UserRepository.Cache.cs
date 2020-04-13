using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class UserRepository
    {
        private string GetCacheKeyByUserId(int userId)
        {
            return CacheUtils.GetEntityKey(TableName, "userId", userId.ToString());
        }

        private string GetCacheKeyByUserName(string userName)
        {
            return CacheUtils.GetEntityKey(TableName, "userName", userName);
        }

        private string GetCacheKeyByMobile(string mobile)
        {
            return CacheUtils.GetEntityKey(TableName, "mobile", mobile);
        }

        private string GetCacheKeyByEmail(string email)
        {
            return CacheUtils.GetEntityKey(TableName, "email", email);
        }

        private string[] GetCacheKeysToRemove(User user)
        {
            if (user == null) return null;

            var list = new List<string>
            {
                GetCacheKeyByUserId(user.Id), 
                GetCacheKeyByUserName(user.UserName)
            };

            if (!string.IsNullOrEmpty(user.Mobile))
            {
                list.Add(GetCacheKeyByMobile(user.Mobile));
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                list.Add(GetCacheKeyByEmail(user.Email));
            }

            return list.ToArray();
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

            return await _repository.GetAsync(userId, Q
                .CachingGet(GetCacheKeyByUserId(userId))
            );
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;

            return await _repository.GetAsync(Q
                .Where(nameof(User.UserName), userName)
                .CachingGet(GetCacheKeyByUserName(userName))
            );
        }

        public async Task<User> GetByMobileAsync(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return null;

            return await _repository.GetAsync(Q
                .Where(nameof(User.Mobile), mobile)
                .CachingGet(GetCacheKeyByMobile(mobile))
            );
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            return await _repository.GetAsync(Q
                .Where(nameof(User.Email), email)
                .CachingGet(GetCacheKeyByEmail(email))
            );
        }

        private static string GetIpAddressCacheKey(string ipAddress)
        {
            return $"{nameof(UserRepository)}:{ipAddress}";
        }

        public async Task<bool> IsIpAddressCachedAsync(string ipAddress)
        {
            var config = await _configRepository.GetAsync();
            if (config.UserRegistrationMinMinutes == 0 || string.IsNullOrEmpty(ipAddress))
            {
                return true;
            }

            var cacheManager = await _repository.GetCacheManagerAsync();
            return cacheManager.Exists(GetIpAddressCacheKey(ipAddress));
        }

        public async Task CacheIpAddressAsync(string ipAddress)
        {
            var config = await _configRepository.GetAsync();
            if (config.UserRegistrationMinMinutes > 0 && !string.IsNullOrEmpty(ipAddress))
            {
                var cacheManager = await _repository.GetCacheManagerAsync();
                var value = new CacheItem<object>(GetIpAddressCacheKey(ipAddress), true, ExpirationMode.Sliding, TimeSpan.FromMinutes(config.UserRegistrationMinMinutes));

                cacheManager.AddOrUpdate(value, _ => value);
            }
        }


        public async Task<string> GetDisplayAsync(int userId)
        {
            if (userId <= 0) return string.Empty;

            var user = await GetByUserIdAsync(userId);
            if (user != null)
            {
                return string.IsNullOrEmpty(user.DisplayName) || user.UserName == user.DisplayName ? user.UserName : $"{user.DisplayName}({user.UserName})";
            }

            return string.Empty;
        }
    }
}

