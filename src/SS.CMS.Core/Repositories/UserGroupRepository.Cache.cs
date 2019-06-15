using System.Collections.Generic;
using System.Linq;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Core;

namespace SS.CMS.Core.Repositories
{
    public partial class UserGroupRepository
    {
        private readonly object _lockObject = new object();

        private void ClearCache()
        {
            DataCacheManager.Remove(CacheKey);
        }

        public IList<UserGroupInfo> GetAllUserGroups()
        {
            var list = DataCacheManager.Get<IList<UserGroupInfo>>(CacheKey);
            if (list == null)
            {
                lock (_lockObject)
                {
                    list = DataCacheManager.Get<IList<UserGroupInfo>>(CacheKey);
                    if (list == null)
                    {
                        list = GetUserGroupInfoListToCache() ?? new List<UserGroupInfo>();

                        DataCacheManager.Insert(CacheKey, list);
                    }
                }
            }

            list.Insert(0, new UserGroupInfo
            {
                Id = 0,
                GroupName = "默认用户组",
                AdminName = _settingsManager.ConfigInfo.UserDefaultGroupAdminName
            });

            return list;
        }

        public bool IsExists(string groupName)
        {
            var list = GetAllUserGroups();
            return list.Any(group => group.GroupName == groupName);
        }

        public UserGroupInfo GetUserGroupInfo(int groupId)
        {
            var list = GetAllUserGroups();
            return list.FirstOrDefault(group => group.Id == groupId) ?? list[0];
        }
    }
}
