using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IUserRepository
    {
        Task<User> GetByAccountAsync(string account);

        Task<User> GetByUserIdAsync(int userId);

        Task<User> GetByUserNameAsync(string userName);

        Task<User> GetByMobileAsync(string mobile);

        Task<User> GetByEmailAsync(string email);

        Task<bool> IsIpAddressCachedAsync(string ipAddress);

        Task CacheIpAddressAsync(string ipAddress);

        Task<string> GetDisplayAsync(int userId);
    }
}