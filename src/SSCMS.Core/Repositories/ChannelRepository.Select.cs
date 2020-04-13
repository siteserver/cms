using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class ChannelRepository
    {
        private string GetListKey(int siteId)
        {
            return CacheUtils.GetListKey(_repository.TableName, siteId);
        }

        private string GetEntityKey(int channelId)
        {
            return CacheUtils.GetEntityKey(_repository.TableName, channelId);
        }

        public async Task CacheAllAsync(Site site)
        {
            var cacheManager = await _repository.GetCacheManagerAsync();

            if (!cacheManager.Exists(GetListKey(site.Id)) || !cacheManager.Exists(GetEntityKey(site.Id)))
            {
                var channels = await _repository.GetAllAsync(Q
                    .Where(nameof(Channel.SiteId), site.Id)
                );

                foreach (var channel in channels)
                {
                    cacheManager.Put(GetEntityKey(channel.Id), channel);
                }

                var summaries = channels
                    .OrderBy(x => x.Taxis)
                    .ThenBy(x => x.Id)
                    .Select(x => new ChannelSummary
                    {
                        Id = x.Id,
                        ChannelName = x.ChannelName,
                        ParentId = x.ParentId,
                        ParentsPath = x.ParentsPath,
                        IndexName = x.IndexName,
                        ContentModelPluginId = x.ContentModelPluginId,
                        Taxis = x.Taxis,
                        AddDate = x.AddDate
                    }).ToList();

                cacheManager.Put(GetListKey(site.Id), summaries);
            }
        }

        public async Task<List<ChannelSummary>> GetSummariesAsync(int siteId)
        {
            return await _repository.GetAllAsync<ChannelSummary>(Q
                .Select(nameof(Channel.Id), nameof(Channel.ChannelName), nameof(Channel.ParentId), nameof(Channel.IndexName), nameof(Channel.Taxis), nameof(Channel.AddDate))
                .Where(nameof(Channel.SiteId), siteId)
                .OrderBy(nameof(Channel.Taxis), nameof(Channel.Id))
                .CachingGet(GetListKey(siteId))
            );
        }

        public async Task<Channel> GetAsync(int channelId)
        {
            if (channelId == 0) return null;

            channelId = Math.Abs(channelId);
            return await _repository.GetAsync(channelId, Q
                .CachingGet(GetEntityKey(channelId))
            );
        }
    }
}
