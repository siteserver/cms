using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IAdministratorRepository
    {
        Task<Administrator> GetByAccountAsync(string account);

        Task<Administrator> GetByUserIdAsync(int userId);

        Task<Administrator> GetByUserNameAsync(string userName);

        Task<Administrator> GetByMobileAsync(string mobile);

        Task<Administrator> GetByEmailAsync(string email);

        string GetUserUploadFileName(string filePath);

        Task<string> GetDisplayAsync(int userId);
    }
}
