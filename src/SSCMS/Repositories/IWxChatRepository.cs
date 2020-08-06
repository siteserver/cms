using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IWxChatRepository : IRepository
    {
        Task<int> InsertAsync(WxChat chat);

        Task DeleteAsync(int siteId, int chatId);

        Task DeleteAllAsync(int siteId, string openId);

        Task<List<WxChat>> GetChatsAsync(int siteId, string openId);
    }
}
