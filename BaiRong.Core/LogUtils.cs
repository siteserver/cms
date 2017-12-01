using System;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core
{
    public class LogUtils
    {
        private static void AddErrorLog(ErrorLogInfo logInfo)
        {
            try
            {
                if (ConfigManager.SystemConfigInfo.IsTimeThreshold)
                {
                    BaiRongDataProvider.ErrorLogDao.Delete(ConfigManager.SystemConfigInfo.TimeThreshold);
                }

                BaiRongDataProvider.ErrorLogDao.Insert(logInfo);
            }
            catch
            {
                // ignored
            }
        }

        public static void AddSystemErrorLog(Exception ex, string summary = "")
        {
            AddErrorLog(new ErrorLogInfo(0, string.Empty, ex.Message, ex.StackTrace, summary, DateTime.Now));
        }

        public static void AddPluginErrorLog(string pluginId, Exception ex, string summary = "")
        {
            AddErrorLog(new ErrorLogInfo(0, pluginId, ex.Message, ex.StackTrace, summary, DateTime.Now));
        }

        public static void AddAdminLog(string userName, string action, string summary = "")
        {
            if (!ConfigManager.SystemConfigInfo.IsLogAdmin) return;

            try
            {
                if (ConfigManager.SystemConfigInfo.IsTimeThreshold)
                {
                    BaiRongDataProvider.LogDao.Delete(ConfigManager.SystemConfigInfo.TimeThreshold);
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

                BaiRongDataProvider.LogDao.Insert(logInfo);
            }
            catch (Exception ex)
            {
                AddSystemErrorLog(ex);
            }
        }

        public static void AddUserLoginLog(string userName)
        {
            AddUserLog(userName, EUserActionType.Login, string.Empty);
        }

        public static void AddUserLog(string userName, EUserActionType actionType, string summary)
        {
            AddUserLog(userName, EUserActionTypeUtils.GetText(actionType), summary);
        }

        public static void AddUserLog(string userName, string actionType, string summary)
        {
            if (!ConfigManager.SystemConfigInfo.IsLogUser) return;

            try
            {
                if (ConfigManager.SystemConfigInfo.IsTimeThreshold)
                {
                    BaiRongDataProvider.UserLogDao.Delete(ConfigManager.SystemConfigInfo.TimeThreshold);
                }

                if (!string.IsNullOrEmpty(summary))
                {
                    summary = StringUtils.MaxLengthText(summary, 250);
                }
                BaiRongDataProvider.UserLogDao.Insert(new UserLogInfo(0, userName, PageUtils.GetIpAddress(), DateTime.Now, actionType, summary));
            }
            catch (Exception ex)
            {
                AddSystemErrorLog(ex);
            }
        }
    }
}
