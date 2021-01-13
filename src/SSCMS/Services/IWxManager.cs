using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        string GetNonceStr();

        string GetTimestamp();

        Task<(bool Success, string AccessToken, string ErrorMessage)> GetAccessTokenAsync(int siteId);

        Task<(bool Success, string AccessToken, string ErrorMessage)> GetAccessTokenAsync(string mpAppId, string mpAppSecret);

        Task<(bool Success, string Ticket, string ErrorMessage)> GetJsApiTicketAsync(string mpAppId, string mpAppSecret);

        string GetJsApiSignature(string ticket, string nonceStr, string timestamp, string url);
    }
}
