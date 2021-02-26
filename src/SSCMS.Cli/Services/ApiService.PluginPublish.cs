using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;

namespace SSCMS.Cli.Services
{
    public partial class ApiService
    {
        public (bool success, string failureMessage) PluginPublish(string publisher, string zipPath)
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

            var url = CloudUtils.Api.GetCliUrl(RestUrlPluginPublish);
            return RestUtils.Upload(url, zipPath, status.AccessToken);
        }
    }
}
