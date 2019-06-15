using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class AreaRepository
    {
        private readonly object _lockObject = new object();

        public List<KeyValuePair<int, string>> GetRestAreas()
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

        public string GetTreeItem(string name, int parentsCount, bool isLastNode, Dictionary<int, bool> parentsCountDict)
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
                str = string.Concat(str, TranslateUtils.DictGetValue(parentsCountDict, i) ? "��" : "��");
            }
            str = string.Concat(str, isLastNode ? "��" : "��");
            str = string.Concat(str, name);
            return str;
        }

        public AreaInfo GetAreaInfo(int areaId)
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

        public string GetThisAreaName(int areaId)
        {
            var areaInfo = GetAreaInfo(areaId);
            if (areaInfo != null)
            {
                return areaInfo.AreaName;
            }
            return string.Empty;
        }

        public string GetAreaName(int areaId)
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

        public string GetParentsPath(int areaId)
        {
            var retval = string.Empty;
            var areaInfo = GetAreaInfo(areaId);
            if (areaInfo != null)
            {
                retval = areaInfo.ParentsPath;
            }
            return retval;
        }

        public List<int> GetAreaIdList()
        {
            var pairList = GetAreaInfoPairList();
            var list = new List<int>();
            foreach (var pair in pairList)
            {
                list.Add(pair.Key);
            }
            return list;
        }

        public void ClearCache()
        {
            lock (_lockObject)
            {
                DataCacheManager.Remove(CacheKey);
            }
        }

        private List<KeyValuePair<int, AreaInfo>> GetAreaInfoPairList()
        {
            lock (_lockObject)
            {
                var list = DataCacheManager.Get<List<KeyValuePair<int, AreaInfo>>>(CacheKey);
                if (list != null) return list;

                var pairListFormDb = GetAreaInfoPairListToCache();
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
