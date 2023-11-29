using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(int siteId);

        Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(string mpAppId, string mpAppSecret);
    }
}
