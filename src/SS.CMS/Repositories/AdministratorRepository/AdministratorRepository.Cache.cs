using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS;
using SS.CMS.Core;

namespace SS.CMS.Repositories
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

        

        public string GetUserUploadFileName(string filePath)
        {
            var dt = DateTime.Now;
            return $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{PathUtils.GetExtension(filePath)}";
        }

        

        

        public async Task<string> GetDisplayAsync(int userId)
        {
            if (userId <= 0) return string.Empty;

            var admin = await GetByUserIdAsync(userId);
            if (admin != null)
            {
                return string.IsNullOrEmpty(admin.DisplayName) || admin.UserName == admin.DisplayName ? admin.UserName : $"{admin.DisplayName}({admin.UserName})";
            }

            return string.Empty;
        }
    }
}
