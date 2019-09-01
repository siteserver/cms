using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache.Stl
{
    public static class StlChannelCache
    {
        private static readonly object LockObject = new object();

        public static void ClearCache()
        {
            StlCacheManager.Clear(nameof(StlChannelCache));
        }

        public static int GetSiteId(int channelId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetSiteId),
                    channelId.ToString());
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.GetInt(cacheKey);
                if (retVal == -1)
                {
                    retVal = DataProvider.ChannelDao.GetSiteId(channelId);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static int GetSequence(int siteId, int channelId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetSequence),
                siteId.ToString(), channelId.ToString());
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.GetInt(cacheKey);
                if (retVal == -1)
                {
                    retVal = DataProvider.ChannelDao.GetSequence(siteId, channelId);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static DataSet GetStlDataSourceBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetStlDataSourceBySiteId),
                       siteId.ToString(), startNum.ToString(), totalNum.ToString(), whereString, orderByString);
            var retVal = StlCacheManager.Get<DataSet>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<DataSet>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.ChannelDao.GetStlDataSourceBySiteId(siteId, startNum, totalNum, whereString, orderByString);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static DataSet GetStlDataSet(List<int> channelIdList, int startNum, int totalNum, string whereString, string orderByString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetStlDataSet),
                       TranslateUtils.ObjectCollectionToString(channelIdList), startNum.ToString(), totalNum.ToString(), whereString, orderByString);
            var retVal = StlCacheManager.Get<DataSet>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<DataSet>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.ChannelDao.GetStlDataSet(channelIdList, startNum, totalNum, whereString, orderByString);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        //public static int GetIdByIndexName(int siteId, string channelIndex)
        //{
        //    var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetIdByIndexName),
        //               siteId.ToString(), channelIndex);
        //    var retVal = StlCacheManager.GetInt(cacheKey);
        //    if (retVal != -1) return retVal;

        //    lock (LockObject)
        //    {
        //        retVal = StlCacheManager.GetInt(cacheKey);
        //        if (retVal == -1)
        //        {
        //            retVal = DataProvider.ChannelDao.GetIdByIndexName(siteId, channelIndex);
        //            StlCacheManager.Set(cacheKey, retVal);
        //        }
        //    }

        //    return retVal;
        //}

        public static int GetIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetIdByParentIdAndTaxis),
                       parentId.ToString(), taxis.ToString(), isNextChannel.ToString());
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.GetInt(cacheKey);
                if (retVal == -1)
                {
                    retVal = DataProvider.ChannelDao.GetIdByParentIdAndTaxis(parentId, taxis, isNextChannel);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static string GetWhereString(int siteId, string groupContent, string groupContentNot, bool isImageExists, bool isImage, string where)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetWhereString),
                       siteId.ToString(), groupContent, groupContentNot, isImageExists.ToString(),
                       isImage.ToString(), where);
            var retVal = StlCacheManager.Get<string>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<string>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.ChannelDao.GetWhereString(groupContent, groupContentNot,
                    isImageExists, isImage, where);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static List<int> GetIdListByTotalNum(List<int> channelIdList, int totalNum, string orderByString, string whereString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetIdListByTotalNum),
                       TranslateUtils.ObjectCollectionToString(channelIdList), totalNum.ToString(), orderByString, whereString);
            var retVal = StlCacheManager.Get<List<int>>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<List<int>>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.ChannelDao.GetIdListByTotalNum(channelIdList, totalNum, orderByString, whereString);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static ChannelInfo GetChannelInfoByLastAddDate(int channelId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetChannelInfoByLastAddDate),
                    channelId.ToString());
            var retVal = StlCacheManager.Get<ChannelInfo>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<ChannelInfo>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.ChannelDao.GetChannelInfoByLastAddDate(channelId);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static ChannelInfo GetChannelInfoByTaxis(int channelId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetChannelInfoByTaxis),
                    channelId.ToString());
            var retVal = StlCacheManager.Get<ChannelInfo>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<ChannelInfo>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.ChannelDao.GetChannelInfoByTaxis(channelId);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }
    }
}
