using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Wx;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task<List<WxUserTag>> GetUserTagsAsync(string accessTokenOrAppId);

        Task<List<string>> GetUserOpenIdsAsync(string accessTokenOrAppId);

        Task<List<WxUser>> GetUsersAsync(string accessTokenOrAppId, List<string> openIds);

        Task<WxUser> GetUserAsync(string accessTokenOrAppId, string openId);
    }
}
