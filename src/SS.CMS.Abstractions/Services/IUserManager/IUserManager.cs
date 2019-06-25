using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IUserManager
    {
        Task<string> SignInAsync(UserInfo userInfo);

        Task SignOutAsync();

        string GetIpAddress();

        string GetUserName();

        int GetUserId();

        Task<UserInfo> GetUserAsync();
    }
}
