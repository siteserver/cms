using System;
using System.Threading.Tasks;
using SSCMS.Models;

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

        Task<bool> IsCensorTextAsync();

        Task<bool> IsCensorImageAsync();

        Task<CensorResult> CensorTextAsync(string text);

        Task<CensorResult> CensorImageAsync(string imageUrl);
    }
}
