using System.Threading.Tasks;

namespace SSCMS.Core.Services
{
    public partial class CloudManager
    {
        public async Task<bool> IsImagesAsync()
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            var isFree = IsFree(config);
            return isAuthentication && !isFree && config.IsCloudImages;
        }
    }
}
