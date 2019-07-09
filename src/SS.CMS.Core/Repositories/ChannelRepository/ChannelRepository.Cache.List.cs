using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using SS.CMS.Utils;
using Attr = SS.CMS.Core.Models.Attributes.ChannelAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ChannelRepository
    {
        [Serializable]
        private class CacheInfo
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public List<int> ParentsIdList { get; set; }
            public string ChannelName { get; set; }
            public string IndexName { get; set; }
        }

        private async Task RemoveListCacheAsync(int siteId)
        {
            var cacheKey = _cache.GetListKey(this, siteId);
            await _cache.RemoveAsync(cacheKey);
        }

        private async Task<List<CacheInfo>> GetListCacheAsync(int siteId)
        {
            var cacheKey = _cache.GetListKey(this, siteId);
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                var channelInfoList = await _repository.GetAllAsync(Q
                .Select(Attr.Id, Attr.ParentId, Attr.ParentsPath, Attr.ChannelName, Attr.IndexName)
                .Where(Attr.SiteId, siteId)
                .OrderBy(Attr.Taxis));

                var cacheInfoList = new List<CacheInfo>();
                foreach (var channelInfo in channelInfoList)
                {
                    cacheInfoList.Add(new CacheInfo
                    {
                        Id = channelInfo.Id,
                        ChannelName = channelInfo.ChannelName,
                        IndexName = channelInfo.IndexName,
                        ParentId = channelInfo.ParentId,
                        ParentsIdList = TranslateUtils.StringCollectionToIntList(channelInfo.ParentsPath)
                    });
                }

                return cacheInfoList;
            });
        }
    }
}