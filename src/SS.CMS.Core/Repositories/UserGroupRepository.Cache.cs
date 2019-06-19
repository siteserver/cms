using System.Collections.Generic;
using System.Linq;
using SS.CMS.Models;

namespace SS.CMS.Core.Repositories
{
    public partial class UserGroupRepository
    {
        private void ClearCache()
        {
            _cacheManager.Remove(CacheKey);
        }

        public IList<UserGroupInfo> GetAllUserGroups()
        {
            var list = _cacheManager.Get<IList<UserGroupInfo>>(CacheKey);
            if (list == null)
            {
                list = _cacheManager.Get<IList<UserGroupInfo>>(CacheKey);
                if (list == null)
                {
                    list = GetUserGroupInfoListToCache() ?? new List<UserGroupInfo>();

                    _cacheManager.Insert(CacheKey, list);
                }
            }

            list.Insert(0, new UserGroupInfo
            {
                Id = 0,
                GroupName = "默认用户组",
                AdminName = _configRepository.Instance.UserDefaultGroupAdminName
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
