using System.Collections;
using System.Collections.Generic;
using BaiRong.Core.Model;

namespace BaiRong.Core
{
    public class AreaManager
    {
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
            if (areaId > 0)
            {
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
            return string.Empty;
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
            CacheUtils.Remove(CacheKey);
        }

        public static List<KeyValuePair<int, AreaInfo>> GetAreaInfoPairList()
        {
            lock (LockObject)
            {
                if (CacheUtils.Get(CacheKey) == null)
                {
                    var pairListFormDb = BaiRongDataProvider.AreaDao.GetAreaInfoPairList();
                    var sl = new List<KeyValuePair<int, AreaInfo>>();
                    foreach (var pair in pairListFormDb)
                    {
                        var areaInfo = pair.Value;
                        if (areaInfo != null)
                        {
                            sl.Add(pair);
                        }
                    }
                    CacheUtils.Max(CacheKey, sl);
                    return sl;
                }
                return CacheUtils.Get(CacheKey) as List<KeyValuePair<int, AreaInfo>>;
            }
        }

        /****************** Cache *********************/

        private static readonly object LockObject = new object();
        private const string CacheKey = "BaiRong.Core.AreaManager";
    }
}