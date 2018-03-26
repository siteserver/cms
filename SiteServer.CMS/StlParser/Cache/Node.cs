using System.Collections.Generic;
using System.Data;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Node
    {
        private static readonly object LockObject = new object();

        public static void ClearCache()
        {
            StlCacheUtils.ClearCache(nameof(Node));
        }

        public static int GetSiteId(int channelId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetSiteId),
                    channelId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ChannelDao.GetSiteId(channelId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetSequence(int siteId, int channelId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetSequence),
                siteId.ToString(), channelId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ChannelDao.GetSequence(siteId, channelId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DataSet GetStlDataSourceBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetStlDataSourceBySiteId),
                       siteId.ToString(), startNum.ToString(), totalNum.ToString(), whereString, orderByString);
            var retval = StlCacheUtils.GetCache<DataSet>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<DataSet>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelDao.GetStlDataSourceBySiteId(siteId, startNum, totalNum, whereString, orderByString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DataSet GetStlDataSet(List<int> channelIdList, int startNum, int totalNum, string whereString, string orderByString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetStlDataSet),
                       TranslateUtils.ObjectCollectionToString(channelIdList), startNum.ToString(), totalNum.ToString(), whereString, orderByString);
            var retval = StlCacheUtils.GetCache<DataSet>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<DataSet>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelDao.GetStlDataSet(channelIdList, startNum, totalNum, whereString, orderByString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetIdByIndexName(int siteId, string channelIndex)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetIdByIndexName),
                       siteId.ToString(), channelIndex);
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ChannelDao.GetIdByIndexName(siteId, channelIndex);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetIdByParentIdAndTaxis),
                       parentId.ToString(), taxis.ToString(), isNextChannel.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ChannelDao.GetIdByParentIdAndTaxis(parentId, taxis, isNextChannel);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetIdByParentIdAndChannelName(int siteId, int parentId, string nodeName, bool recursive)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node),
                       nameof(GetIdByParentIdAndChannelName), siteId.ToString(),
                       parentId.ToString(), nodeName, recursive.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ChannelDao.GetIdByParentIdAndChannelName(siteId,
                    parentId, nodeName, recursive);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetWhereString(int siteId, string groupContent, string groupContentNot, bool isImageExists, bool isImage, string where)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetWhereString),
                       siteId.ToString(), groupContent, groupContentNot, isImageExists.ToString(),
                       isImage.ToString(), where);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelDao.GetWhereString(siteId, groupContent, groupContentNot,
                    isImageExists, isImage, where);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<int> GetIdListByTotalNum(List<int> channelIdList, int totalNum, string orderByString, string whereString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetIdListByTotalNum),
                       TranslateUtils.ObjectCollectionToString(channelIdList), totalNum.ToString(), orderByString, whereString);
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelDao.GetIdListByTotalNum(channelIdList, totalNum, orderByString, whereString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static ChannelInfo GetChannelInfoByLastAddDate(int channelId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetChannelInfoByLastAddDate),
                    channelId.ToString());
            var retval = StlCacheUtils.GetCache<ChannelInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<ChannelInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelDao.GetChannelInfoByLastAddDate(channelId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static ChannelInfo GetChannelInfoByTaxis(int channelId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetChannelInfoByTaxis),
                    channelId.ToString());
            var retval = StlCacheUtils.GetCache<ChannelInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<ChannelInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelDao.GetChannelInfoByTaxis(channelId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
