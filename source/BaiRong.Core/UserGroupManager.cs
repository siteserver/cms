using System.Collections.Generic;
using BaiRong.Core.Model;

namespace BaiRong.Core
{
    public class UserGroupManager
	{
        private UserGroupManager()
		{
			
		}

        public static UserGroupInfo GetDefaultGroupInfo()
        {
            UserGroupInfo groupInfo = null;

            var pairList = GetUserGroupInfoPairList();

            foreach (var pair in pairList)
            {
                groupInfo = pair.Value;
                if (groupInfo.IsDefault)
                {
                    break;
                }
            }

            if (groupInfo == null)
            {
                groupInfo = BaiRongDataProvider.UserGroupDao.AddDefaultGroup();
            }

            return groupInfo;
        }

        public static UserGroupInfo GetGroupInfo(int groupId)
		{
            UserGroupInfo groupInfo = null;

            var pairList = GetUserGroupInfoPairList();

            foreach (var pair in pairList)
            {
                var theGroupId = pair.Key;
                if (theGroupId == groupId)
                {
                    groupInfo = pair.Value;
                    break;
                }
            }

            if (groupInfo == null)
            {
                groupInfo = GetDefaultGroupInfo();
            }

            return groupInfo;
		}

        public static List<int> GetGroupIdList()
		{
            var list = new List<int>();
            var pairList = GetUserGroupInfoPairList();
		    foreach (var pair in pairList)
		    {
		        list.Add(pair.Key);
		    }
            return list;
		}

        public static bool IsGroupIdInGroupNames(int groupId, List<string> groupNameList)
        {
            var pairList = GetUserGroupInfoPairList();
            foreach (var pair in pairList)
            {
                var groupInfo = pair.Value;
                if (groupNameList.Contains(groupInfo.GroupName) && groupInfo.GroupId == groupId)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsExists(int groupId)
        {
            return groupId > 0 && GetGroupIdList().Contains(groupId);
        }

        public static void ClearCache()
		{
            var cacheKey = GetCacheKey();
            CacheUtils.Remove(cacheKey);
		}

        public static List<KeyValuePair<int, UserGroupInfo>> GetUserGroupInfoPairList()
        {
            lock (LockObject)
            {
                var cacheKey = GetCacheKey();
                if (CacheUtils.Get(cacheKey) == null)
                {
                    var pairListFromDb = BaiRongDataProvider.UserGroupDao.GetUserGroupInfoPairList();
                    if (pairListFromDb.Count == 0)
                    {
                        pairListFromDb = BaiRongDataProvider.UserGroupDao.GetUserGroupInfoPairList();
                    }
                    var sl = new List<KeyValuePair<int, UserGroupInfo>>();
                    foreach (var pair in pairListFromDb)
                    {
                        var groupInfo = pair.Value;
                        //配置路径
                        if (groupInfo != null)
                        {
                            sl.Add(pair);
                        }
                    }
                    CacheUtils.Max(cacheKey, sl);
                    return sl;
                }
                return CacheUtils.Get(cacheKey) as List<KeyValuePair<int, UserGroupInfo>>;
            }
        }

        private static readonly object LockObject = new object();

        private static string GetCacheKey()
        {
            return "BaiRong.Core.UserGroupManager";
        }

        public static string GetGroupName(int groupId)
        {
            var groupName = string.Empty;

            var groupInfo = GetGroupInfo(groupId);

            if (groupInfo != null)
            {
                groupName = groupInfo.GroupName;
            }

            return groupName;
        }

        public static List<UserGroupInfo> GetGroupInfoList()
        {
            var list = new List<UserGroupInfo>();
            var pairList = GetUserGroupInfoPairList();

            foreach (var pair in pairList)
            {
                var groupInfo = pair.Value;
                list.Add(groupInfo);
            }
            return list;
        }
	}
}
