using System;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class CacheManager
    {
        public static void RemoveByClassName(string className)
        {
            RemoveByStartString(StringUtils.GetCacheKey(className));
        }

        public static void RemoveByPrefix(string prefix)
        {
            RemoveByStartString(prefix);
        }
    }
}
