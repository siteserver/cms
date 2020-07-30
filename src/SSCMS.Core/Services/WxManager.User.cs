using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.User;
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

        public async Task<List<WxUser>> GetUsersAsync(string token)
        {
            var users = new List<WxUser>();
            string nextOpenId = null;
            while (true)
            {
                nextOpenId = await LoadUsersAsync(users, token, nextOpenId);
                if (string.IsNullOrEmpty(nextOpenId)) break;
            }

            return users;
        }

        private async Task<string> LoadUsersAsync(List<WxUser> users, string token, string nextOpenId)
        {
            var openIdResult = await UserApi.GetAsync(token, nextOpenId);
            var userList = openIdResult.data.openid.Select(openId => new BatchGetUserInfoData
                {
                    openid = openId
                })
                .ToList();

            var userResult = await UserApi.BatchGetUserInfoAsync(token, userList);
            users.AddRange(userResult.user_info_list.Select(GetOpenUser));

            return users.Count != openIdResult.total ? openIdResult.next_openid : null;
        }

        private WxUser GetOpenUser(UserInfoJson json)
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
                SubscribeTime = json.subscribe_time,
                UnionId = json.unionid,
                Remark = json.remark,
                GroupId = json.groupid,
                TagIdList = json.tagid_list,
                SubscribeScene = json.subscribe_scene,
                QrScene = json.qr_scene,
                QrSceneStr = json.qr_scene_str
            };
        }
    }
}
