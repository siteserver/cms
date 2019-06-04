using System;
using System.Collections.Generic;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Models;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Common
{
    public static class LogUtils
    {
        private const string CategoryStl = "stl";
        private const string CategoryAdmin = "admin";
        private const string CategoryHome = "home";
        private const string CategoryApi = "api";

        public static readonly Lazy<List<KeyValuePair<string, string>>> AllCategoryList = new Lazy<List<KeyValuePair<string, string>>>(
            () =>
            {
                var list = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(CategoryStl, "STL 解析错误"),
                    new KeyValuePair<string, string>(CategoryAdmin, "后台错误"),
                    new KeyValuePair<string, string>(CategoryHome, "用户中心错误"),
                    new KeyValuePair<string, string>(CategoryApi, "API错误")
                };
                return list;
            });

        private static int AddErrorLog(ErrorLogInfo logInfo)
        {
            try
            {
                if (!ConfigManager.Instance.IsLogError) return 0;

                DataProvider.ErrorLogDao.DeleteIfThreshold();

                return DataProvider.ErrorLogDao.Insert(logInfo);
            }
            catch
            {
                // ignored
            }

            return 0;
        }

        public static int AddErrorLog(Exception ex, string summary = "")
        {
            var logInfo = new ErrorLogInfo
            {
                Category = CategoryAdmin,
                PluginId = string.Empty,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary,
                AddDate = DateTime.Now
            };

            return AddErrorLog(logInfo);
        }

        public static int AddErrorLog(string pluginId, Exception ex, string summary = "")
        {
            var logInfo = new ErrorLogInfo
            {
                Category = CategoryAdmin,
                PluginId = pluginId,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary,
                AddDate = DateTime.Now
            };

            return AddErrorLog(logInfo);
        }

        public static string AddStlErrorLog(PageInfo pageInfo, string elementName, string stlContent, Exception ex)
        {
            var summary = string.Empty;
            if (pageInfo != null)
            {
                summary = $@"站点名称：{pageInfo.SiteInfo.SiteName}，
模板类型：{TemplateTypeUtils.GetText(pageInfo.TemplateInfo.Type)}，
模板名称：{pageInfo.TemplateInfo.TemplateName}
<br />";
            }

            summary += $@"STL标签：{StringUtils.HtmlEncode(stlContent)}";

            var logInfo = new ErrorLogInfo
            {
                Category = CategoryStl,
                PluginId = string.Empty,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary,
                AddDate = DateTime.Now
            };

            AddErrorLog(logInfo);

            return $@"
<!--
{elementName}
error: {ex.Message}
stl: {stlContent}
-->";
        }

        public static void AddSiteLog(int siteId, int channelId, int contentId, string ipAddress, string adminName, string action, string summary)
        {
            if (!ConfigManager.Instance.IsLogSite) return;

            if (siteId <= 0)
            {
                AddAdminLog(adminName, action, summary);
            }
            else
            {
                try
                {
                    DataProvider.SiteLogDao.DeleteIfThreshold();

                    if (!string.IsNullOrEmpty(action))
                    {
                        action = StringUtils.MaxLengthText(action, 250);
                    }
                    if (!string.IsNullOrEmpty(summary))
                    {
                        summary = StringUtils.MaxLengthText(summary, 250);
                    }
                    if (channelId < 0)
                    {
                        channelId = -channelId;
                    }

                    var siteLogInfo = new SiteLogInfo
                    {
                        SiteId = siteId,
                        ChannelId = channelId,
                        ContentId = contentId,
                        UserName = adminName,
                        IpAddress = ipAddress,
                        AddDate = DateTime.Now,
                        Action = action,
                        Summary = summary
                    };
                    DataProvider.SiteLogDao.Insert(siteLogInfo);
                }
                catch (Exception ex)
                {
                    AddErrorLog(ex);
                }
            }
        }

        public static void AddAdminLog(string ipAddress, string adminName, string action, string summary = "")
        {
            if (!ConfigManager.Instance.IsLogAdmin) return;

            try
            {
                DataProvider.LogDao.DeleteIfThreshold();

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
                    AddDate = DateTime.Now,
                    Action = action,
                    Summary = summary
                };

                DataProvider.LogDao.Insert(logInfo);
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
            }
        }

        public static void AddUserLog(string ipAddress, string userName, string actionType, string summary)
        {
            if (!ConfigManager.Instance.IsLogUser) return;

            try
            {
                DataProvider.UserLogDao.DeleteIfThreshold();

                if (!string.IsNullOrEmpty(summary))
                {
                    summary = StringUtils.MaxLengthText(summary, 250);
                }

                var userLogInfo = new UserLogInfo
                {
                    UserName = userName,
                    IpAddress = ipAddress,
                    AddDate = DateTime.Now,
                    Action = actionType,
                    Summary = summary
                };

                DataProvider.UserLogDao.Insert(userName, userLogInfo);
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
            }
        }
    }
}
