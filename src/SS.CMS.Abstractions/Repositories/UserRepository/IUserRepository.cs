using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SS.CMS.Abstractions
{
    public partial interface IUserRepository : IRepository
    {
        Task<(int UserId, string ErrorMessage)> InsertAsync(User user, string password, string ipAddress);

        Task<(User User, string ErrorMessage)> UpdateAsync(User user, IDictionary<string, object> body);

        Task UpdateAsync(User user);

        Task UpdateLastActivityDateAndCountOfLoginAsync(User user);

        Task<(bool IsValid, string ErrorMessage)> ChangePasswordAsync(int userId, string password);

        Task<(bool Valid, string ErrorMessage)> IsPasswordCorrectAsync(string password);

        Task CheckAsync(IList<int> idList);

        Task LockAsync(IList<int> idList);

        Task UnLockAsync(IList<int> idList);

        Task<bool> IsUserNameExistsAsync(string userName);

        Task<bool> IsEmailExistsAsync(string email);

        Task<bool> IsMobileExistsAsync(string mobile);

        Task<List<int>> GetIdListAsync(bool isChecked);

        bool CheckPassword(string password, bool isPasswordMd5, string dbPassword, PasswordFormat passwordFormat,
            string passwordSalt);

        Task<(User User, string UserName, string ErrorMessage)> ValidateAsync(string account, string password,
            bool isPasswordMd5);

        Dictionary<DateTime, int> GetTrackingDictionary(DateTime dateFrom, DateTime dateTo, string xType);

        Task<int> GetCountAsync();

        Task<int> GetCountAsync(bool? state, int groupId, int dayOfLastActivity, string keyword);

        Task<List<User>> GetUsersAsync(bool? state, int groupId, int dayOfLastActivity, string keyword, string order,
            int offset, int limit);

        Task<bool> IsExistsAsync(int id);

        Task<User> DeleteAsync(int userId);
    }
}
