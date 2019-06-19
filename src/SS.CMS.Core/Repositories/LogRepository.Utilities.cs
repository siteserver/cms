using System;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class LogRepository
    {
        public void AddAdminLog(string ipAddress, string adminName, string action, string summary = "")
        {
            // if (!ConfigManager.Instance.IsLogAdmin) return;

            try
            {
                DeleteIfThreshold();

                if (!string.IsNullOrEmpty(action))
                {
                    action = StringUtils.MaxLengthText(action, 250);
                }
                if (!string.IsNullOrEmpty(summary))
                {
                    summary = StringUtils.MaxLengthText(summary, 250);
                }

                var logInfo = new LogInfo
                {
                    UserName = adminName,
                    IpAddress = ipAddress,
                    Action = action,
                    Summary = summary
                };

                Insert(logInfo);
            }
            catch (Exception ex)
            {
                _errorLogRepository.AddErrorLog(ex);
            }
        }
    }
}
