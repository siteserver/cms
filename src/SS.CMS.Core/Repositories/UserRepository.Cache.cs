using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Repositories
{
    public partial class UserRepository
    {
        public void ClearCache()
        {
            _cacheManager.Remove(CacheKey);
        }

        public void UpdateCache(UserInfo userInfo)
        {
            if (userInfo == null) return;

            var dict = GetDict();

            dict[GetDictKeyByUserId(userInfo.Id)] = userInfo;
            dict[GetDictKeyByUserName(userInfo.UserName)] = userInfo;
            if (!string.IsNullOrEmpty(userInfo.Mobile))
            {
                dict[GetDictKeyByMobile(userInfo.Mobile)] = userInfo;
            }
            if (!string.IsNullOrEmpty(userInfo.Email))
            {
                dict[GetDictKeyByEmail(userInfo.Email)] = userInfo;
            }
        }

        public void RemoveCache(UserInfo userInfo)
        {
            if (userInfo == null) return;

            var dict = GetDict();

            dict.Remove(GetDictKeyByUserId(userInfo.Id));
            dict.Remove(GetDictKeyByUserName(userInfo.UserName));
            if (!string.IsNullOrEmpty(userInfo.Mobile))
            {
                dict.Remove(GetDictKeyByMobile(userInfo.Mobile));
            }

            if (!string.IsNullOrEmpty(userInfo.Email))
            {
                dict.Remove(GetDictKeyByEmail(userInfo.Email));
            }
        }

        private string GetDictKeyByUserId(int userId)
        {
            return $"userId:{userId}";
        }

        private string GetDictKeyByUserName(string userName)
        {
            return $"userName:{userName}";
        }

        private string GetDictKeyByMobile(string mobile)
        {
            return $"mobile:{mobile}";
        }

        private string GetDictKeyByEmail(string email)
        {
            return $"email:{email}";
        }

        public UserInfo GetUserInfoByUserId(int userId)
        {
            if (userId <= 0) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByUserId(userId), out UserInfo userInfo);
            if (userInfo != null) return userInfo;

            dict.TryGetValue(GetDictKeyByUserId(userId), out userInfo);

            if (userInfo == null)
            {
                userInfo = GetByUserId(userId);
                if (userInfo != null)
                {
                    dict[GetDictKeyByUserId(userInfo.Id)] = userInfo;
                    dict[GetDictKeyByUserName(userInfo.UserName)] = userInfo;
                    if (!string.IsNullOrEmpty(userInfo.Mobile))
                    {
                        dict[GetDictKeyByMobile(userInfo.Mobile)] = userInfo;
                    }
                    if (!string.IsNullOrEmpty(userInfo.Email))
                    {
                        dict[GetDictKeyByEmail(userInfo.Email)] = userInfo;
                    }
                }
            }

            return userInfo;
        }

        public UserInfo GetUserInfoByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByUserName(userName), out UserInfo userInfo);
            if (userInfo != null) return userInfo;

            dict.TryGetValue(GetDictKeyByUserName(userName), out userInfo);

            if (userInfo == null)
            {
                userInfo = GetByUserName(userName);
                if (userInfo != null)
                {
                    dict[GetDictKeyByUserId(userInfo.Id)] = userInfo;
                    dict[GetDictKeyByUserName(userInfo.UserName)] = userInfo;
                    if (!string.IsNullOrEmpty(userInfo.Mobile))
                    {
                        dict[GetDictKeyByMobile(userInfo.Mobile)] = userInfo;
                    }
                    if (!string.IsNullOrEmpty(userInfo.Email))
                    {
                        dict[GetDictKeyByEmail(userInfo.Email)] = userInfo;
                    }
                }
            }

            return userInfo;
        }

        public async Task<UserInfo> GetUserInfoByUserNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByUserName(userName), out UserInfo userInfo);
            if (userInfo != null) return userInfo;

            dict.TryGetValue(GetDictKeyByUserName(userName), out userInfo);

            if (userInfo == null)
            {
                userInfo = await GetByUserNameAsync(userName);
                if (userInfo != null)
                {
                    dict[GetDictKeyByUserId(userInfo.Id)] = userInfo;
                    dict[GetDictKeyByUserName(userInfo.UserName)] = userInfo;
                    if (!string.IsNullOrEmpty(userInfo.Mobile))
                    {
                        dict[GetDictKeyByMobile(userInfo.Mobile)] = userInfo;
                    }
                    if (!string.IsNullOrEmpty(userInfo.Email))
                    {
                        dict[GetDictKeyByEmail(userInfo.Email)] = userInfo;
                    }
                }
            }

            return userInfo;
        }

        public UserInfo GetUserInfoByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByMobile(mobile), out UserInfo userInfo);
            if (userInfo != null) return userInfo;

            dict.TryGetValue(GetDictKeyByMobile(mobile), out userInfo);

            if (userInfo == null)
            {
                userInfo = GetByMobile(mobile);
                if (userInfo != null)
                {
                    dict[GetDictKeyByUserId(userInfo.Id)] = userInfo;
                    dict[GetDictKeyByUserName(userInfo.UserName)] = userInfo;
                    if (!string.IsNullOrEmpty(userInfo.Mobile))
                    {
                        dict[GetDictKeyByMobile(userInfo.Mobile)] = userInfo;
                    }
                    if (!string.IsNullOrEmpty(userInfo.Email))
                    {
                        dict[GetDictKeyByEmail(userInfo.Email)] = userInfo;
                    }
                }
            }

            return userInfo;
        }

        public UserInfo GetUserInfoByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByEmail(email), out UserInfo userInfo);
            if (userInfo != null) return userInfo;

            dict.TryGetValue(GetDictKeyByEmail(email), out userInfo);

            if (userInfo == null)
            {
                userInfo = GetByEmail(email);
                if (userInfo != null)
                {
                    dict[GetDictKeyByUserId(userInfo.Id)] = userInfo;
                    dict[GetDictKeyByUserName(userInfo.UserName)] = userInfo;
                    if (!string.IsNullOrEmpty(userInfo.Mobile))
                    {
                        dict[GetDictKeyByMobile(userInfo.Mobile)] = userInfo;
                    }
                    if (!string.IsNullOrEmpty(userInfo.Email))
                    {
                        dict[GetDictKeyByEmail(userInfo.Email)] = userInfo;
                    }
                }
            }

            return userInfo;
        }

        private Dictionary<string, UserInfo> GetDict()
        {
            var dict = _cacheManager.Get<Dictionary<string, UserInfo>>(CacheKey);
            if (dict != null) return dict;

            dict = _cacheManager.Get<Dictionary<string, UserInfo>>(CacheKey);
            if (dict == null)
            {
                dict = new Dictionary<string, UserInfo>();
                _cacheManager.Insert(CacheKey, dict);
            }

            return dict;
        }

        public UserInfo GetUserInfoByAccount(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;

            if (StringUtils.IsMobile(account))
            {
                return GetUserInfoByMobile(account);
            }
            if (StringUtils.IsEmail(account))
            {
                return GetUserInfoByEmail(account);
            }

            return GetUserInfoByUserName(account);
        }

        public List<int> GetLatestTop10SiteIdList(List<int> siteIdListLatestAccessed, List<int> siteIdListOrderByLevel, List<int> siteIdListWithPermissions)
        {
            var siteIdList = new List<int>();

            foreach (var siteId in siteIdListLatestAccessed)
            {
                if (siteIdList.Count >= 10) break;
                if (!siteIdList.Contains(siteId) && siteIdListWithPermissions.Contains(siteId))
                {
                    siteIdList.Add(siteId);
                }
            }

            if (siteIdList.Count < 10)
            {
                // var siteIdListOrderByLevel = SiteManager.GetSiteIdListOrderByLevel();
                foreach (var siteId in siteIdListOrderByLevel)
                {
                    if (siteIdList.Count >= 5) break;
                    if (!siteIdList.Contains(siteId) && siteIdListWithPermissions.Contains(siteId))
                    {
                        siteIdList.Add(siteId);
                    }
                }
            }

            return siteIdList;
        }

        public string GetDisplayName(string userName)
        {
            var userInfo = GetUserInfoByUserName(userName);
            if (userInfo == null) return userName;

            return userInfo.DisplayName;
        }
    }
}