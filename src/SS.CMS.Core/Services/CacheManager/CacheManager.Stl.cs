using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class CacheManager
    {
        public int GetInt(string cacheKey)
        {
            return Exists(cacheKey) ? Get<int>(cacheKey) : -1;
        }

        public void Set(string cacheKey, object value)
        {
            InsertMinutes(cacheKey, value, 2);
        }

        public void Clear(string nameofClass)
        {
            RemoveByStartString(StringUtils.GetCacheKey(nameofClass, string.Empty));
        }
    }
}
