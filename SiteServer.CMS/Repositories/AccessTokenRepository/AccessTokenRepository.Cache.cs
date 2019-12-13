using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;

namespace SiteServer.CMS.Repositories
{
    public partial class AccessTokenRepository
    {
        private string GetCacheKeyByToken(string token)
        {
            return CacheManager.GetEntityKey(TableName, "token", token);
        }

        //private async Task RemoveCacheAsync(string token)
        //{
        //    var cacheKey = GetCacheKeyByToken(token);
        //    await _cache.RemoveAsync(cacheKey);
        //}

        public async Task<AccessToken> GetByTokenAsync(string token)
        {
            var cacheKey = GetCacheKeyByToken(token);

            return await _repository.GetAsync(Q
                .Where(nameof(AccessToken.Token), WebConfigUtils.EncryptStringBySecretKey(token))
                .CachingGet(cacheKey)
            );

            //var cacheKey = GetCacheKeyByToken(token);
            //return await
            //    _cache.GetOrCreateAsync(cacheKey, async entry =>
            //    {
            //        var accessToken = await _repository.GetAsync(Q
            //            .Where(nameof(AccessToken.Token), WebConfigUtils.EncryptStringBySecretKey(token)) 
            //        );

            //        return accessToken;
            //    });
        }
    }
}
