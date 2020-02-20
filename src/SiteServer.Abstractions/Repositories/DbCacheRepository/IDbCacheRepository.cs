using System.Threading.Tasks;
using Datory;

namespace SiteServer.Abstractions
{
    public interface IDbCacheRepository : IRepository
    {
        Task RemoveAndInsertAsync(string cacheKey, string cacheValue);

        Task ClearAsync();

        Task<string> GetValueAndRemoveAsync(string cacheKey);
    }
}