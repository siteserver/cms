using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Core.Services
{
    public class CensorManager : ICensorManager
    {
        public Task<bool> IsTextEnabledAsync()
        {
            return Task.FromResult(false);
        }

        public Task<bool> IsImageEnabledAsync()
        {
            return Task.FromResult(false);
        }

        public Task<CensorResult> ScanText(string text)
        {
            throw new System.NotImplementedException();
        }

        public Task<CensorResult> ScanImage(string imagePath)
        {
            throw new System.NotImplementedException();
        }
    }
}
