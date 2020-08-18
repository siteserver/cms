using System.Threading.Tasks;
using RestSharp;
using SSCMS.Cli.Models;
using SSCMS.Core.Plugins;
using SSCMS.Utils;

namespace SSCMS.Cli.Services
{
    public partial class ApiService
    {
        public async Task<(bool success, string failureMessage)> LoginAsync(string account, string password)
        {
            var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlLogin)) { Timeout = -1 };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", TranslateUtils.JsonSerialize(new LoginRequest
            {
                Account = account,
                Password = AuthUtils.Md5ByString(password),
                IsPersistent = true
            }), ParameterType.RequestBody);
            var response = client.Execute<LoginResult>(request);
            if (!response.IsSuccessful)
            {
                return (false, "your account or password was incorrect");
            }

            var loginResult = response.Data;

            var status = new ConfigStatus
            {
                UserName = loginResult.UserName,
                AccessToken = loginResult.AccessToken
            };

            await _configService.SaveStatusAsync(status);

            return (true, null);
        }
    }
}
