using System;
using System.Threading.Tasks;
using Datory;

namespace SSCMS.Repositories
{
    public interface IDbCacheRepository : IRepository
    {
        Task RemoveAndInsertAsync(string cacheKey, string cacheValue);

        Task RemoveAsync(string cacheKey);

        Task ClearAsync();

        Task<string> GetValueAndRemoveAsync(string cacheKey);

        Task<string> GetValueAsync(string cacheKey);

        Task<(string, DateTime)> GetValueAndCreatedDateAsync(string cacheKey);
    }
}