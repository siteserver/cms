using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache
{
    public static class DepartmentManager
    {
        private static readonly object LockObject = new object();
        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(DepartmentManager));

        public static DepartmentInfo GetDepartmentInfo(int departmentId)
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

        public static string GetThisDepartmentName(int departmentId)
        {
            var departmentInfo = GetDepartmentInfo(departmentId);
            if (departmentInfo != null)
            {
                return departmentInfo.DepartmentName;
            }
            return string.Empty;
        }

        public static string GetDepartmentName(int departmentId)
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

        public static string GetDepartmentCode(int departmentId)
        {
            if (departmentId > 0)
            {
                var departmentInfo = GetDepartmentInfo(departmentId);
                if (departmentInfo != null)
                {
                    return departmentInfo.Code;
                }
            }
            return string.Empty;
        }

        public static string GetParentsPath(int departmentId)
        {
            var retval = string.Empty;
            var departmentInfo = GetDepartmentInfo(departmentId);
            if (departmentInfo != null)
            {
                retval = departmentInfo.ParentsPath;
            }
            return retval;
        }

        public static List<int> GetDepartmentIdList()
        {
            var pairList = GetDepartmentInfoKeyValuePair();
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

        public static List<KeyValuePair<int, DepartmentInfo>> GetDepartmentInfoKeyValuePair()
        {
            lock (LockObject)
            {
                var list = DataCacheManager.Get<List<KeyValuePair<int, DepartmentInfo>>>(CacheKey);
                if (list != null) return list;

                list = DataProvider.DepartmentDao.GetDepartmentInfoKeyValuePair();
                DataCacheManager.Insert(CacheKey, list);
                return list;
            }
        }
    }
}