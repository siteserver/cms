using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache
{
    public static class AreaManager
    {
        private static readonly object LockObject = new object();
        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(AreaManager));

        public static AreaInfo GetAreaInfo(int areaId)
        {
            var pairList = GetAreaInfoPairList();

            foreach (var pair in pairList)
            {
                var theAreaId = pair.Key;
                if (theAreaId == areaId)
                {
                    var areaInfo = pair.Value;
                    return areaInfo;
                }
            }
            return null;
        }

        public static string GetThisAreaName(int areaId)
        {
            var areaInfo = GetAreaInfo(areaId);
            if (areaInfo != null)
            {
                return areaInfo.AreaName;
            }
            return string.Empty;
        }

        public static string GetAreaName(int areaId)
        {
            if (areaId <= 0) return string.Empty;

            var areaNameList = new List<string>();

            var parentsPath = GetParentsPath(areaId);
            var areaIdList = new List<int>();
            if (!string.IsNullOrEmpty(parentsPath))
            {
                areaIdList = TranslateUtils.StringCollectionToIntList(parentsPath);
            }
            areaIdList.Add(areaId);

            foreach (var theAreaId in areaIdList)
            {
                var areaInfo = GetAreaInfo(theAreaId);
                if (areaInfo != null)
                {
                    areaNameList.Add(areaInfo.AreaName);
                }
            }

            return TranslateUtils.ObjectCollectionToString(areaNameList, " > ");
        }

        public static string GetParentsPath(int areaId)
        {
            var retval = string.Empty;
            var areaInfo = GetAreaInfo(areaId);
            if (areaInfo != null)
            {
                retval = areaInfo.ParentsPath;
            }
            return retval;
        }

        public static List<int> GetAreaIdList()
        {
            var pairList = GetAreaInfoPairList();
            var list = new List<int>();
            foreach (var pair in pairList)
            {
                list.Add(pair.Key);
            }
            return list;
        }

        public static void ClearCache()
        {
            DataCacheManager.Remove(CacheKey);
        }

        public static List<KeyValuePair<int, AreaInfo>> GetAreaInfoPairList()
        {
            lock (LockObject)
            {
                var list = DataCacheManager.Get<List<KeyValuePair<int, AreaInfo>>>(CacheKey);
                if (list != null) return list;

                var pairListFormDb = DataProvider.AreaDao.GetAreaInfoPairList();
                list = new List<KeyValuePair<int, AreaInfo>>();
                foreach (var pair in pairListFormDb)
                {
                    var areaInfo = pair.Value;
                    if (areaInfo != null)
                    {
                        list.Add(pair);
                    }
                }
                DataCacheManager.Insert(CacheKey, list);
                return list;
            }
        }
    }
}