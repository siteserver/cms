using System;
using System.Threading.Tasks;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class CloudManager
    {
        private class BackupSubmitRequest
        {
            public long Size { get; set; }
        }

        public async Task BackupAsync(long size)
        {
            var config = await _configRepository.GetAsync();
            if (string.IsNullOrEmpty(config.CloudUserName) || string.IsNullOrEmpty(config.CloudToken))
            {
                throw new Exception("云助手未登录");
            }

            var url = GetCloudUrl(RouteBackup);
            var request = new BackupSubmitRequest
            {
                Size = size,
            };
            var (success, errorMessage) = await RestUtils.PostAsync(url, request, config.CloudToken);

            if (!success)
            {
                throw new Exception(errorMessage);
            }
        }
    }
}
