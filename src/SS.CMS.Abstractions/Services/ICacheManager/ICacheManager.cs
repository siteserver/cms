using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace SS.CMS.Services.ICacheManager
{
    public partial interface ICacheManager
    {
        void Remove(string key);

        T Get<T>(string key);

        Task<T> GetOrCreateAsync<T>(string key, Func<ICacheEntry, Task<T>> factory);

        bool TryGetValue<T>(string key, out T value);
    }
}
