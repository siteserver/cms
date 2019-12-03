using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SiteServer.CMS.DataCache.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.DataCache.Stl
{
    public static class StlChannelCache
    {
        private static readonly object LockObject = new object();

        public static void ClearCache()
        {
            StlCacheManager.Clear(nameof(StlChannelCache));
        }

        public static async Task<int> GetSiteIdAsync(int channelId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetSiteIdAsync),
                    channelId.ToString());
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal == -1)
            {
                retVal = await DataProvider.ChannelRepository.GetSiteIdAsync(channelId);
                StlCacheManager.Set(cacheKey, retVal);
            }

            return retVal;
        }

        public static async Task<int> GetSequenceAsync(int siteId, int channelId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetSequenceAsync),
                siteId.ToString(), channelId.ToString());
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal == -1)
            {
                retVal = await DataProvider.ChannelRepository.GetSequenceAsync(siteId, channelId);
                StlCacheManager.Set(cacheKey, retVal);
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
                    retVal = DataProvider.ChannelRepository.GetStlDataSourceBySiteId(siteId, startNum, totalNum, whereString, orderByString);
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
                    retVal = DataProvider.ChannelRepository.GetStlDataSet(channelIdList, startNum, totalNum, whereString, orderByString);
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
        //            retVal = DataProvider.ChannelRepository.GetIdByIndexName(siteId, channelIndex);
        //            StlCacheManager.Set(cacheKey, retVal);
        //        }
        //    }

        //    return retVal;
        //}

        public static async Task<int> GetIdByParentIdAndTaxisAsync(int parentId, int taxis, bool isNextChannel)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetIdByParentIdAndTaxisAsync),
                       parentId.ToString(), taxis.ToString(), isNextChannel.ToString());
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal == -1)
            {
                retVal = await DataProvider.ChannelRepository.GetIdByParentIdAndTaxisAsync(parentId, taxis, isNextChannel);
                StlCacheManager.Set(cacheKey, retVal);
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
                    retVal = DataProvider.ChannelRepository.GetWhereString(groupContent, groupContentNot,
                    isImageExists, isImage, where);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static async Task<IEnumerable<int>> GetIdListByTotalNumAsync(List<int> channelIdList, int totalNum, string orderByString, string whereString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetIdListByTotalNumAsync),
                       TranslateUtils.ObjectCollectionToString(channelIdList), totalNum.ToString(), orderByString, whereString);
            var retVal = StlCacheManager.Get<IEnumerable<int>>(cacheKey);
            if (retVal != null) return retVal;

            retVal = StlCacheManager.Get<List<int>>(cacheKey);
            if (retVal == null)
            {
                retVal = await DataProvider.ChannelRepository.GetIdListByTotalNumAsync(channelIdList, totalNum, orderByString, whereString);
                StlCacheManager.Set(cacheKey, retVal);
            }

            return retVal;
        }

        public static async Task<Channel> GetChannelInfoByLastAddDateAsync(int channelId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetChannelInfoByLastAddDateAsync),
                    channelId.ToString());
            var retVal = StlCacheManager.Get<Channel>(cacheKey);
            if (retVal != null) return retVal;

            retVal = StlCacheManager.Get<Channel>(cacheKey);
            if (retVal == null)
            {
                retVal = await DataProvider.ChannelRepository.GetChannelByLastAddDateAsyncTask(channelId);
                StlCacheManager.Set(cacheKey, retVal);
            }

            return retVal;
        }

        public static async Task<Channel> GetChannelInfoByTaxisAsync(int channelId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetChannelInfoByTaxisAsync),
                    channelId.ToString());
            var retVal = StlCacheManager.Get<Channel>(cacheKey);
            if (retVal != null) return retVal;

            retVal = StlCacheManager.Get<Channel>(cacheKey);
            if (retVal == null)
            {
                retVal = await DataProvider.ChannelRepository.GetChannelByTaxisAsync(channelId);
                StlCacheManager.Set(cacheKey, retVal);
            }

            return retVal;
        }
    }
}
