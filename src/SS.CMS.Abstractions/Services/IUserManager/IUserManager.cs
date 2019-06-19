using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services.IUserManager
{
    public partial interface IUserManager
    {
        Task SignInAsync(UserInfo userInfo, bool isPersistent = false);

        Task SignOutAsync();

        string GetIpAddress();

        string GetUserName();

        int GetUserId();

        Task<UserInfo> GetUserAsync();
    }
}
