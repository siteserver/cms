using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IUserLogRepository
    {
        Task AddUserLoginLogAsync(int userId);

        Task AddUserLogAsync(int userId, string actionType, string summary);
    }
}