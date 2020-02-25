using System.Threading.Tasks;


namespace SS.CMS.Abstractions
{
    public partial interface IUserManager : IService
    {
        string GetToken(User userInfo);

        string GetIpAddress();

        string GetUserName();

        int GetUserId();

        Task<User> GetUserAsync();
    }
}
