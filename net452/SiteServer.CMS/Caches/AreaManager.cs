using System.Collections.Generic;
using SiteServer.CMS.Caches.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Caches
{
    public static class AreaManager
    {
        private static readonly object LockObject = new object();
        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(AreaManager));

        public static List<KeyValuePair<int, string>> GetRestAreas()
        {
            var list = new List<KeyValuePair<int, string>>();

            var areaIdList = GetAreaIdList();
            var parentsCountDict = new Dictionary<int, bool>();
            foreach (var areaId in areaIdList)
            {
                var areaInfo = GetAreaInfo(areaId);
                list.Add(new KeyValuePair<int, string>(areaId, GetTreeItem(areaInfo.AreaName, areaInfo.ParentsCount, areaInfo.LastNode, parentsCountDict)));
            }

            return list;
        }

        public static string GetTreeItem(string name, int parentsCount, bool isLastNode, Dictionary<int, bool> parentsCountDict)
        {
            var str = "";
            if (isLastNode == false)
            {
                parentsCountDict[parentsCount] = false;
            }
            else
            {
                parentsCountDict[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, TranslateUtils.DictGetValue(parentsCountDict, i) ? "¡¡" : "©¦");
            }
            str = string.Concat(str, isLastNode ? "©¸" : "©À");
            str = string.Concat(str, name);
            return str;
        }

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
            lock (LockObject)
            {
                DataCacheManager.Remove(CacheKey);
            }
        }

        public static List<KeyValuePair<int, AreaInfo>> GetAreaInfoPairList()
        {
            lock (LockObject)
            {
                var list = DataCacheManager.Get<List<KeyValuePair<int, AreaInfo>>>(CacheKey);
                if (list != null) return list;

                var pairListFormDb = DataProvider.Area.GetAreaInfoPairList();
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