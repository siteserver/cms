using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IUserRepository : IRepository
    {
        Task<(User user, string errorMessage)> InsertAsync(User user, string password, string ipAddress);

        Task<(bool success, string errorMessage)> UpdateAsync(User user);

        Task UpdateLastActivityDateAndCountOfLoginAsync(User user);

        Task UpdateLastActivityDateAsync(User user);

        Task<(bool success, string errorMessage)> ChangePasswordAsync(int userId, string password);

        Task<(bool success, string errorMessage)> IsPasswordCorrectAsync(string password);

        Task CheckAsync(IList<int> userIds);

        Task LockAsync(IList<int> userIds);

        Task UnLockAsync(IList<int> userIds);

        Task<bool> IsUserNameExistsAsync(string userName);

        Task<bool> IsEmailExistsAsync(string email);

        Task<bool> IsMobileExistsAsync(string mobile);

        Task<List<int>> GetUserIdsAsync(bool isChecked);

        bool CheckPassword(string password, bool isPasswordMd5, string dbPassword, PasswordFormat passwordFormat,
            string passwordSalt);

        Task<(User user, string userName, string errorMessage)> ValidateAsync(string account, string password,
            bool isPasswordMd5);

        Task<(bool success, string errorMessage)> ValidateStateAsync(User user);

        Dictionary<DateTime, int> GetTrackingDictionary(DateTime dateFrom, DateTime dateTo, string xType);

        Task<int> GetCountAsync();

        Task<int> GetCountAsync(bool? state, int groupId, int dayOfLastActivity, string keyword);

        Task<List<User>> GetUsersAsync(bool? state, int groupId, int dayOfLastActivity, string keyword, string order,
            int offset, int limit);

        Task<bool> IsExistsAsync(int id);

        Task<User> DeleteAsync(int userId);
    }
}
