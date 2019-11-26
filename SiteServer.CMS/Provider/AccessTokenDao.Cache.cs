using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public partial class AccessTokenDao
    {
        private readonly string _cacheKey = CacheManager.Cache.GetKey(nameof(AccessToken));

        private async Task RemoveCacheAsync()
        {
            await CacheManager.Cache.RemoveAsync(_cacheKey);
        }

        public async Task<bool> IsScopeAsync(string token, string scope)
        {
            if (string.IsNullOrEmpty(token)) return false;

            var tokenInfo = await GetAccessTokenInfoAsync(token);
            return tokenInfo != null && StringUtils.ContainsIgnoreCase(TranslateUtils.StringCollectionToStringList(tokenInfo.Scopes), scope);
        }

        public async Task<AccessToken> GetAccessTokenInfoAsync(string token)
        {
            AccessToken tokenInfo = null;
            var dict = await GetAccessTokenDictionaryAsync();

            if (dict != null && dict.ContainsKey(token))
            {
                tokenInfo = dict[token];
            }
            return tokenInfo;
        }

        private async Task<Dictionary<string, AccessToken>> GetAccessTokenDictionaryAsync()
        {
            return await
                CacheManager.Cache.GetOrCreateAsync(_cacheKey, async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromHours(1);

                    var dictionary = new Dictionary<string, AccessToken>();

                    foreach (var accessTokenInfo in await _repository.GetAllAsync())
                    {
                        var token = WebConfigUtils.DecryptStringBySecretKey(accessTokenInfo.Token);
                        if (!string.IsNullOrEmpty(token))
                        {
                            dictionary[token] = accessTokenInfo;
                        }
                    }

                    return dictionary;
                });
        }
    }
}
