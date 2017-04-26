using System.Collections.Generic;
using BaiRong.Core.Model;

namespace BaiRong.Core
{
    public class DepartmentManager
    {
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
            CacheUtils.Remove(CacheKey);
        }

        public static List<KeyValuePair<int, DepartmentInfo>> GetDepartmentInfoKeyValuePair()
        {
            lock (LockObject)
            {
                if (CacheUtils.Get(CacheKey) == null)
                {
                    var list = BaiRongDataProvider.DepartmentDao.GetDepartmentInfoKeyValuePair();
                    CacheUtils.Max(CacheKey, list);
                    return list;
                }
                return CacheUtils.Get(CacheKey) as List<KeyValuePair<int, DepartmentInfo>>;
            }
        }

        /****************** Cache *********************/

        private static readonly object LockObject = new object();
        private const string CacheKey = "BaiRong.Core.DepartmentManager";
    }
}