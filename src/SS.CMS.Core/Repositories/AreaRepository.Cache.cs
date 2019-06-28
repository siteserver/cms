using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class AreaRepository
    {
        public async Task<List<KeyValuePair<int, string>>> GetRestAreasAsync()
        {
            var list = new List<KeyValuePair<int, string>>();

            var areaIdList = await GetAreaIdListAsync();
            var parentsCountDict = new Dictionary<int, bool>();
            foreach (var areaId in areaIdList)
            {
                var areaInfo = await GetAreaInfoAsync(areaId);
                list.Add(new KeyValuePair<int, string>(areaId, GetTreeItem(areaInfo.AreaName, areaInfo.ParentsCount, areaInfo.IsLastNode, parentsCountDict)));
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

        public async Task<AreaInfo> GetAreaInfoAsync(int areaId)
        {
            var pairList = await GetAreaInfoPairListAsync();

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

        public async Task<string> GetThisAreaNameAsync(int areaId)
        {
            var areaInfo = await GetAreaInfoAsync(areaId);
            if (areaInfo != null)
            {
                return areaInfo.AreaName;
            }
            return string.Empty;
        }

        public async Task<string> GetAreaNameAsync(int areaId)
        {
            if (areaId <= 0) return string.Empty;

            var areaNameList = new List<string>();

            var parentsPath = await GetParentsPathAsync(areaId);
            var areaIdList = new List<int>();
            if (!string.IsNullOrEmpty(parentsPath))
            {
                areaIdList = TranslateUtils.StringCollectionToIntList(parentsPath);
            }
            areaIdList.Add(areaId);

            foreach (var theAreaId in areaIdList)
            {
                var areaInfo = await GetAreaInfoAsync(theAreaId);
                if (areaInfo != null)
                {
                    areaNameList.Add(areaInfo.AreaName);
                }
            }

            return TranslateUtils.ObjectCollectionToString(areaNameList, " > ");
        }

        public async Task<string> GetParentsPathAsync(int areaId)
        {
            var retval = string.Empty;
            var areaInfo = await GetAreaInfoAsync(areaId);
            if (areaInfo != null)
            {
                retval = areaInfo.ParentsPath;
            }
            return retval;
        }

        public async Task<List<int>> GetAreaIdListAsync()
        {
            var pairList = await GetAreaInfoPairListAsync();
            var list = new List<int>();
            foreach (var pair in pairList)
            {
                list.Add(pair.Key);
            }
            return list;
        }

        private void ClearCache()
        {
            _cacheManager.Remove(CacheKey);
        }

        private async Task<List<KeyValuePair<int, AreaInfo>>> GetAreaInfoPairListAsync()
        {
            var list = _cacheManager.Get<List<KeyValuePair<int, AreaInfo>>>(CacheKey);
            if (list != null) return list;

            var pairListFormDb = await GetAreaInfoPairListToCacheAsync();
            list = new List<KeyValuePair<int, AreaInfo>>();
            foreach (var pair in pairListFormDb)
            {
                var areaInfo = pair.Value;
                if (areaInfo != null)
                {
                    list.Add(pair);
                }
            }
            _cacheManager.Insert(CacheKey, list);
            return list;
        }
    }
}
