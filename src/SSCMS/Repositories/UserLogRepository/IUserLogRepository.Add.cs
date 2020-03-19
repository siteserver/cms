using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IUserLogRepository
    {
        Task AddUserLoginLogAsync(int userId);

        Task AddUserLogAsync(int userId, string actionType, string summary);
    }
}