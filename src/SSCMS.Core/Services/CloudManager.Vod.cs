using System.Threading.Tasks;
using SSCMS.Dto;

namespace SSCMS.Core.Services
{
    public partial class CloudManager
    {
        public Task<bool> IsVodAsync(int siteId)
        {
            return Task.FromResult(false);
        }

        public Task<VodPlay> UploadVodAsync(string filePath)
        {
            throw new System.NotImplementedException();
        }
    }
}
