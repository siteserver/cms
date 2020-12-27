using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Services
{
    public interface ICensorManager
    {
        Task<bool> IsTextEnabledAsync();

        Task<bool> IsImageEnabledAsync();

        Task<CensorResult> ScanText(string text);

        Task<CensorResult> ScanImage(string imageUrl);
    }
}
