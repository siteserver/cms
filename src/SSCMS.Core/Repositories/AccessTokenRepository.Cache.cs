using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class AccessTokenRepository
    {
        private string GetCacheKeyByToken(string token)
        {
            return CacheUtils.GetEntityKey(TableName, "token", token);
        }

        public async Task<AccessToken> GetByTokenAsync(string token)
        {
            var cacheKey = GetCacheKeyByToken(token);

            return await _repository.GetAsync(Q
                .Where(nameof(AccessToken.Token),
                    _settingsManager.Encrypt(token))
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
