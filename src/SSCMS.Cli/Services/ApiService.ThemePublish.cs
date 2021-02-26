using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;

namespace SSCMS.Cli.Services
{
    public partial class ApiService
    {
        public (bool success, string failureMessage) ThemePublish(string zipPath)
        {
            var status = _configService.Status;
            if (status == null || string.IsNullOrEmpty(status.UserName) || string.IsNullOrEmpty(status.AccessToken))
            {
                return (false, "you have not logged in");
            }

            var url = CloudUtils.Api.GetCliUrl(RestUrlThemePublish);
            return RestUtils.Upload(url, zipPath, status.AccessToken);

            //var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlThemePublish)) { Timeout = -1 };
            //var request = new RestRequest(Method.POST);
            ////request.AddHeader("Content-Type", "multipart/form-data");
            //request.AddHeader("Authorization", $"Bearer {status.AccessToken}");
            //request.AddFile("file", zipPath);
            //var response = client.Execute(request);

            //return response.IsSuccessful ? (true, null) : (false, StringUtils.Trim(response.Content, '"'));
        }
    }
}
