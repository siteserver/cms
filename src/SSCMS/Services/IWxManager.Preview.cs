using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task<(bool success, string errorMessage)> PreviewAsync(string accessToken, string mediaId, string wxName);
    }
}
