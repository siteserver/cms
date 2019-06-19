using System.Collections.Generic;
using System.Linq;
using SS.CMS.Core.Common;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class TableStyleRepository
    {
        private readonly string CacheKey = StringUtils.GetCacheKey(nameof(TableStyleRepository));

        private void ClearCache()
        {
            _cacheManager.Remove(CacheKey);
        }

        public List<KeyValuePair<string, TableStyleInfo>> GetAllTableStyles()
        {
            var retval = _cacheManager.Get<List<KeyValuePair<string, TableStyleInfo>>>(CacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<List<KeyValuePair<string, TableStyleInfo>>>(CacheKey);
            if (retval == null)
            {
                retval = GetAllTableStyles();

                _cacheManager.Insert(CacheKey, retval);
            }

            return retval;
        }
    }
}