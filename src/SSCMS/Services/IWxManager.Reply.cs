using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task<List<WxReplyMessage>> GetMessagesAsync(int siteId, int ruleId);

        Task<WxReplyMessage> GetMessageAsync(int siteId, int messageId);
    }
}
