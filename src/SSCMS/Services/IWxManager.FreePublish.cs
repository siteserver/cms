using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task<(bool success, string publishId, string errorMessage)> FreePublishSubmitAsync(string accessToken, string mediaId);
    }
}
