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
        private class Cache
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

        private async Task<List<Cache>> GetListCacheAsync(int siteId)
        {
            var cacheKey = _cache.GetListKey(this, siteId);
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                var channelList = await _repository.GetAllAsync(Q
                .Select(Attr.Id, Attr.ParentId, Attr.ParentsPath, Attr.ChannelName, Attr.IndexName)
                .Where(Attr.SiteId, siteId)
                .OrderBy(Attr.Taxis));

                var list = new List<Cache>();
                foreach (var channel in channelList)
                {
                    list.Add(new Cache
                    {
                        Id = channel.Id,
                        ChannelName = channel.ChannelName,
                        IndexName = channel.IndexName,
                        ParentId = channel.ParentId,
                        ParentsIdList = TranslateUtils.StringCollectionToIntList(channel.ParentsPath)
                    });
                }

                return list;
            });
        }
    }
}