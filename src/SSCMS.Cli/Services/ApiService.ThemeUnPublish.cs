using System.Threading.Tasks;
using SSCMS.Core.Utils;

namespace SSCMS.Cli.Services
{
  public partial class ApiService
    {
        public async Task<(bool success, string failureMessage)> ThemeUnPublishAsync(string name)
        {
            var status = _configService.Status;
            if (status == null || string.IsNullOrEmpty(status.UserName) || string.IsNullOrEmpty(status.AccessToken))
            {
                return (false, "you have not logged in");
            }

            var url = GetCliUrl(RestUrlThemeUnPublish);
            return await RestUtils.PostAsync(url, new ThemeUnPublishRequest
            {
                Name = name
            }, status.AccessToken);

            //var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlThemeUnPublish)) { Timeout = -1 };
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("Authorization", $"Bearer {status.AccessToken}");
            //request.AddParameter("application/json", TranslateUtils.JsonSerialize(new ThemeUnPublishRequest
            //{
            //    Name = name
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
