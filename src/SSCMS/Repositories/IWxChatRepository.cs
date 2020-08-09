using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IWxChatRepository : IRepository
    {
        Task<bool> UserAdd(WxChat chat);

        Task ReplyAdd(WxChat chat);

        Task Star(int siteId, int chatId, bool star);

        Task DeleteAsync(int siteId, int chatId);

        Task DeleteAllAsync(int siteId, string openId);

        Task<List<WxChat>> GetChatsAsyncByOpenId(int siteId, string openId);

        Task<int> GetCountAsync(int siteId, bool start, string keyword);

        Task<List<WxChat>> GetChatsAsync(int siteId, bool start, string keyword, int page, int perPage);
    }
}
