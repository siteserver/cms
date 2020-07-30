using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Wx;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task<List<WxUserTag>> GetUserTagsAsync(string token);

        Task<List<WxUser>> GetUsersAsync(string token);
    }
}
