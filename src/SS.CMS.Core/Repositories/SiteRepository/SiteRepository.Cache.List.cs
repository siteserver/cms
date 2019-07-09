using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;

namespace SS.CMS.Core.Repositories
{
    public partial class SiteRepository
    {
        [Serializable]
        private class CacheInfo
        {
            public int Id { get; set; }
            public string SiteName { get; set; }
            public string SiteDir { get; set; }
            public string TableName { get; set; }
            public bool IsRoot { get; set; }
            public int ParentId { get; set; }
            public int Taxis { get; set; }
        }

        private async Task RemoveListCacheAsync()
        {
            var cacheKey = _cache.GetListKey(this);
            await _cache.RemoveAsync(cacheKey);
        }

        private async Task<List<CacheInfo>> GetListCacheAsync()
        {
            var cacheKey = _cache.GetListKey(this);
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                var siteInfoList = await _repository.GetAllAsync(Q
                .Select(Attr.Id, Attr.SiteName, Attr.SiteDir, Attr.TableName, Attr.IsRoot, Attr.ParentId, Attr.Taxis)
                .OrderBy(Attr.IsRoot, Attr.Taxis, Attr.Id));

                var cacheInfoList = new List<CacheInfo>();
                foreach (var siteInfo in siteInfoList)
                {
                    cacheInfoList.Add(new CacheInfo
                    {
                        Id = siteInfo.Id,
                        SiteName = siteInfo.SiteName,
                        SiteDir = siteInfo.SiteDir,
                        TableName = siteInfo.TableName,
                        IsRoot = siteInfo.IsRoot,
                        ParentId = siteInfo.ParentId,
                        Taxis = siteInfo.Taxis
                    });
                }

                return cacheInfoList;
            });
        }
    }
}