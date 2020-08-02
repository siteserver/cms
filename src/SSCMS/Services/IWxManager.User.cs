using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Wx;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task<List<WxUserTag>> GetUserTagsAsync(string token);

        Task<List<string>> GetUserOpenIdsAsync(string token);

        Task<List<WxUser>> GetUsersAsync(string token, List<string> openIds);

        Task<WxUser> GetUserAsync(string token, string openId);
    }
}
