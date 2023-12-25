using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Utils;

// https://developers.weixin.qq.com/doc/offiaccount/User_Management/Getting_a_User_List.html

namespace SSCMS.Core.Services
{
    public partial class WxManager
    {
        public class UserGetResult
        {
            public int total { get; set; }
            public int count { get; set; }
            public UserGetResultData data { get; set; }
            public string next_openid { get; set; }
        }

        public class UserGetResultData
        {
            public List<string> openid { get; set; }
        }

        public async Task<(bool success, string errorMessage)> UserGetAsync(string accessToken, List<string> openIds, string nextOpenId = null)
        {
            var openId = string.IsNullOrEmpty(nextOpenId) ? string.Empty : nextOpenId;
            var url = $"https://api.weixin.qq.com/cgi-bin/user/get?access_token={accessToken}&next_openid={openId}";
            var (success, result, errorMessage) = await RestUtils.GetStringAsync(url);
            if (success)
            {
                (success, errorMessage) = await GetErrorMessageAsync(result);
                if (success)
                {
                    var theResult = TranslateUtils.JsonDeserialize<UserGetResult>(result);
                    if (string.IsNullOrEmpty(theResult.next_openid))
                    {
                        return (success, errorMessage);
                    }
                    openIds.AddRange(theResult.data.openid);
                    return await UserGetAsync(accessToken, openIds, theResult.next_openid);
                }
            }

            return (success, errorMessage);
        }
    }
}
