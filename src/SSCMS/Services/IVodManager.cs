using System.Threading.Tasks;
using SSCMS.Dto;

namespace SSCMS.Services
{
    public interface IVodManager
    {
        Task<bool> IsEnabledAsync(int siteId);

        Task<(string coverUrl, string playUrl)> UploadAsync(string filePath);
    }
}
