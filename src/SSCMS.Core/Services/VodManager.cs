using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Services;

namespace SSCMS.Core.Services
{
    public class VodManager : IVodManager
    {
        public Task<bool> IsEnabledAsync(int siteId)
        {
            return Task.FromResult(false);
        }

        public Task<VodPlay> UploadAsync(string filePath)
        {
            throw new System.NotImplementedException();
        }
    }
}
