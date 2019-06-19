using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlKata;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class TagRepository
    {
        public SortedList ReadContent(int siteId)
        {
            var cacheKey = "SiteServer.CMS.Core.WordSpliter." + siteId;
            if (!_cacheManager.Exists(cacheKey))
            {
                var arrText = new SortedList();

                var tagList = GetTagList(siteId);
                if (tagList.Count > 0)
                {
                    foreach (var line in tagList)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            arrText.Add(line.Trim(), line.Trim());
                        }
                    }
                }
                _cacheManager.InsertHours(cacheKey, arrText, 1);
            }
            return (SortedList)_cacheManager.Get(cacheKey);
        }
    }
}