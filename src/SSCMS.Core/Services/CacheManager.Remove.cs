namespace SSCMS.Core.Services
{
    public partial class CacheManager<TCacheValue>
    {
        public void Remove(string key)
        {
            _cacheManager.Remove(key);
        }

        public void Clear()
        {
            _cacheManager.Clear();
        }
    }
}
