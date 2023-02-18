using System.Threading.Tasks;
using SSCMS.Dto;

namespace SSCMS.Services
{
    public interface IVodManager
    {
        Task<VodSettings> GetVodSettingsAsync();

        Task<VodResult> UploadVodAsync(string filePath);
    }
}
