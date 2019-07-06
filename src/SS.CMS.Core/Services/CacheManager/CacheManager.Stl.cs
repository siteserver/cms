using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class CacheManager
    {
        public static int GetInt(string cacheKey)
        {
            return Exists(cacheKey) ? TranslateUtils.ToInt(Get(cacheKey).ToString()) : -1;
        }

        public static void Set(string cacheKey, object value)
        {
            InsertMinutes(cacheKey, value, 2);
        }

        public static void Clear(string nameofClass)
        {
            RemoveByStartString(StringUtils.GetCacheKey(nameofClass, string.Empty));
        }
    }
}
