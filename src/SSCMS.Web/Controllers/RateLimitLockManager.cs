using System.Collections.Concurrent;

namespace SSCMS.Web.Controllers
{
    internal static class RateLimitLockManager
    {
        private static readonly ConcurrentDictionary<string, object> Locks = new ConcurrentDictionary<string, object>();

        public static object GetLock(string cacheKey)
        {
            return Locks.GetOrAdd(cacheKey ?? string.Empty, _ => new object());
        }
    }
}
