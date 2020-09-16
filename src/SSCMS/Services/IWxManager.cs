using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task<(bool, string, string)> GetAccessTokenAsync(string mpAppId, string mpAppSecret);

        Task<(bool, string, string)> GetAccessTokenAsync(int siteId);
    }
}
