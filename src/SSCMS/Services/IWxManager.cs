using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        string GetNonceStr();

        string GetTimestamp();

        Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(int siteId);

        Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(string mpAppId, string mpAppSecret);

        Task<(bool success, string ticket, string errorMessage)> GetJsApiTicketAsync(string mpAppId, string mpAppSecret);

        string GetJsApiSignature(string ticket, string nonceStr, string timestamp, string url);
    }
}
