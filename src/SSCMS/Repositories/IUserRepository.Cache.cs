using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IUserRepository
    {
        Task<User> GetByAccountAsync(string account);

        Task<User> GetByUserIdAsync(int userId);

        Task<User> GetByUserNameAsync(string userName);

        Task<User> GetByMobileAsync(string mobile);

        Task<User> GetByEmailAsync(string email);

        Task<string> GetDisplayAsync(int userId);
    }
}