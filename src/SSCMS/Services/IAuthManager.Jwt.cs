using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IAuthManager
    {
        string AuthenticateAdministrator(Administrator administrator, bool isPersistent);

        Task<string> RefreshAdministratorTokenAsync(string accessToken);

        string AuthenticateUser(User user, bool isPersistent);

        Task<string> RefreshUserTokenAsync(string accessToken);

        string AuthenticateApi(AccessToken accessToken, bool isPersistent);

        Task<string> RefreshApiTokenAsync(string accessToken);
    }
}
