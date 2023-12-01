using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        string GetErrorUnAuthenticated(WxAccount account);
        
        Task<WxAccount> GetAccountAsync(int siteId);

        Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(WxAccount account);
        
        Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(int siteId);

        Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(string mpAppId, string mpAppSecret);
    }
}
