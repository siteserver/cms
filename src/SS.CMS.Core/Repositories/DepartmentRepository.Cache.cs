using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class DepartmentRepository
    {
        private readonly object _lockObject = new object();

        public List<KeyValuePair<int, string>> GetRestDepartments()
        {
            var list = new List<KeyValuePair<int, string>>();

            var departmentIdList = GetDepartmentIdList();
            var parentsCountDict = new Dictionary<int, bool>();
            foreach (var departmentId in departmentIdList)
            {
                var departmentInfo = GetDepartmentInfo(departmentId);
                list.Add(new KeyValuePair<int, string>(departmentId, GetTreeItem(departmentInfo.DepartmentName, departmentInfo.ParentsCount, departmentInfo.LastNode, parentsCountDict)));
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

        public DepartmentInfo GetDepartmentInfo(int departmentId)
        {
            var pairList = GetDepartmentInfoKeyValuePair();

            foreach (var pair in pairList)
            {
                if (pair.Key == departmentId)
                {
                    return pair.Value;
                }
            }
            return null;
        }

        public string GetDepartmentName(int departmentId)
        {
            if (departmentId <= 0) return string.Empty;

            var departmentNameList = new List<string>();

            var parentsPath = GetParentsPath(departmentId);
            var departmentIdList = new List<int>();
            if (!string.IsNullOrEmpty(parentsPath))
            {
                departmentIdList = TranslateUtils.StringCollectionToIntList(parentsPath);
            }
            departmentIdList.Add(departmentId);

            foreach (var theDepartmentId in departmentIdList)
            {
                var departmentInfo = GetDepartmentInfo(theDepartmentId);
                if (departmentInfo != null)
                {
                    departmentNameList.Add(departmentInfo.DepartmentName);
                }
            }

            return TranslateUtils.ObjectCollectionToString(departmentNameList, " > ");
        }

        public string GetParentsPath(int departmentId)
        {
            var retval = string.Empty;
            var departmentInfo = GetDepartmentInfo(departmentId);
            if (departmentInfo != null)
            {
                retval = departmentInfo.ParentsPath;
            }
            return retval;
        }

        public List<int> GetDepartmentIdList()
        {
            var pairList = GetDepartmentInfoKeyValuePair();
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

        public List<KeyValuePair<int, DepartmentInfo>> GetDepartmentInfoKeyValuePair()
        {
            lock (_lockObject)
            {
                var list = DataCacheManager.Get<List<KeyValuePair<int, DepartmentInfo>>>(CacheKey);
                if (list != null) return list;

                list = GetDepartmentInfoKeyValuePairToCache();
                DataCacheManager.Insert(CacheKey, list);
                return list;
            }
        }
    }
}