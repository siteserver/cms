using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using Attr = SS.CMS.Core.Models.Attributes.ContentAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private const int TaxisIsTopStartValue = 2000000000;

        public async Task<int> GetMaxTaxisAsync(int channelId, bool isTop)
        {
            var maxTaxis = 0;

            if (isTop)
            {
                maxTaxis = TaxisIsTopStartValue;

                var max = await _repository.MaxAsync(Attr.Taxis, Q
                    .Where(Attr.ChannelId, channelId)
                    .Where(Attr.Taxis, ">=", TaxisIsTopStartValue)
                );

                if (max != null)
                {
                    maxTaxis = max.Value;
                }

                if (maxTaxis < TaxisIsTopStartValue)
                {
                    maxTaxis = TaxisIsTopStartValue;
                }
            }
            else
            {
                var max = await _repository.MaxAsync(Attr.Taxis, Q
                    .Where(Attr.ChannelId, channelId)
                    .Where(Attr.Taxis, "<", TaxisIsTopStartValue)
                );

                if (max != null)
                {
                    maxTaxis = max.Value;
                }
            }

            return maxTaxis;
        }

        public async Task<(int ChannelId, T Value)?> GetChanelIdAndValueAsync<T>(int contentId, string name)
        {
            return await _repository.GetAsync<(int ChannelId, T Value)?>(Q
                .Select(Attr.ChannelId, name)
                .Where(Attr.Id, contentId)
            );
        }

        public async Task<T> GetValueAsync<T>(int contentId, string name)
        {
            return await _repository.GetAsync<T>(Q
                .Select(name)
                .Where(Attr.Id, contentId)
            );
        }

        public async Task<int> GetTotalHitsAsync(int siteId)
        {
            return await _repository.SumAsync(Attr.Hits, Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.IsChecked, true.ToString())
                .Where(Attr.Hits, ">", 0)
            );
        }

        public async Task<int> GetFirstContentIdAsync(int siteId, int channelId)
        {
            return await _repository.GetAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .OrderByDesc(Attr.Taxis, Attr.Id)
            );
        }

        public async Task<int> GetContentIdAsync(int channelId, int taxis, bool isNextContent)
        {
            if (isNextContent)
            {
                return await _repository.GetAsync<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.ChannelId, channelId)
                    .Where(Attr.Taxis, "<", taxis)
                    .Where(Attr.IsChecked, true.ToString())
                    .OrderByDesc(Attr.Taxis));
            }

            return await _repository.GetAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.Taxis, ">", taxis)
                .Where(Attr.IsChecked, true.ToString())
                .OrderBy(Attr.Taxis));
        }

        public async Task<int> GetChannelIdAsync(int contentId)
        {
            return await _repository.GetAsync<int>(Q.Select(Attr.ChannelId).Where(Attr.Id, contentId));
        }

        public async Task<int> GetContentIdAsync(int channelId, TaxisType taxisType)
        {
            var query = Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId);

            QueryOrder(query, taxisType);

            return await _repository.GetAsync<int>(query);
        }

        public async Task<int> GetTaxisToInsertAsync(int channelId, bool isTop)
        {
            int taxis;

            if (isTop)
            {
                taxis = await GetMaxTaxisAsync(channelId, true) + 1;
            }
            else
            {
                taxis = await GetMaxTaxisAsync(channelId, false) + 1;
            }

            return taxis;
        }

        private async Task<int> GetTaxisAsync(int contentId)
        {
            return await _repository.GetAsync<int>(Q
                .Select(Attr.Taxis)
                .Where(Attr.Id, contentId)
            );
        }

        public async Task<int> GetSequenceAsync(int channelId, int contentId)
        {
            var taxis = await GetTaxisAsync(contentId);

            return await _repository.CountAsync(Q
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.IsChecked, true.ToString())
                .Where(Attr.Taxis, "<", taxis)
                .WhereNot(Attr.SourceId, SourceManager.Preview)
            ) + 1;
        }

        public async Task<int> GetCountCheckedImageAsync(int siteId, int channelId)
        {
            return await _repository.CountAsync(Q
                .Where(Attr.ChannelId, channelId)
                .WhereNotNull(Attr.ImageUrl)
                .WhereNot(Attr.ImageUrl, string.Empty)
                .Where(Attr.IsChecked, true.ToString())
                .WhereNot(Attr.SourceId, SourceManager.Preview)
            );
        }

        public async Task<int> GetCountOfContentAddAsync(int siteId, int channelId, ScopeType scope, DateTime begin, DateTime end, int userId, bool? checkedState)
        {
            var channelInfo = await _channelRepository.GetChannelInfoAsync(channelId);
            var channelIdList = await _channelRepository.GetChannelIdListAsync(channelInfo, scope, string.Empty, string.Empty, string.Empty);
            return await GetCountOfContentAddAsync(siteId, channelIdList, begin, end, userId, checkedState);
        }

        private async Task<int> GetCountOfContentAddAsync(int siteId, List<int> channelIdList, DateTime begin, DateTime end, int userId, bool? checkedState)
        {
            var query = Q.Where(Attr.SiteId, siteId);
            query.WhereIn(Attr.ChannelId, channelIdList);
            query.WhereBetween(Attr.AddDate, begin, end.AddDays(1));
            if (userId > 0)
            {
                query.Where(Attr.UserId, userId);
            }

            if (checkedState.HasValue)
            {
                query.Where(Attr.IsChecked, checkedState.ToString());
            }

            return await _repository.CountAsync(query);
        }

        public async Task<int> GetCountOfContentUpdateAsync(int siteId, int channelId, ScopeType scope, DateTime begin, DateTime end, int userId)
        {
            var channelInfo = await _channelRepository.GetChannelInfoAsync(channelId);
            var channelIdList = await _channelRepository.GetChannelIdListAsync(channelInfo, scope, string.Empty, string.Empty, string.Empty);
            return await GetCountOfContentUpdateAsync(siteId, channelIdList, begin, end, userId);
        }

        private async Task<int> GetCountOfContentUpdateAsync(int siteId, List<int> channelIdList, DateTime begin, DateTime end, int userId)
        {
            var query = Q.Where(Attr.SiteId, siteId);
            query.WhereIn(Attr.ChannelId, channelIdList);
            query.WhereBetween(Attr.LastModifiedDate, begin, end.AddDays(1));
            query.WhereRaw($"{Attr.LastModifiedDate} != {Attr.AddDate}");
            if (userId > 0)
            {
                query.Where(Attr.UserId, userId);
            }

            return await _repository.CountAsync(query);
        }

        public async Task<ContentInfo> GetCacheContentInfoAsync(int contentId)
        {
            if (contentId <= 0) return null;
            return await _repository.GetAsync(contentId);
        }
    }
}
