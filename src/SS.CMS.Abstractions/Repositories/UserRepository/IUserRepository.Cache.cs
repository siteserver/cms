using System.Threading.Tasks;

namespace SS.CMS.Abstractions
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

        string GetHomeUploadPath(params string[] paths);

        string GetUserUploadPath(int userId, string relatedPath);

        string GetUserUploadFileName(string filePath);

        string GetHomeUploadUrl(params string[] paths);

        string DefaultAvatarUrl { get; }

        string GetUserUploadUrl(int userId, string relatedUrl);

        string GetUserAvatarUrl(User user);

        Task<string> GetDisplayAsync(int userId);
    }
}