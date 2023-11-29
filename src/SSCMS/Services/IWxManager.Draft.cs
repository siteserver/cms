using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task<(bool success, string mediaId, string errorMessage)> DraftAddAsync(string accessToken, int messageId);
    }
}
