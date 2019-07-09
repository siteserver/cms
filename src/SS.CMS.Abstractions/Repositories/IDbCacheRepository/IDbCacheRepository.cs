using System.Threading.Tasks;
using SS.CMS.Data;

namespace SS.CMS.Repositories
{
    public interface IDbCacheRepository : IRepository
    {
        Task RemoveAndInsertAsync(string cacheKey, string cacheValue);

        Task ClearAsync();

        Task<bool> IsExistsAsync(string cacheKey);

        Task<string> GetValueAsync(string cacheKey);

        Task<string> GetValueAndRemoveAsync(string cacheKey);

        Task<int> GetCountAsync();

        Task DeleteExcess90DaysAsync();
    }
}
