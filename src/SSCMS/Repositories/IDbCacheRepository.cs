using System.Threading.Tasks;
using Datory;

namespace SSCMS.Repositories
{
    public interface IDbCacheRepository : IRepository
    {
        Task RemoveAndInsertAsync(string cacheKey, string cacheValue);

        Task ClearAsync();

        Task<string> GetValueAndRemoveAsync(string cacheKey);

        Task<string> GetValueAsync(string cacheKey);
    }
}