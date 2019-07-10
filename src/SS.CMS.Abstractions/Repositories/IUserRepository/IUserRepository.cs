using System.Collections.Generic;
using System.Threading.Tasks;
using SqlKata;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IUserRepository : IRepository
    {
        Task<IEnumerable<User>> GetAllAsync(Query query);

        Task<int> GetCountAsync(Query query);

        Task<int> GetCountAsync();

        Task<(bool IsSuccess, int UserId, string ErrorMessage)> InsertAsync(User userInfo);

        Task<(bool IsSuccess, string ErrorMessage)> UpdateAsync(User userInfo);

        Task<bool> UpdateLastActivityDateAndCountOfFailedLoginAsync(User userInfo);

        Task<bool> UpdateLastActivityDateAndCountOfLoginAsync(User userInfo);

        Task UpdateSiteIdCollectionAsync(User userInfo, string siteIdCollection);

        Task<List<int>> UpdateSiteIdAsync(User userInfo, int siteId);

        Task<bool> DeleteAsync(User userInfo);

        Task LockAsync(List<int> userIdList);

        Task UnLockAsync(List<int> userIdList);

        Task<User> GetByAccountAsync(string account);

        Task<User> GetByUserIdAsync(int userId);

        Task<User> GetByUserNameAsync(string userName);

        Task<User> GetByMobileAsync(string mobile);

        Task<User> GetByEmailAsync(string email);

        Task<bool> IsUserNameExistsAsync(string userName);

        Task<bool> IsEmailExistsAsync(string email);

        Task<bool> IsMobileExistsAsync(string mobile);

        Task<int> GetCountByAreaIdAsync(int areaId);

        Task<int> GetCountByDepartmentIdAsync(int departmentId);

        /// <summary>
        /// 获取管理员用户名列表。
        /// </summary>
        /// <returns>
        /// 管理员用户名列表。
        /// </returns>
        Task<IEnumerable<string>> GetUserNameListAsync();

        Task<IEnumerable<string>> GetUserNameListAsync(int departmentId);

        Task<(bool IsSuccess, string ErrorMessage)> ChangePasswordAsync(User userInfo, string password);

        Task<(bool IsSuccess, string UserName, string ErrorMessage)> ValidateAsync(string account, string password, bool isPasswordMd5);

        bool CheckPassword(string password, bool isPasswordMd5, string dbPassword, PasswordFormat passwordFormat, string passwordSalt);
    }
}
