using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Wx;

namespace SSCMS.Core.Services
{
    public partial class WxManager
    {
        public async Task AddUserTag(string accessTokenOrAppId, string tagName)
        {
            await UserTagApi.CreateAsync(accessTokenOrAppId, tagName);
        }

        public async Task UpdateUserTag(string accessTokenOrAppId, int tagId, string tagName)
        {
            await UserTagApi.UpdateAsync(accessTokenOrAppId, tagId, tagName);
        }

        public async Task DeleteUserTag(string accessTokenOrAppId, int tagId)
        {
            await UserTagApi.DeleteAsync(accessTokenOrAppId, tagId);
        }

        public async Task UpdateUserRemarkAsync(string accessTokenOrAppId, string openId, string remark)
        {
            await UserApi.UpdateRemarkAsync(accessTokenOrAppId, openId, remark);
        }

        public async Task UserBatchTaggingAsync(string accessTokenOrAppId, int tagId, List<string> openIds)
        {
            await UserTagApi.BatchTaggingAsync(accessTokenOrAppId, tagId, openIds);
        }

        public async Task UserBatchUnTaggingAsync(string accessTokenOrAppId, int tagId, List<string> openIds)
        {
            await UserTagApi.BatchUntaggingAsync(accessTokenOrAppId, tagId, openIds);
        }

        public async Task UserBatchBlackListAsync(string accessTokenOrAppId, List<string> openIds)
        {
            await UserApi.BatchBlackListAsync(accessTokenOrAppId, openIds);
        }

        public async Task UserBatchUnBlackListAsync(string accessTokenOrAppId, List<string> openIds)
        {
            await UserApi.BatchUnBlackListAsync(accessTokenOrAppId, openIds);
        }

        public async Task<List<WxUserTag>> GetUserTagsAsync(string accessTokenOrAppId)
        {
            return (await UserTagApi.GetAsync(accessTokenOrAppId)).tags.Select(tag =>
                new WxUserTag
                {
                    Id = tag.id,
                    Name = tag.name,
                    Count = tag.count
                }).ToList();
        }

        public async Task<List<string>> GetUserOpenIdsAsync(string accessTokenOrAppId, bool isBlock)
        {
            var openIds = new List<string>();
            string nextOpenId = null;
            while (true)
            {
                nextOpenId = await LoadUserOpenIdsAsync(accessTokenOrAppId, openIds, nextOpenId, isBlock);
                if (string.IsNullOrEmpty(nextOpenId)) break;
            }

            return openIds;
        }

        public async Task<List<WxUser>> GetUsersAsync(string accessTokenOrAppId, List<string> openIds)
        {
            var users = new List<WxUser>();
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
                var userResult = await UserApi.BatchGetUserInfoAsync(accessTokenOrAppId, userList);
                users.AddRange(userResult.user_info_list.Select(GetWxUser));
            }

            users = users.OrderByDescending(x => x.SubscribeTime).ToList();

            return users;
        }

        public async Task<WxUser> GetUserAsync(string accessTokenOrAppId, string openId)
        {
            if (string.IsNullOrEmpty(openId)) return null;

            var userResult = await UserApi.InfoAsync(accessTokenOrAppId, openId);

            return GetWxUser(userResult);
        }

        private async Task<string> LoadUserOpenIdsAsync(string accessTokenOrAppId, List<string> openIds, string nextOpenId, bool isBlock)
        {
            var openIdResult = isBlock
                ? await UserApi.GetBlackListAsync(accessTokenOrAppId, nextOpenId)
                : await UserApi.GetAsync(accessTokenOrAppId, nextOpenId);

            if (openIdResult.data != null)
            {
                openIds.AddRange(openIdResult.data.openid);
            }

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
