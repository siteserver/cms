namespace SSCMS.Core.Services
{
    public partial class CacheManager<TCacheValue>
    {
        public TCacheValue Get(string key)
        {
            return _cacheManager.Get(key);
        }

        public bool Exists(string key)
        {
            return _cacheManager.Exists(key);
        }
    }
}
