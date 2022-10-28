using System;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Core.Services
{
    public class CensorManager : ICensorManager
    {
        [Obsolete]
        public Task<bool> IsTextEnabledAsync()
        {
            return Task.FromResult(false);
        }

        [Obsolete]
        public Task<bool> IsImageEnabledAsync()
        {
            return Task.FromResult(false);
        }

        [Obsolete]
        public Task<CensorResult> ScanText(string text)
        {
            throw new System.NotImplementedException();
        }

        [Obsolete]
        public Task<CensorResult> ScanImage(string imagePath)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsCensorTextAsync()
        {
            return Task.FromResult(false);
        }

        public Task<bool> IsCensorImageAsync()
        {
            return Task.FromResult(false);
        }

        public Task<CensorResult> CensorTextAsync(string text)
        {
            throw new System.NotImplementedException();
        }

        public Task<CensorResult> CensorImageAsync(string imagePath)
        {
            throw new System.NotImplementedException();
        }
    }
}
