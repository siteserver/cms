using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.Containers;

namespace SSCMS.Core.Services
{
    public partial class OpenManager
    {
        public async Task<(bool, string, string)> GetWxAccessTokenAsync(int siteId)
        {
            var success = false;
            var errorMessage = string.Empty;
            string token = null;

            var account = await _openAccountRepository.GetBySiteIdAsync(siteId);
            if (!account.WxConnected)
            {
                errorMessage = "微信公众号未连接，请设置后进入微信公众号基本配置中启用";
            }
            else
            {
                try
                {
                    token = AccessTokenContainer.TryGetAccessToken(account.WxAppId, account.WxAppSecret);
                    success = true;
                }
                catch (ErrorJsonResultException ex)
                {
                    errorMessage = $"API 调用发生错误：{ex.JsonResult.errmsg}";
                }
                catch (Exception ex)
                {
                    errorMessage = $"执行过程发生错误：{ex.Message}";
                }
            }

            return (success, token, errorMessage);
        }

        public async Task<IEnumerable<(int Id, string Name, int Count)>> GetUserTags(string token)
        {
            return (await UserTagApi.GetAsync(token)).tags.Select(tag =>
                (tag.id, tag.name, tag.count));
        }

        public async Task<List<UserInfoJson>> GetUsersAsync(string token)
        {
            var openIdResult = await UserApi.GetAsync(token, null);

            var userList = openIdResult.data.openid.Select(openId => new BatchGetUserInfoData
                {
                    openid = openId
                })
                .ToList();

            var userResult = await UserApi.BatchGetUserInfoAsync(token, userList);
            return userResult.user_info_list;
        }
    }
}
