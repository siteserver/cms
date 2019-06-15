using System.Collections.Generic;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Cache.Stl
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
                    retval = DataProvider.ChannelRepository.GetSiteId(channelId);
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
                    retval = DataProvider.ChannelRepository.GetSequence(siteId, channelId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        // public static DataSet GetStlDataSourceBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
        // {
        //     var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetStlDataSourceBySiteId),
        //                siteId.ToString(), startNum.ToString(), totalNum.ToString(), whereString, orderByString);
        //     var retval = StlCacheManager.Get<DataSet>(cacheKey);
        //     if (retval != null) return retval;

        //     lock (LockObject)
        //     {
        //         retval = StlCacheManager.Get<DataSet>(cacheKey);
        //         if (retval == null)
        //         {
        //             retval = DataProvider.ChannelDao.GetStlDataSourceBySiteId(siteId, startNum, totalNum, whereString, orderByString);
        //             StlCacheManager.Set(cacheKey, retval);
        //         }
        //     }

        //     return retval;
        // }

        public static IList<KeyValuePair<int, ChannelInfo>> GetContainerChannelList(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, TaxisType taxisType, ScopeType scopeType, bool isTotal)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetContainerChannelList), siteId.ToString(), channelId.ToString(), group.ToString(), groupNot.ToString(), isImage.ToString(), startNum.ToString(), totalNum.ToString(), TaxisTypeUtils.GetValue(taxisType), scopeType.Value, isTotal.ToString());
            var retval = StlCacheManager.Get<IList<KeyValuePair<int, ChannelInfo>>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<IList<KeyValuePair<int, ChannelInfo>>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelRepository.GetContainerChannelList(siteId, channelId, group, groupNot, isImage, startNum, totalNum, taxisType, scopeType, isTotal);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        // public static DataSet GetStlDataSet(List<int> channelIdList, int startNum, int totalNum, string whereString, string orderByString)
        // {
        //     var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetStlDataSet),
        //                TranslateUtils.ObjectCollectionToString(channelIdList), startNum.ToString(), totalNum.ToString(), whereString, orderByString);
        //     var retval = StlCacheManager.Get<DataSet>(cacheKey);
        //     if (retval != null) return retval;

        //     lock (LockObject)
        //     {
        //         retval = StlCacheManager.Get<DataSet>(cacheKey);
        //         if (retval == null)
        //         {
        //             retval = DataProvider.ChannelDao.GetStlDataSet(channelIdList, startNum, totalNum, whereString, orderByString);
        //             StlCacheManager.Set(cacheKey, retval);
        //         }
        //     }

        //     return retval;
        // }

        // public static List<Container.Channel> GetContainerChannelList(List<int> channelIdList, int startNum, int totalNum, string whereString, string orderByString)
        // {
        //     var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetContainerChannelList),
        //                TranslateUtils.ObjectCollectionToString(channelIdList), startNum.ToString(), totalNum.ToString(), whereString, orderByString);
        //     var retval = StlCacheManager.Get<List<Container.Channel>>(cacheKey);
        //     if (retval != null) return retval;

        //     lock (LockObject)
        //     {
        //         retval = StlCacheManager.Get<List<Container.Channel>>(cacheKey);
        //         if (retval == null)
        //         {
        //             retval = DataProvider.ChannelDao.GetContainerChannelList(channelIdList, startNum, totalNum, whereString, orderByString);
        //             StlCacheManager.Set(cacheKey, retval);
        //         }
        //     }

        //     return retval;
        // }

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
                    retval = DataProvider.ChannelRepository.GetIdByParentIdAndTaxis(parentId, taxis, isNextChannel);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static IList<int> GetIdListByTotalNum(int siteId, int channelId, TaxisType taxisType, ScopeType scopeType, string groupChannel, string groupChannelNot, bool? isImage, int totalNum)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlChannelCache), nameof(GetIdListByTotalNum),
                       siteId.ToString(), channelId.ToString(), TaxisTypeUtils.GetValue(taxisType), scopeType.Value, groupChannel, groupChannelNot, isImage.ToString(), totalNum.ToString());
            var retval = StlCacheManager.Get<IList<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<IList<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ChannelRepository.GetIdListByTotalNum(siteId, channelId, taxisType, scopeType, groupChannel, groupChannelNot, isImage, totalNum);
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
                    retval = DataProvider.ChannelRepository.GetChannelInfoByLastAddDate(channelId);
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
                    retval = DataProvider.ChannelRepository.GetChannelInfoByTaxis(channelId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
