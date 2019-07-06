using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace SS.CMS.Core.Repositories
{
    public partial class UserRepository
    {
        public async Task RemoveCacheAsync(UserInfo userInfo)
        {
            if (userInfo == null) return;

            var cacheKey = GetCacheKeyByUserId(userInfo.Id);
            await _cache.RemoveAsync(cacheKey);

            cacheKey = GetCacheKeyByUserName(userInfo.UserName);
            await _cache.RemoveAsync(cacheKey);

            if (!string.IsNullOrEmpty(userInfo.Mobile))
            {
                cacheKey = GetCacheKeyByMobile(userInfo.Mobile);
                await _cache.RemoveAsync(cacheKey);
            }

            if (!string.IsNullOrEmpty(userInfo.Email))
            {
                cacheKey = GetCacheKeyByEmail(userInfo.Email);
                await _cache.RemoveAsync(cacheKey);
            }
        }

        private string GetCacheKeyByUserId(int userId)
        {
            return _cache.GetKey(nameof(UserRepository), nameof(GetCacheKeyByUserId), userId.ToString());
        }

        private string GetCacheKeyByUserName(string userName)
        {
            return _cache.GetKey(nameof(UserRepository), nameof(GetCacheKeyByUserName), userName);
        }

        private string GetCacheKeyByMobile(string mobile)
        {
            return _cache.GetKey(nameof(UserRepository), nameof(GetCacheKeyByMobile), mobile);
        }

        private string GetCacheKeyByEmail(string email)
        {
            return _cache.GetKey(nameof(UserRepository), nameof(GetCacheKeyByEmail), email);
        }

        public List<int> GetLatestTop10SiteIdList(List<int> siteIdListLatestAccessed, List<int> siteIdListOrderByLevel, List<int> siteIdListWithPermissions)
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
                // var siteIdListOrderByLevel = SiteManager.GetSiteIdListOrderByLevel();
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

        public async Task<string> GetDisplayNameAsync(string userName)
        {
            var userInfo = await GetByUserNameAsync(userName);
            if (userInfo == null) return userName;

            return userInfo.DisplayName;
        }
    }
}