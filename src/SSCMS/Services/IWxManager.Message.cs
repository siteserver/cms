using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task<string> ReplyAsync(WxAccount account, string fromUserName, string toUserName, WxReplyMessage replyMessage, string timestamp, string nonce);
    }
}
