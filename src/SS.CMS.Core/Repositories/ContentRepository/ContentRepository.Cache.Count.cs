using System.Collections.Generic;
using System.Linq;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private readonly object CountLockObject = new object();
        private readonly string CountCacheKey =
            StringUtils.GetCacheKey(nameof(ContentRepository)) + ".Count";

        private Dictionary<string, List<ContentCountInfo>> CountGetAllContentCounts()
        {
            lock (CountLockObject)
            {
                var retVal = _cacheManager.Get<Dictionary<string, List<ContentCountInfo>>>(CountCacheKey);
                if (retVal != null) return retVal;

                retVal = _cacheManager.Get<Dictionary<string, List<ContentCountInfo>>>(CountCacheKey);
                if (retVal == null)
                {
                    retVal = new Dictionary<string, List<ContentCountInfo>>();
                    _cacheManager.Insert(CountCacheKey, retVal);
                }

                return retVal;
            }
        }

        private IList<ContentCountInfo> CountGetContentCountInfoList(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return new List<ContentCountInfo>();

            var dict = CountGetAllContentCounts();
            dict.TryGetValue(tableName, out var countList);
            if (countList != null) return countList;

            countList = GetContentCountInfoList();
            dict[tableName] = countList;

            return countList;
        }

        private void CountClear(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            lock (CountLockObject)
            {
                var dict = CountGetAllContentCounts();
                dict.Remove(tableName);
            }
        }

        private void CountAdd(string tableName, ContentInfo contentInfo)
        {
            if (string.IsNullOrEmpty(tableName) || contentInfo == null) return;

            lock (CountLockObject)
            {
                var countInfoList = CountGetContentCountInfoList(tableName);
                var countInfo = countInfoList.FirstOrDefault(x =>
                    x.SiteId == contentInfo.SiteId && x.ChannelId == contentInfo.ChannelId &&
                    x.IsChecked == contentInfo.IsChecked && x.CheckedLevel == contentInfo.CheckedLevel && x.UserId == contentInfo.UserId);
                if (countInfo != null)
                {
                    countInfo.Count++;
                }
                else
                {
                    countInfo = new ContentCountInfo
                    {
                        SiteId = contentInfo.SiteId,
                        ChannelId = contentInfo.ChannelId,
                        IsChecked = contentInfo.IsChecked,
                        CheckedLevel = contentInfo.CheckedLevel,
                        UserId = contentInfo.UserId,
                        Count = 1
                    };
                    countInfoList.Add(countInfo);
                }
            }
        }

        private bool CountIsChanged(ContentInfo contentInfo1, ContentInfo contentInfo2)
        {
            if (contentInfo1 == null || contentInfo2 == null) return true;

            return contentInfo1.SiteId != contentInfo2.SiteId ||
                   contentInfo1.ChannelId != contentInfo2.ChannelId ||
                   contentInfo1.IsChecked != contentInfo2.IsChecked ||
                   contentInfo1.CheckedLevel != contentInfo2.CheckedLevel ||
                   contentInfo1.UserId != contentInfo2.UserId;
        }

        private void CountRemove(string tableName, ContentInfo contentInfo)
        {
            if (string.IsNullOrEmpty(tableName) || contentInfo == null) return;

            lock (CountLockObject)
            {
                var countInfoList = CountGetContentCountInfoList(tableName);
                var countInfo = countInfoList.FirstOrDefault(x =>
                    x.SiteId == contentInfo.SiteId && x.ChannelId == contentInfo.ChannelId &&
                    x.IsChecked == contentInfo.IsChecked && x.CheckedLevel == contentInfo.CheckedLevel && x.UserId == contentInfo.UserId);
                if (countInfo != null && countInfo.Count > 0)
                {
                    countInfo.Count--;
                }
            }
        }

        private int CountGetSiteCountByIsChecked(SiteInfo siteInfo, bool isChecked)
        {
            var tableNames = _siteRepository.GetTableNameList(_pluginManager, siteInfo);

            lock (CountLockObject)
            {
                var count = 0;
                foreach (var tableName in tableNames)
                {
                    var list = CountGetContentCountInfoList(tableName);
                    count += list.Where(x => x.SiteId == siteInfo.Id && x.IsChecked == isChecked).Sum(x => x.Count);
                }

                return count;
            }
        }

        private int CountGetChannelCountByOnlyAdminId(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId)
        {
            var tableName = _channelRepository.GetTableName(_pluginManager, siteInfo, channelInfo);

            lock (CountLockObject)
            {
                var list = CountGetContentCountInfoList(tableName);
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
        }

        private int CountGetChannelCountByIsChecked(SiteInfo siteInfo, ChannelInfo channelInfo, bool isChecked)
        {
            var tableName = _channelRepository.GetTableName(_pluginManager, siteInfo, channelInfo);

            lock (CountLockObject)
            {
                var list = CountGetContentCountInfoList(tableName);
                return list.Where(x =>
                        x.SiteId == siteInfo.Id && x.ChannelId == channelInfo.Id &&
                        x.IsChecked == isChecked)
                    .Sum(x => x.Count);
            }
        }

        // public

        public int GetCount(SiteInfo siteInfo, bool isChecked)
        {
            return CountGetSiteCountByIsChecked(siteInfo, isChecked);
        }

        public int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId)
        {
            return CountGetChannelCountByOnlyAdminId(siteInfo, channelInfo, onlyAdminId);
        }

        public int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo, bool isChecked)
        {
            return CountGetChannelCountByIsChecked(siteInfo, channelInfo, isChecked);
        }
    }
}