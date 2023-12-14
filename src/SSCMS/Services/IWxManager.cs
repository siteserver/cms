using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        string GetNonceStr();

        string GetTimestamp();

        Task<(bool success, string ticket, string errorMessage)> GetJsApiTicketAsync(string mpAppId, string mpAppSecret);

        string GetJsApiSignature(string ticket, string nonceStr, string timestamp, string url);

        Task<bool> IsEnabledAsync(Site site);
    }
}
