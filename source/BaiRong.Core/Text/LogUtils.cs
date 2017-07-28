using System;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Text
{
    public class LogUtils
    {
        public static void AddErrorLog(ErrorLogInfo logInfo)
        {
            try
            {
                if (ConfigManager.SystemConfigInfo.IsTimeThreshold)
                {
                    BaiRongDataProvider.ErrorLogDao.Delete(ConfigManager.SystemConfigInfo.TimeThreshold, 0);
                }
                if (ConfigManager.SystemConfigInfo.IsCounterThreshold)
                {
                    BaiRongDataProvider.ErrorLogDao.Delete(0, ConfigManager.SystemConfigInfo.CounterThreshold * 10000);
                }

                BaiRongDataProvider.ErrorLogDao.Insert(logInfo);
            }
            catch
            {
                // ignored
            }
        }

        public static void AddErrorLog(Exception ex)
        {
            AddErrorLog(ex, string.Empty);
        }

        public static void AddErrorLog(Exception ex, string summary)
        {
            var logInfo = new ErrorLogInfo(0, DateTime.Now, ex.Message, ex.StackTrace, summary);
            AddErrorLog(logInfo);
        }

        public static void AddAdminLog(string userName, string action)
        {
            AddAdminLog(userName, action, string.Empty);
        }

        public static void AddAdminLog(string userName, string action, string summary)
        {
            if (!ConfigManager.SystemConfigInfo.IsLogAdmin) return;

            try
            {
                BaiRongDataProvider.LogDao.Delete(ConfigManager.SystemConfigInfo.IsTimeThreshold ? ConfigManager.SystemConfigInfo.TimeThreshold : 0, ConfigManager.SystemConfigInfo.IsCounterThreshold ? ConfigManager.SystemConfigInfo.CounterThreshold * 10000 : 0);

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
                AddErrorLog(ex);
            }
        }

        public static void AddUserLoginLog(string userName)
        {
            AddUserLog(userName, EUserActionType.Login, string.Empty);
        }

        public static void AddUserLog(string userName, EUserActionType actionType, string summary)
        {
            if (!ConfigManager.SystemConfigInfo.IsLogUser) return;

            try
            {
                BaiRongDataProvider.UserLogDao.Delete(ConfigManager.SystemConfigInfo.IsTimeThreshold ? ConfigManager.SystemConfigInfo.TimeThreshold : 0, ConfigManager.SystemConfigInfo.IsCounterThreshold ? ConfigManager.SystemConfigInfo.CounterThreshold * 10000 : 0);

                if (!string.IsNullOrEmpty(summary))
                {
                    summary = StringUtils.MaxLengthText(summary, 250);
                }
                BaiRongDataProvider.UserLogDao.Insert(new UserLogInfo(0, userName, PageUtils.GetIpAddress(), DateTime.Now, EUserActionTypeUtils.GetValue(actionType), summary));
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
            }
        }
    }
}
