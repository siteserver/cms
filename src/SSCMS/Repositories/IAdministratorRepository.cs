using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IAdministratorRepository : IRepository
    {
        Task UpdateLastActivityDateAndCountOfFailedLoginAsync(Administrator administrator);

        Task UpdateLastActivityDateAsync(Administrator administrator);

        Task UpdateLastActivityDateAndCountOfLoginAsync(Administrator administrator);

        Task UpdateSiteIdsAsync(Administrator administrator, List<int> siteIds);

        Task<List<int>> UpdateSiteIdAsync(Administrator administrator, int siteId);

        Task LockAsync(IList<string> userNames);

        Task UnLockAsync(IList<string> userNames);

        Task<int> GetCountAsync(string creatorUserName, string role, int lastActivityDate, string keyword);

        Task<List<Administrator>> GetAdministratorsAsync(string creatorUserName, string role, string order,
            int lastActivityDate, string keyword, int offset, int limit);

        Task<bool> IsUserNameExistsAsync(string adminName);

        Task<bool> IsEmailExistsAsync(string email);

        Task<bool> IsMobileExistsAsync(string mobile);

        Task<List<string>> GetUserNamesAsync();

        Task<List<int>> GetUserIdsAsync();

        Task<(bool IsValid, string ErrorMessage)> InsertValidateAsync(string userName, string password, string email,
            string mobile);

        Task<(bool IsValid, string ErrorMessage)> InsertAsync(Administrator administrator, string password);

        Task<(bool IsValid, string ErrorMessage)> UpdateAsync(Administrator administrator);

        Task<(bool IsValid, string ErrorMessage)> ChangePasswordAsync(Administrator adminEntity, string password);

        Task<(Administrator administrator, string userName, string errorMessage)> ValidateAsync(string account, string password, bool isPasswordMd5);

        Task<(bool Success, string ErrorMessage)> ValidateLockAsync(Administrator administrator);

        Task<int> GetCountAsync();

        Task<List<Administrator>> GetAdministratorsAsync(int offset, int limit);

        Task<bool> IsExistsAsync(int id);

        Task<Administrator> DeleteAsync(int id);
    }
}
