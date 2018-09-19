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
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ChannelDao.GetSiteId(channelId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetSequence(int siteId, int channelId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetSequence),
                siteId.ToString(), channelId.ToString());
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ChannelDao.GetSequence(siteId, channelId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DataSet GetStlDataSourceBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetStlDataSourceBySiteId),
                       siteId.ToString(), startNum.ToString(), totalNum.ToString(), whereString, orderByString);
            var retval = StlCacheManager.Get<DataSet>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<DataSet>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelDao.GetStlDataSourceBySiteId(siteId, startNum, totalNum, whereString, orderByString);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DataSet GetStlDataSet(List<int> channelIdList, int startNum, int totalNum, string whereString, string orderByString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetStlDataSet),
                       TranslateUtils.ObjectCollectionToString(channelIdList), startNum.ToString(), totalNum.ToString(), whereString, orderByString);
            var retval = StlCacheManager.Get<DataSet>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<DataSet>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelDao.GetStlDataSet(channelIdList, startNum, totalNum, whereString, orderByString);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        //public static int GetIdByIndexName(int siteId, string channelIndex)
        //{
        //    var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetIdByIndexName),
        //               siteId.ToString(), channelIndex);
        //    var retval = StlCacheManager.GetInt(cacheKey);
        //    if (retval != -1) return retval;

        //    lock (LockObject)
        //    {
        //        retval = StlCacheManager.GetInt(cacheKey);
        //        if (retval == -1)
        //        {
        //            retval = DataProvider.ChannelDao.GetIdByIndexName(siteId, channelIndex);
        //            StlCacheManager.Set(cacheKey, retval);
        //        }
        //    }

        //    return retval;
        //}

        public static int GetIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetIdByParentIdAndTaxis),
                       parentId.ToString(), taxis.ToString(), isNextChannel.ToString());
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ChannelDao.GetIdByParentIdAndTaxis(parentId, taxis, isNextChannel);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetWhereString(int siteId, string groupContent, string groupContentNot, bool isImageExists, bool isImage, string where)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetWhereString),
                       siteId.ToString(), groupContent, groupContentNot, isImageExists.ToString(),
                       isImage.ToString(), where);
            var retval = StlCacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelDao.GetWhereString(groupContent, groupContentNot,
                    isImageExists, isImage, where);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<int> GetIdListByTotalNum(List<int> channelIdList, int totalNum, string orderByString, string whereString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetIdListByTotalNum),
                       TranslateUtils.ObjectCollectionToString(channelIdList), totalNum.ToString(), orderByString, whereString);
            var retval = StlCacheManager.Get<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelDao.GetIdListByTotalNum(channelIdList, totalNum, orderByString, whereString);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static ChannelInfo GetChannelInfoByLastAddDate(int channelId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetChannelInfoByLastAddDate),
                    channelId.ToString());
            var retval = StlCacheManager.Get<ChannelInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<ChannelInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelDao.GetChannelInfoByLastAddDate(channelId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static ChannelInfo GetChannelInfoByTaxis(int channelId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetChannelInfoByTaxis),
                    channelId.ToString());
            var retval = StlCacheManager.Get<ChannelInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<ChannelInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelDao.GetChannelInfoByTaxis(channelId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
