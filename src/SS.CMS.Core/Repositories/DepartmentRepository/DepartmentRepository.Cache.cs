using System.Collections.Generic;
using SS.CMS.Models;
using SS.CMS.Utils;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace SS.CMS.Core.Repositories
{
    public partial class DepartmentRepository
    {
        public async Task<List<KeyValuePair<int, string>>> GetRestDepartmentsAsync()
        {
            var list = new List<KeyValuePair<int, string>>();

            var departmentIdList = await GetDepartmentIdListAsync();
            var parentsCountDict = new Dictionary<int, bool>();
            foreach (var departmentId in departmentIdList)
            {
                var departmentInfo = await GetDepartmentInfoAsync(departmentId);
                list.Add(new KeyValuePair<int, string>(departmentId, GetTreeItem(departmentInfo.DepartmentName, departmentInfo.ParentsCount, departmentInfo.IsLastNode, parentsCountDict)));
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

        public async Task<DepartmentInfo> GetDepartmentInfoAsync(int departmentId)
        {
            var pairList = await GetDepartmentInfoKeyValuePairAsync();

            foreach (var pair in pairList)
            {
                if (pair.Key == departmentId)
                {
                    return pair.Value;
                }
            }
            return null;
        }

        public async Task<string> GetDepartmentNameAsync(int departmentId)
        {
            if (departmentId <= 0) return string.Empty;

            var departmentNameList = new List<string>();

            var parentsPath = await GetParentsPathAsync(departmentId);
            var departmentIdList = new List<int>();
            if (!string.IsNullOrEmpty(parentsPath))
            {
                departmentIdList = TranslateUtils.StringCollectionToIntList(parentsPath);
            }
            departmentIdList.Add(departmentId);

            foreach (var theDepartmentId in departmentIdList)
            {
                var departmentInfo = await GetDepartmentInfoAsync(theDepartmentId);
                if (departmentInfo != null)
                {
                    departmentNameList.Add(departmentInfo.DepartmentName);
                }
            }

            return TranslateUtils.ObjectCollectionToString(departmentNameList, " > ");
        }

        public async Task<string> GetParentsPathAsync(int departmentId)
        {
            var retval = string.Empty;
            var departmentInfo = await GetDepartmentInfoAsync(departmentId);
            if (departmentInfo != null)
            {
                retval = departmentInfo.ParentsPath;
            }
            return retval;
        }

        public async Task<List<int>> GetDepartmentIdListAsync()
        {
            var pairList = await GetDepartmentInfoKeyValuePairAsync();
            var list = new List<int>();
            foreach (var pair in pairList)
            {
                list.Add(pair.Key);
            }
            return list;
        }

        public async Task<List<KeyValuePair<int, DepartmentInfo>>> GetDepartmentInfoKeyValuePairAsync()
        {
            return await _cache.GetOrCreateAsync(_cacheKey, async options =>
            {
                return await GetDepartmentInfoKeyValuePairToCacheAsync();
            });
        }
    }
}