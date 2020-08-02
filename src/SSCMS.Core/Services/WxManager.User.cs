using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Wx;

namespace SSCMS.Core.Services
{
    public partial class WxManager
    {
        public async Task<List<WxUserTag>> GetUserTagsAsync(string token)
        {
            return (await UserTagApi.GetAsync(token)).tags.Select(tag =>
                new WxUserTag
                {
                    Id = tag.id,
                    Name = tag.name,
                    Count = tag.count
                }).ToList();
        }

        public async Task<List<string>> GetUserOpenIdsAsync(string token)
        {
            var cacheKey = CacheUtils.GetClassKey(typeof(WxManager), token);
            var openIds = _openIdCacheManager.Get(cacheKey);
            if (openIds == null)
            {
                openIds = new List<string>();
                string nextOpenId = null;
                while (true)
                {
                    nextOpenId = await LoadUserOpenIdsAsync(openIds, token, nextOpenId);
                    if (string.IsNullOrEmpty(nextOpenId)) break;
                }

                _openIdCacheManager.AddOrUpdateAbsolute(cacheKey, openIds, 30);
            }

            return openIds;
        }

        public async Task<List<WxUser>> GetUsersAsync(string token, List<string> openIds)
        {
            var cacheKey = CacheUtils.GetClassKey(typeof(WxManager), ListUtils.ToString(openIds));
            var users = _userCacheManager.Get(cacheKey);
            if (users == null)
            {
                users = new List<WxUser>();
                var num = TranslateUtils.Ceiling(openIds.Count, 100);
                for (var i = 0; i < num; i++)
                {
                    var pageOpenIds = openIds.Skip(i * 100).Take(100);

                    var userList = pageOpenIds.Select(openId => new BatchGetUserInfoData
                        {
                            openid = openId,
                            LangEnum = Language.zh_CN
                        })
                        .ToList();
                    var userResult = await UserApi.BatchGetUserInfoAsync(token, userList);
                    users.AddRange(userResult.user_info_list.Select(GetWxUser));
                }

                users = users.OrderByDescending(x => x.SubscribeTime).ToList();

                _userCacheManager.AddOrUpdateAbsolute(cacheKey, users, 30);
            }

            return users;
        }

        public async Task<WxUser> GetUserAsync(string token, string openId)
        {
            if (string.IsNullOrEmpty(openId)) return null;

            var userResult = await UserApi.InfoAsync(token, openId);

            return GetWxUser(userResult);
        }

        private async Task<string> LoadUserOpenIdsAsync(List<string> openIds, string token, string nextOpenId)
        {
            var openIdResult = await UserApi.GetAsync(token, nextOpenId);
            openIds.AddRange(openIdResult.data.openid);

            return openIds.Count != openIdResult.total ? openIdResult.next_openid : null;
        }

        private static WxUser GetWxUser(UserInfoJson json)
        {
            return new WxUser
            {
                Subscribe = json.subscribe,
                OpenId = json.openid,
                Nickname = json.nickname,
                Sex = json.sex,
                Language = json.language,
                City = json.city,
                Province = json.province,
                Country = json.country,
                HeadImgUrl = json.headimgurl,
                SubscribeTime = GetDateTimeWithTimeStamp(json.subscribe_time),
                UnionId = json.unionid,
                Remark = json.remark,
                GroupId = json.groupid,
                TagIdList = ListUtils.GetIntList(json.tagid_list),
                SubscribeScene = json.subscribe_scene,
                QrScene = json.qr_scene,
                QrSceneStr = json.qr_scene_str
            };
        }

        /// <summary>
        /// 时间戳转换为datetime
        /// </summary>
        /// <param name="timeStamp">微信接口返回时间戳</param>
        /// <returns></returns>
        private static DateTime GetDateTimeWithTimeStamp(long timeStamp)
        {
            var startTime = new DateTime(1970, 1, 1); // 当地时区
            return startTime.AddMilliseconds(timeStamp * 1000);
        }
    }
}
