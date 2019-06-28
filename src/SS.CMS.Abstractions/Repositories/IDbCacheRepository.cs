using System.Threading.Tasks;
using SS.CMS.Data;

namespace SS.CMS.Repositories
{
    public interface IDbCacheRepository : IRepository
    {
        Task RemoveAndInsertAsync(string cacheKey, string cacheValue);

        Task ClearAsync();

        bool IsExists(string cacheKey);

        string GetValue(string cacheKey);

        Task<string> GetValueAndRemoveAsync(string cacheKey);

        int GetCount();

        Task DeleteExcess90DaysAsync();
    }
}
