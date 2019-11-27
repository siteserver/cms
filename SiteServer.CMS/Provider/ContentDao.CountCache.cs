using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public partial class ContentDao
    {
        private static class CountCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CacheKey =
                DataCacheManager.GetCacheKey(nameof(ContentDao)) + "." + nameof(CountCache);

            private static Dictionary<string, List<ContentCount>> GetAllContentCounts()
            {
                lock (LockObject)
                {
                    var retVal = DataCacheManager.Get<Dictionary<string, List<ContentCount>>>(CacheKey);
                    if (retVal != null) return retVal;

                    retVal = DataCacheManager.Get<Dictionary<string, List<ContentCount>>>(CacheKey);
                    if (retVal == null)
                    {
                        retVal = new Dictionary<string, List<ContentCount>>();
                        DataCacheManager.Insert(CacheKey, retVal);
                    }

                    return retVal;
                }
            }

            private static IList<ContentCount> GetContentCountInfoList(string tableName)
            {
                if (string.IsNullOrEmpty(tableName)) return new List<ContentCount>();

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

            public static void Add(string tableName, Content content)
            {
                if (string.IsNullOrEmpty(tableName) || content == null) return;

                lock (LockObject)
                {
                    var countInfoList = GetContentCountInfoList(tableName);
                    var countInfo = countInfoList.FirstOrDefault(x =>
                        x.SiteId == content.SiteId && x.ChannelId == content.ChannelId &&
                        x.IsChecked == content.Checked.ToString() && x.CheckedLevel == content.CheckedLevel && x.AdminId == content.AdminId);
                    if (countInfo != null)
                    {
                        countInfo.Count++;
                    }
                    else
                    {
                        countInfo = new ContentCount
                        {
                            SiteId = content.SiteId,
                            ChannelId = content.ChannelId,
                            IsChecked = content.Checked.ToString(),
                            CheckedLevel = content.CheckedLevel,
                            AdminId = content.AdminId,
                            Count = 1
                        };
                        countInfoList.Add(countInfo);
                    }
                }
            }

            public static bool IsChanged(Content contentInfo1, Content contentInfo2)
            {
                if (contentInfo1 == null || contentInfo2 == null) return true;

                return contentInfo1.SiteId != contentInfo2.SiteId ||
                       contentInfo1.ChannelId != contentInfo2.ChannelId ||
                       contentInfo1.Checked != contentInfo2.Checked ||
                       contentInfo1.CheckedLevel != contentInfo2.CheckedLevel ||
                       contentInfo1.AdminId != contentInfo2.AdminId;
            }

            public static void Remove(string tableName, Content content)
            {
                if (string.IsNullOrEmpty(tableName) || content == null) return;

                lock (LockObject)
                {
                    var countInfoList = GetContentCountInfoList(tableName);
                    var countInfo = countInfoList.FirstOrDefault(x =>
                        x.SiteId == content.SiteId && x.ChannelId == content.ChannelId &&
                        x.IsChecked == content.Checked.ToString() && x.CheckedLevel == content.CheckedLevel && x.AdminId == content.AdminId);
                    if (countInfo != null && countInfo.Count > 0)
                    {
                        countInfo.Count--;
                    }
                }
            }

            public static async Task<int> GetSiteCountIsCheckedAsync(Site site)
            {
                var tableNames = await DataProvider.SiteDao.GetTableNameListAsync(site);
                var isChecked = true.ToString();

                lock (LockObject)
                {
                    var count = 0;
                    foreach (var tableName in tableNames)
                    {
                        var list = GetContentCountInfoList(tableName);
                        count += list.Where(x => x.SiteId == site.Id && x.IsChecked == isChecked)
                            .Sum(x => x.Count);
                    }

                    return count;
                }
            }

            public static async Task<int> GetSiteCountIsCheckingAsync(Site site)
            {
                var tableNames = await DataProvider.SiteDao.GetTableNameListAsync(site);
                var isChecked = false.ToString();

                lock (LockObject)
                {
                    var count = 0;
                    foreach (var tableName in tableNames)
                    {
                        var list = GetContentCountInfoList(tableName);
                        count += list.Where(x => x.SiteId == site.Id && x.IsChecked == isChecked && x.CheckedLevel != -site.CheckContentLevel && x.CheckedLevel != CheckManager.LevelInt.CaoGao)
                            .Sum(x => x.Count);
                    }

                    return count;
                }
            }

            public static async Task<int> GetChannelCountAsync(Site site, Channel channel, int adminId, bool isAllContents)
            {
                return isAllContents
                    ? await GetChannelCountAllAsync(site, channel, adminId)
                    : await GetChannelCountSelfAsync(site, channel, adminId);
            }

            private static async Task<int> GetChannelCountSelfAsync(Site site, Channel channel, int adminId)
            {
                var tableName = await ChannelManager.GetTableNameAsync(site, channel);

                lock (LockObject)
                {
                    var list = GetContentCountInfoList(tableName);
                    return adminId > 0
                        ? list.Where(x =>
                                x.SiteId == site.Id &&
                                x.ChannelId == channel.Id &&
                                x.AdminId == adminId)
                            .Sum(x => x.Count)
                        : list.Where(x =>
                                x.SiteId == site.Id &&
                                x.ChannelId == channel.Id)
                            .Sum(x => x.Count);
                }
            }

            private static async Task<int> GetChannelCountAllAsync(Site site, Channel channel, int adminId)
            {
                var count = 0;
                var channelInfoList = new List<Channel> {channel};
                var channelIdList = await ChannelManager.GetChannelIdListAsync(channel, EScopeType.Descendant);
                foreach (var channelId in channelIdList)
                {
                    channelInfoList.Add(await ChannelManager.GetChannelAsync(site.Id, channelId));
                }

                foreach (var info in channelInfoList)
                {
                    count += await GetChannelCountSelfAsync(site, info, adminId);
                }

                return count;
            }

            public static async Task<int> GetChannelCountByIsCheckedAsync(Site site, Channel channel, bool isChecked)
            {
                var tableName = await ChannelManager.GetTableNameAsync(site, channel);

                lock (LockObject)
                {
                    var list = GetContentCountInfoList(tableName);
                    return list.Where(x =>
                            x.SiteId == site.Id && x.ChannelId == channel.Id &&
                            x.IsChecked == isChecked.ToString())
                        .Sum(x => x.Count);
                }
            }
        }

        public async Task<int> GetCountIsCheckedAsync(Site site)
        {
            return await CountCache.GetSiteCountIsCheckedAsync(site);
        }

        public async Task<int> GetCountCheckingAsync(Site site)
        {
            return await CountCache.GetSiteCountIsCheckingAsync(site);
        }

        public async Task<int> GetCountAsync(Site site, Channel channel, int adminId, bool isAllContents = false)
        {
            return await CountCache.GetChannelCountAsync(site, channel, adminId, isAllContents);
        }

        public async Task<int> GetCountAsync(Site site, Channel channel, bool isChecked)
        {
            return await CountCache.GetChannelCountByIsCheckedAsync(site, channel, isChecked);
        }
    }
}