using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IUserManager
    {
        string GetToken(User userInfo);

        string GetIpAddress();

        string GetUserName();

        int GetUserId();

        Task<User> GetUserAsync();
    }
}
