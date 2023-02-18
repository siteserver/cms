using System;
using System.Threading.Tasks;
using SSCMS.Dto;

namespace SSCMS.Services
{
    public interface ICensorManager
    {
        [Obsolete]
        Task<bool> IsTextEnabledAsync();

        [Obsolete]
        Task<bool> IsImageEnabledAsync();

        [Obsolete]
        Task<CensorResult> ScanText(string text);

        [Obsolete]
        Task<CensorResult> ScanImage(string imageUrl);

        Task<CensorSettings> GetCensorSettingsAsync();

        Task<CensorResult> CensorTextAsync(string text);

        Task<(bool success, string errorMessage)> AddCensorWhiteListAsync(string word);
    }
}
