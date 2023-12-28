using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task<string> ReplyTextAsync(WxAccount account, string fromUserName, string toUserName, string message, string timestamp, string nonce);
    }
}
