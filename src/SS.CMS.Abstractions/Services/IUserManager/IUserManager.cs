using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IUserManager
    {
        string GetToken(UserInfo userInfo);

        string GetIpAddress();

        string GetUserName();

        int GetUserId();

        Task<UserInfo> GetUserAsync();
    }
}
