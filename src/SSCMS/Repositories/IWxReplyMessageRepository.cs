using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Repositories
{
    public interface IWxReplyMessageRepository : IRepository
    {
        Task<int> InsertAsync(WxReplyMessage message);

        Task UpdateAsync(WxReplyMessage message);

        Task DeleteAllAsync(int siteId, int ruleId);

        Task<List<WxReplyMessage>> GetMessagesAsync(int siteId, int ruleId);

        Task<WxReplyMessage> GetMessageAsync(int siteId, int messageId);
    }
}
