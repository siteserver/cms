using System.Threading.Tasks;
using SSCMS.Cli.Models;
using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Cli.Services
{
    public partial class ApiService
    {
        public async Task<(bool success, string failureMessage)> LoginAsync(string account, string password)
        {
            var url = CloudUtils.Api.GetCliUrl(RestUrlLogin);
            var (success, result, _) = RestUtils.Post<LoginRequest, LoginResult>(url,
                new LoginRequest
                {
                    Account = account,
                    Password = AuthUtils.Md5ByString(password),
                    IsPersistent = true
                });

            if (!success)
            {
                return (false, "your account or password was incorrect");
            }

            var status = new ConfigStatus
            {
                UserName = result.UserName,
                AccessToken = result.AccessToken
            };

            await _configService.SaveStatusAsync(status);

            return (true, null);
        }
    }
}
