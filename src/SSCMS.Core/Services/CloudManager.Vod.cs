using System;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class CloudManager
    {
        public async Task<bool> IsVodAsync()
        {
            var config = await _configRepository.GetAsync();
            return config.IsCloudVod;
        }

        public async Task<VodPlay> UploadVodAsync(string filePath)
        {
            var config = await _configRepository.GetAsync();
            if (string.IsNullOrEmpty(config.CloudUserName) || string.IsNullOrEmpty(config.CloudToken))
            {
                throw new Exception("云助手未登录");
            }

            var url = GetCloudUrl(RouteVod);
            var (success, result, errorMessage) = await RestUtils.UploadAsync<VodPlay>(url, filePath, config.CloudToken);

            if (!success && result == null)
            {
                result = new VodPlay
                {
                    Success = false,
                    ErrorMessage = errorMessage
                };
            }

            return result;
        }
    }
}
