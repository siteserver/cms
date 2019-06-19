using System;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class CacheManager
    {
        public void RemoveByClassName(string className)
        {
            RemoveByStartString(StringUtils.GetCacheKey(className));
        }

        public void RemoveByPrefix(string prefix)
        {
            RemoveByStartString(prefix);
        }
    }
}
