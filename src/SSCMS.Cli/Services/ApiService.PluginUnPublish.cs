using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;

namespace SSCMS.Cli.Services
{
    public partial class ApiService
    {
        public (bool success, string failureMessage) PluginUnPublish(string pluginId)
        {
            var status = _configService.Status;
            if (status == null || string.IsNullOrEmpty(status.UserName) || string.IsNullOrEmpty(status.AccessToken))
            {
                return (false, "you have not logged in");
            }

            var url = CloudUtils.Api.GetCliUrl(RestUrlPluginUnPublish);
            return RestUtils.Post(url, new PluginUnPublishRequest
            {
                PluginId = pluginId
            }, status.AccessToken);

            //var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlPluginUnPublish)) { Timeout = -1 };
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("Authorization", $"Bearer {status.AccessToken}");
            //request.AddParameter("application/json", TranslateUtils.JsonSerialize(new PluginUnPublishRequest
            //{
            //    PluginId = pluginId
            //}), ParameterType.RequestBody);
            //var response = client.Execute(request);
            //if (!response.IsSuccessful)
            //{
            //    return (false, StringUtils.Trim(response.Content, '"'));
            //}

            //return (true, null);
        }
    }
}
