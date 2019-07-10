using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Core.Services;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private readonly string CountCacheKey =
            StringUtils.GetCacheKey(nameof(ContentRepository)) + ".Count";

        private Dictionary<string, IEnumerable<ContentCount>> CountGetAllContentCounts()
        {
            var retVal = CacheManager.Get<Dictionary<string, IEnumerable<ContentCount>>>(CountCacheKey);
            if (retVal != null) return retVal;

            retVal = CacheManager.Get<Dictionary<string, IEnumerable<ContentCount>>>(CountCacheKey);
            if (retVal == null)
            {
                retVal = new Dictionary<string, IEnumerable<ContentCount>>();
                CacheManager.Insert(CountCacheKey, retVal);
            }

            return retVal;
        }

        private async Task<IEnumerable<ContentCount>> CountGetContentCountInfoListAsync(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return new List<ContentCount>();

            var dict = CountGetAllContentCounts();
            dict.TryGetValue(tableName, out var countList);
            if (countList != null) return countList;

            countList = await GetContentCountInfoListAsync();
            dict[tableName] = countList;

            return countList;
        }

        private void CountClear(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var dict = CountGetAllContentCounts();
            dict.Remove(tableName);
        }

        private async Task CountAddAsync(string tableName, Content contentInfo)
        {
            if (string.IsNullOrEmpty(tableName) || contentInfo == null) return;

            var countInfoList = await CountGetContentCountInfoListAsync(tableName);
            var countInfo = countInfoList.FirstOrDefault(x =>
                x.SiteId == contentInfo.SiteId && x.ChannelId == contentInfo.ChannelId &&
                x.IsChecked == contentInfo.IsChecked && x.CheckedLevel == contentInfo.CheckedLevel && x.UserId == contentInfo.UserId);
            if (countInfo != null)
            {
                countInfo.Count++;
            }
            else
            {
                countInfo = new ContentCount
                {
                    SiteId = contentInfo.SiteId,
                    ChannelId = contentInfo.ChannelId,
                    IsChecked = contentInfo.IsChecked,
                    CheckedLevel = contentInfo.CheckedLevel,
                    UserId = contentInfo.UserId,
                    Count = 1
                };
                //countInfoList.Add(countInfo);
            }
        }

        private bool CountIsChanged(Content contentInfo1, Content contentInfo2)
        {
            if (contentInfo1 == null || contentInfo2 == null) return true;

            return contentInfo1.SiteId != contentInfo2.SiteId ||
                   contentInfo1.ChannelId != contentInfo2.ChannelId ||
                   contentInfo1.IsChecked != contentInfo2.IsChecked ||
                   contentInfo1.CheckedLevel != contentInfo2.CheckedLevel ||
                   contentInfo1.UserId != contentInfo2.UserId;
        }

        private async Task CountRemoveAsync(string tableName, Content contentInfo)
        {
            if (string.IsNullOrEmpty(tableName) || contentInfo == null) return;

            var countInfoList = await CountGetContentCountInfoListAsync(tableName);
            var countInfo = countInfoList.FirstOrDefault(x =>
                x.SiteId == contentInfo.SiteId && x.ChannelId == contentInfo.ChannelId &&
                x.IsChecked == contentInfo.IsChecked && x.CheckedLevel == contentInfo.CheckedLevel && x.UserId == contentInfo.UserId);
            if (countInfo != null && countInfo.Count > 0)
            {
                countInfo.Count--;
            }
        }

        private async Task<int> CountGetSiteCountByIsCheckedAsync(Site siteInfo, bool isChecked)
        {
            var tableNames = await _siteRepository.GetTableNameListAsync(_pluginManager, siteInfo);

            var count = 0;
            foreach (var tableName in tableNames)
            {
                var list = await CountGetContentCountInfoListAsync(tableName);
                count += list.Where(x => x.SiteId == siteInfo.Id && x.IsChecked == isChecked).Sum(x => x.Count);
            }

            return count;
        }

        private async Task<int> CountGetChannelCountByOnlyAdminIdAsync(Site siteInfo, Channel channelInfo, int? onlyAdminId)
        {
            var tableName = await _channelRepository.GetTableNameAsync(_pluginManager, siteInfo, channelInfo);

            var list = await CountGetContentCountInfoListAsync(tableName);
            return onlyAdminId.HasValue
                ? list.Where(x =>
                        x.SiteId == siteInfo.Id &&
                        x.ChannelId == channelInfo.Id &&
                        x.UserId == onlyAdminId.Value)
                    .Sum(x => x.Count)
                : list.Where(x =>
                        x.SiteId == siteInfo.Id &&
                        x.ChannelId == channelInfo.Id)
                    .Sum(x => x.Count);
        }

        private async Task<int> CountGetChannelCountByIsCheckedAsync(Site siteInfo, Channel channelInfo, bool isChecked)
        {
            var tableName = await _channelRepository.GetTableNameAsync(_pluginManager, siteInfo, channelInfo);

            var list = await CountGetContentCountInfoListAsync(tableName);
            return list.Where(x =>
                    x.SiteId == siteInfo.Id && x.ChannelId == channelInfo.Id &&
                    x.IsChecked == isChecked)
                .Sum(x => x.Count);
        }

        // public

        public async Task<int> GetCountAsync(Site siteInfo, bool isChecked)
        {
            return await CountGetSiteCountByIsCheckedAsync(siteInfo, isChecked);
        }

        public async Task<int> GetCountAsync(Site siteInfo, Channel channelInfo, int? onlyAdminId)
        {
            return await CountGetChannelCountByOnlyAdminIdAsync(siteInfo, channelInfo, onlyAdminId);
        }

        public async Task<int> GetCountAsync(Site siteInfo, Channel channelInfo, bool isChecked)
        {
            return await CountGetChannelCountByIsCheckedAsync(siteInfo, channelInfo, isChecked);
        }
    }
}