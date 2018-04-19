using System;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public static class LogUtils
    {
        private static int AddErrorLog(ErrorLogInfo logInfo)
        {
            try
            {
                if (!ConfigManager.SystemConfigInfo.IsLogError) return 0;

                if (ConfigManager.SystemConfigInfo.IsTimeThreshold)
                {
                    DataProvider.ErrorLogDao.Delete(ConfigManager.SystemConfigInfo.TimeThreshold);
                }

                return DataProvider.ErrorLogDao.Insert(logInfo);
            }
            catch
            {
                // ignored
            }

            return 0;
        }

        public static void AddErrorLogAndRedirect(Exception ex, string summary = "")
        {
            if (ex.HResult == -2147467259) // 文件名不存在
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
            else
            {
                var logId = AddErrorLog(ex, summary);
                if (logId > 0)
                {
                    PageUtils.RedirectToErrorPage(logId);
                }
                else
                {
                    PageUtils.RedirectToErrorPage(ex.Message);
                }
            }
        }

        public static int AddErrorLog(Exception ex, string summary = "")
        {
            return AddErrorLog(new ErrorLogInfo(0, string.Empty, ex.Message, ex.StackTrace, summary, DateTime.Now));
        }

        public static int AddErrorLog(string pluginId, Exception ex, string summary = "")
        {
            return AddErrorLog(new ErrorLogInfo(0, pluginId, ex.Message, ex.StackTrace, summary, DateTime.Now));
        }

        public static void AddAdminLog(string userName, string action, string summary = "")
        {
            if (!ConfigManager.SystemConfigInfo.IsLogAdmin) return;

            try
            {
                if (ConfigManager.SystemConfigInfo.IsTimeThreshold)
                {
                    DataProvider.LogDao.Delete(ConfigManager.SystemConfigInfo.TimeThreshold);
                }

                if (!string.IsNullOrEmpty(action))
                {
                    action = StringUtils.MaxLengthText(action, 250);
                }
                if (!string.IsNullOrEmpty(summary))
                {
                    summary = StringUtils.MaxLengthText(summary, 250);
                }
                var logInfo = new LogInfo(0, userName, PageUtils.GetIpAddress(), DateTime.Now, action, summary);

                DataProvider.LogDao.Insert(logInfo);
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
            }
        }

        public static void AddUserLoginLog(string userName)
        {
            AddUserLog(userName, "用户登录", string.Empty);
        }

        public static void AddUserLog(string userName, string actionType, string summary)
        {
            if (!ConfigManager.SystemConfigInfo.IsLogUser) return;

            try
            {
                if (ConfigManager.SystemConfigInfo.IsTimeThreshold)
                {
                    DataProvider.UserLogDao.Delete(ConfigManager.SystemConfigInfo.TimeThreshold);
                }

                if (!string.IsNullOrEmpty(summary))
                {
                    summary = StringUtils.MaxLengthText(summary, 250);
                }
                DataProvider.UserLogDao.Insert(new UserLogInfo(0, userName, PageUtils.GetIpAddress(), DateTime.Now, actionType, summary));
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
            }
        }
    }
}
