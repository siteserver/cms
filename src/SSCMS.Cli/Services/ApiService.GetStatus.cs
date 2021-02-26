using SSCMS.Cli.Models;
using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;

namespace SSCMS.Cli.Services
{
    public partial class ApiService
    {
        public (ConfigStatus status, string failureMessage) GetStatus()
        {
            var status = _configService.Status;
            if (status == null || string.IsNullOrEmpty(status.UserName) || string.IsNullOrEmpty(status.AccessToken))
            {
                return (null, "you have not logged in");
            }

            //var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlStatus)) { Timeout = -1 };
            //var request = new RestRequest(Method.GET);
            //request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("Authorization", $"Bearer {status.AccessToken}");
            //var response = client.Execute<StatusResult>(request);

            var url = CloudUtils.Api.GetCliUrl(RestUrlStatus);
            var (isSuccess, result, _) = RestUtils.Get<StatusResult>(url, status.AccessToken);

            if (!isSuccess || result.UserName != status.UserName)
            {
                return (null, "you have not logged in");
            }

            return (status, null);
        }
    }
}
