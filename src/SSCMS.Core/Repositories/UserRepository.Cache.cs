﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SqlKata;
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

        private string GetCacheKeyByGuid(string guid)
        {
            return CacheUtils.GetEntityKey(TableName, "guid", guid);
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

        private string GetCacheKeyByOpenId(string openId)
        {
            return CacheUtils.GetEntityKey(TableName, "openId", openId);
        }

        private string[] GetCacheKeysToRemove(User user)
        {
            if (user == null) return null;

            var list = new List<string>
            {
                GetCacheKeyByUserId(user.Id),
                GetCacheKeyByGuid(user.Guid),
                GetCacheKeyByUserName(user.UserName),
            };

            if (!string.IsNullOrEmpty(user.Mobile))
            {
                list.Add(GetCacheKeyByMobile(user.Mobile));
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                list.Add(GetCacheKeyByEmail(user.Email));
            }

            if (!string.IsNullOrEmpty(user.OpenId))
            {
                list.Add(GetCacheKeyByOpenId(user.OpenId));
            }

            return list.ToArray();
        }

        public async Task<User> GetByAccountAsync(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;

            var user = await GetByUserNameAsync(account);
            if (user != null)
            {
                return user;
            }
            if (StringUtils.IsMobile(account))
            {
                return await GetByMobileAsync(account);
            }
            if (StringUtils.IsEmail(account))
            {
                return await GetByEmailAsync(account);
            }
            
            return await GetByOpenIdAsync(account);
        }
        
        private async Task<User> GetAsync(Query query)
        {
            var user = await _repository.GetAsync(query);

            if (user != null && string.IsNullOrEmpty(user.DisplayName))
            {
                user.DisplayName = user.UserName;
            }

            return user;
        }

        public async Task<User> GetByUserIdAsync(int userId)
        {
            if (userId <= 0) return null;

            return await GetAsync(Q
                .Where(nameof(User.Id), userId)
                .CachingGet(GetCacheKeyByUserId(userId))
            );
        }

        public async Task<User> GetByGuidAsync(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid)) return null;

            return await GetAsync(Q
                .Where(nameof(User.Guid), guid)
                .CachingGet(GetCacheKeyByGuid(guid))
            );
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;

            return await GetAsync(Q
                .Where(nameof(User.UserName), userName)
                .CachingGet(GetCacheKeyByUserName(userName))
            );
        }

        public async Task<User> GetByMobileAsync(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return null;

            return await GetAsync(Q
                .Where(nameof(User.Mobile), mobile)
                .CachingGet(GetCacheKeyByMobile(mobile))
            );
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            return await GetAsync(Q
                .Where(nameof(User.Email), email)
                .CachingGet(GetCacheKeyByEmail(email))
            );
        }

        public async Task<User> GetByOpenIdAsync(string openId)
        {
            if (string.IsNullOrWhiteSpace(openId)) return null;

            return await GetAsync(Q
                .Where(nameof(User.OpenId), openId)
                .CachingGet(GetCacheKeyByOpenId(openId))
            );
        }

        private static string GetIpAddressCacheKey(string ipAddress)
        {
            return $"{nameof(UserRepository)}:{ipAddress}";
        }

        private async Task<bool> IsIpAddressCachedAsync(string ipAddress)
        {
            var config = await _configRepository.GetAsync();
            if (config.UserRegistrationMinMinutes == 0 || string.IsNullOrEmpty(ipAddress))
            {
                return false;
            }

            return _cacheManager.Exists(GetIpAddressCacheKey(ipAddress));
        }

        private async Task CacheIpAddressAsync(string ipAddress)
        {
            var config = await _configRepository.GetAsync();
            if (config.UserRegistrationMinMinutes > 0 && !string.IsNullOrEmpty(ipAddress))
            {
                _cacheManager.AddOrUpdateAbsolute(GetIpAddressCacheKey(ipAddress), true, config.UserRegistrationMinMinutes);
            }
        }

        public async Task<string> GetDisplayAsync(int userId)
        {
            if (userId <= 0) return string.Empty;

            var user = await GetByUserIdAsync(userId);
            return GetDisplay(user);
        }

        public string GetDisplay(User user)
        {
            if (user == null) return string.Empty;

            return string.IsNullOrEmpty(user.DisplayName) || user.UserName == user.DisplayName ? user.UserName : $"{user.DisplayName}({user.UserName})";
        }
    }
}

