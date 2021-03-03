using System.Threading.Tasks;
using SSCMS.Core.Utils;

namespace SSCMS.Cli.Services
{
    public partial class ApiService
    {
        public async Task<(bool success, string failureMessage)> PluginPublishAsync(string publisher, string zipPath)
        {
            var status = _configService.Status;
            if (status == null || string.IsNullOrEmpty(status.UserName) || string.IsNullOrEmpty(status.AccessToken))
            {
                return (false, "you have not logged in");
            }

            if (status.UserName != publisher)
            {
                return (false, $"the publisher in package.json should be '{status.UserName}'");
            }

            var url = GetCliUrl(RestUrlPluginPublish);
            return await RestUtils.UploadAsync(url, zipPath, status.AccessToken);
        }
    }
}
