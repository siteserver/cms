using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using Datory;

namespace SiteServer.CMS.Provider
{
    public partial class AccessTokenDao
    {
        private string GetCacheKeyByToken(string token)
        {
            return _cache.GetEntityKey(this, "token", token);
        }

        private async Task RemoveCacheAsync(string token)
        {
            var cacheKey = GetCacheKeyByToken(token);
            await _cache.RemoveAsync(cacheKey);
        }

        public async Task<AccessToken> GetByTokenAsync(string token)
        {
            var cacheKey = GetCacheKeyByToken(token);
            return await
                _cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    var accessToken = await _repository.GetAsync(Q
                        .Where(nameof(AccessToken.Token), WebConfigUtils.EncryptStringBySecretKey(token)) 
                    );

                    return accessToken;
                });
        }
    }
}
