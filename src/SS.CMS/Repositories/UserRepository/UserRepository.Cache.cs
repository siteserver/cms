using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS;
using SS.CMS.Core;

namespace SS.CMS.Repositories
{
    public partial class UserRepository
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

        public async Task<bool> IsIpAddressCachedAsync(string ipAddress)
        {
            var config = await _configRepository.GetAsync();
            if (config.UserRegistrationMinMinutes == 0 || string.IsNullOrEmpty(ipAddress))
            {
                return true;
            }
            return CacheUtils.Exists($"SS.CMS.Provider.UserRepository.Insert.IpAddress.{ipAddress}");
        }

        public async Task CacheIpAddressAsync(string ipAddress)
        {
            var config = await _configRepository.GetAsync();
            if (config.UserRegistrationMinMinutes > 0 && !string.IsNullOrEmpty(ipAddress))
            {
                CacheUtils.InsertMinutes($"SS.CMS.Provider.UserRepository.Insert.IpAddress.{ipAddress}", ipAddress, config.UserRegistrationMinMinutes);
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

