using System.Threading.Tasks;

namespace SiteServer.Abstractions
{
    public partial interface IAdministratorRepository
    {
        Task<Administrator> GetByAccountAsync(string account);

        Task<Administrator> GetByUserIdAsync(int userId);

        Task<Administrator> GetByUserNameAsync(string userName);

        Task<Administrator> GetByMobileAsync(string mobile);

        Task<Administrator> GetByEmailAsync(string email);

        string GetUserUploadPath(int userId, string relatedPath);

        string GetUserUploadFileName(string filePath);

        string GetUserUploadUrl(int userId, string relatedUrl);

        Task<string> GetDisplayAsync(int userId);
    }
}
