using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Wx;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task AddUserTag(string accessTokenOrAppId, string tagName);

        Task UpdateUserTag(string accessTokenOrAppId, int tagId, string tagName);

        Task DeleteUserTag(string accessTokenOrAppId, int tagId);

        Task UpdateUserRemarkAsync(string accessTokenOrAppId, string openId, string remark);

        Task UserBatchTaggingAsync(string accessTokenOrAppId, int tagId, List<string> openIds);

        Task UserBatchUnTaggingAsync(string accessTokenOrAppId, int tagId, List<string> openIds);

        Task UserBatchBlackListAsync(string accessTokenOrAppId, List<string> openIds);

        Task UserBatchUnBlackListAsync(string accessTokenOrAppId, List<string> openIds);

        Task<List<WxUserTag>> GetUserTagsAsync(string accessTokenOrAppId);

        Task<List<string>> GetUserOpenIdsAsync(string accessTokenOrAppId, bool isBlock);

        Task<List<WxUser>> GetUsersAsync(string accessTokenOrAppId, List<string> openIds);

        Task<WxUser> GetUserAsync(string accessTokenOrAppId, string openId);
    }
}
