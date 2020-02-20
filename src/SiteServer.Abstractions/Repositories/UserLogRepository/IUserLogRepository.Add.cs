using System.Threading.Tasks;

namespace SiteServer.Abstractions
{
    public partial interface IUserLogRepository
    {
        Task AddUserLoginLogAsync(int userId);

        Task AddUserLogAsync(int userId, string actionType, string summary);
    }
}