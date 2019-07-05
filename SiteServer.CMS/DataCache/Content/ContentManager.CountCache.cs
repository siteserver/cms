using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.DataCache.Content
{
    public static partial class ContentManager
    {
        private static class CountCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CacheKey =
                DataCacheManager.GetCacheKey(nameof(ContentManager)) + "." + nameof(CountCache);

            private static Dictionary<string, List<ContentCountInfo>> GetAllContentCounts()
            {
                lock (LockObject)
                {
                    var retVal = DataCacheManager.Get<Dictionary<string, List<ContentCountInfo>>>(CacheKey);
                    if (retVal != null) return retVal;

                    retVal = DataCacheManager.Get<Dictionary<string, List<ContentCountInfo>>>(CacheKey);
                    if (retVal == null)
                    {
                        retVal = new Dictionary<string, List<ContentCountInfo>>();
                        DataCacheManager.Insert(CacheKey, retVal);
                    }

                    return retVal;
                }
            }

            private static IList<ContentCountInfo> GetContentCountInfoList(string tableName)
            {
                if (string.IsNullOrEmpty(tableName)) return new List<ContentCountInfo>();

                var dict = GetAllContentCounts();
                dict.TryGetValue(tableName, out var countList);
                if (countList != null) return countList;

                countList = DataProvider.ContentDao.GetContentCountInfoList(tableName);
                dict[tableName] = countList;

                return countList;
            }

            public static void Clear(string tableName)
            {
                if (string.IsNullOrEmpty(tableName)) return;

                lock (LockObject)
                {
                    var dict = GetAllContentCounts();
                    dict.Remove(tableName);
                }
            }

            public static void Add(string tableName, ContentInfo contentInfo)
            {
                if (string.IsNullOrEmpty(tableName) || contentInfo == null) return;

                lock (LockObject)
                {
                    var countInfoList = GetContentCountInfoList(tableName);
                    var countInfo = countInfoList.FirstOrDefault(x =>
                        x.SiteId == contentInfo.SiteId && x.ChannelId == contentInfo.ChannelId &&
                        x.IsChecked == contentInfo.IsChecked.ToString() && x.CheckedLevel == contentInfo.CheckedLevel && x.AdminId == contentInfo.AdminId);
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
                            IsChecked = contentInfo.IsChecked.ToString(),
                            CheckedLevel = contentInfo.CheckedLevel,
                            AdminId = contentInfo.AdminId,
                            Count = 1
                        };
                        countInfoList.Add(countInfo);
                    }
                }
            }

            public static bool IsChanged(ContentInfo contentInfo1, ContentInfo contentInfo2)
            {
                if (contentInfo1 == null || contentInfo2 == null) return true;

                return contentInfo1.SiteId != contentInfo2.SiteId ||
                       contentInfo1.ChannelId != contentInfo2.ChannelId ||
                       contentInfo1.IsChecked != contentInfo2.IsChecked ||
                       contentInfo1.CheckedLevel != contentInfo2.CheckedLevel ||
                       contentInfo1.AdminId != contentInfo2.AdminId;
            }

            public static void Remove(string tableName, ContentInfo contentInfo)
            {
                if (string.IsNullOrEmpty(tableName) || contentInfo == null) return;

                lock (LockObject)
                {
                    var countInfoList = GetContentCountInfoList(tableName);
                    var countInfo = countInfoList.FirstOrDefault(x =>
                        x.SiteId == contentInfo.SiteId && x.ChannelId == contentInfo.ChannelId &&
                        x.IsChecked == contentInfo.IsChecked.ToString() && x.CheckedLevel == contentInfo.CheckedLevel && x.AdminId == contentInfo.AdminId);
                    if (countInfo != null && countInfo.Count > 0)
                    {
                        countInfo.Count--;
                    }
                }
            }

            public static int GetSiteCountByIsChecked(SiteInfo siteInfo, bool isChecked)
            {
                var tableNames = SiteManager.GetTableNameList(siteInfo);

                lock (LockObject)
                {
                    var count = 0;
                    foreach (var tableName in tableNames)
                    {
                        var list = GetContentCountInfoList(tableName);
                        count += list.Where(x => x.SiteId == siteInfo.Id && x.IsChecked == isChecked.ToString())
                            .Sum(x => x.Count);
                    }

                    return count;
                }
            }

            public static int GetChannelCountByOnlyAdminId(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId)
            {
                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                lock (LockObject)
                {
                    var list = GetContentCountInfoList(tableName);
                    return onlyAdminId.HasValue
                        ? list.Where(x =>
                                x.SiteId == siteInfo.Id &&
                                x.ChannelId == channelInfo.Id &&
                                x.AdminId == onlyAdminId.Value)
                            .Sum(x => x.Count)
                        : list.Where(x =>
                                x.SiteId == siteInfo.Id &&
                                x.ChannelId == channelInfo.Id)
                            .Sum(x => x.Count);
                }
            }

            public static int GetChannelCountByIsChecked(SiteInfo siteInfo, ChannelInfo channelInfo, bool isChecked)
            {
                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                lock (LockObject)
                {
                    var list = GetContentCountInfoList(tableName);
                    return list.Where(x =>
                            x.SiteId == siteInfo.Id && x.ChannelId == channelInfo.Id &&
                            x.IsChecked == isChecked.ToString())
                        .Sum(x => x.Count);
                }
            }
        }

        public static int GetCount(SiteInfo siteInfo, bool isChecked)
        {
            return CountCache.GetSiteCountByIsChecked(siteInfo, isChecked);
        }

        public static int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId)
        {
            return CountCache.GetChannelCountByOnlyAdminId(siteInfo, channelInfo, onlyAdminId);
        }

        public static int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo, bool isChecked)
        {
            return CountCache.GetChannelCountByIsChecked(siteInfo, channelInfo, isChecked);
        }
    }
}