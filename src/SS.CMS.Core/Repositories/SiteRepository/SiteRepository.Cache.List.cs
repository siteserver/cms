using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;

namespace SS.CMS.Core.Repositories
{
    public partial class SiteRepository
    {
        [Serializable]
        private class Cache
        {
            public int Id { get; set; }
            public string SiteName { get; set; }
            public string SiteDir { get; set; }
            public string TableName { get; set; }
            public bool IsRoot { get; set; }
            public int ParentId { get; set; }
            public int Taxis { get; set; }
        }

        private async Task RemoveCacheListAsync()
        {
            var cacheKey = _cache.GetListKey(this);
            await _cache.RemoveAsync(cacheKey);
        }

        private async Task<List<Cache>> GetCacheListAsync()
        {
            var cacheKey = _cache.GetListKey(this);
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                var siteInfoList = await _repository.GetAllAsync(Q
                    .Select(Attr.Id, Attr.SiteName, Attr.SiteDir, Attr.TableName, Attr.IsRoot, Attr.ParentId, Attr.Taxis)
                    .OrderBy(Attr.Taxis, Attr.Id)
                );

                var cacheList = new List<Cache>();
                Cache rootCache = null;
                foreach (var siteInfo in siteInfoList)
                {
                    var cache = new Cache
                    {
                        Id = siteInfo.Id,
                        SiteName = siteInfo.SiteName,
                        SiteDir = siteInfo.SiteDir,
                        TableName = siteInfo.TableName,
                        IsRoot = siteInfo.IsRoot,
                        ParentId = siteInfo.ParentId,
                        Taxis = siteInfo.Taxis
                    };

                    if (cache.IsRoot)
                    {
                        rootCache = cache;
                    }
                    else
                    {
                        cacheList.Add(cache);
                    }
                }
                if (rootCache != null)
                {
                    cacheList.Insert(0, rootCache);
                }

                return cacheList;
            });
        }

        private async Task<List<Cache>> GetCacheListAsync(int parentId)
        {
            var cacheList = await GetCacheListAsync();
            return cacheList.Where(x => x.ParentId == parentId).ToList();
        }
    }
}