using SS.CMS.Data;

namespace SS.CMS.Repositories
{
    public interface IDbCacheRepository : IRepository
    {
        void RemoveAndInsert(string cacheKey, string cacheValue);

        void Clear();

        bool IsExists(string cacheKey);

        string GetValue(string cacheKey);

        string GetValueAndRemove(string cacheKey);

        int GetCount();

        void DeleteExcess90Days();
    }
}
