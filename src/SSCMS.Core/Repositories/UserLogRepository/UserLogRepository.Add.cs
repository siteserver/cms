using System;
using System.Threading.Tasks;
using SSCMS;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories.UserLogRepository
{
    public partial class UserLogRepository
    {
        public async Task AddUserLoginLogAsync(int userId)
        {
            await AddUserLogAsync(userId, "用户登录", string.Empty);
        }

        public async Task AddUserLogAsync(int userId, string actionType, string summary)
        {
            var config = await _configRepository.GetAsync();
            if (!config.IsLogUser) return;

            try
            {
                await DeleteIfThresholdAsync();

                if (!string.IsNullOrEmpty(summary))
                {
                    summary = StringUtils.MaxLengthText(summary, 250);
                }

                var userLogInfo = new UserLog
                {
                    Id = 0,
                    UserId = userId,
                    IpAddress = string.Empty,
                    Action = actionType,
                    Summary = summary
                };

                await InsertAsync(userLogInfo);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(ex);
            }
        }
    }
}
