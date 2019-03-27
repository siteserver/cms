using System;
using System.Collections.Generic;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
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

                DataProvider.ErrorLog.DeleteIfThreshold();

                return DataProvider.ErrorLog.Insert(logInfo);
            }
            catch
            {
                // ignored
            }

            return 0;
        }

        public static void AddErrorLogAndRedirect(Exception ex, string summary = "")
        {
            if (ex == null || ex.StackTrace.Contains("System.Web.HttpResponse.set_StatusCode(Int32 value)")) return;

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

        public static int AddErrorLog(Exception ex, string summary = "")
        {
            //0, CategoryAdmin, string.Empty, ex.Message, ex.StackTrace, summary, DateTime.Now
            return AddErrorLog(new ErrorLogInfo
            {
                Category = CategoryAdmin,
                PluginId = string.Empty,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary,
                AddDate = DateTime.Now
            });
        }
        public static void AddErrorLog(string pluginId, Exception ex, string summary = "")
        {
            AddErrorLog(new ErrorLogInfo
            {
                Category = CategoryAdmin,
                PluginId = pluginId,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary,
                AddDate = DateTime.Now
            });
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
            AddErrorLog(new ErrorLogInfo
            {
                Category = CategoryStl,
                PluginId = string.Empty,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary,
                AddDate = DateTime.Now
            });

            return $@"
<!--
{elementName}
error: {ex.Message}
stl: {stlContent}
-->";
        }

        public static void AddSiteLog(int siteId, int channelId, int contentId, string adminName, string action, string summary)
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
                    DataProvider.SiteLog.DeleteIfThreshold();

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
                        IpAddress = PageUtils.GetIpAddress(),
                        AddDate = DateTime.Now,
                        Action = action,
                        Summary = summary
                    };

                    DataProvider.SiteLog.Insert(siteLogInfo);
                }
                catch (Exception ex)
                {
                    AddErrorLog(ex);
                }
            }
        }

        public static void AddAdminLog(string adminName, string action, string summary = "")
        {
            if (!ConfigManager.Instance.IsLogAdmin) return;

            try
            {
                DataProvider.Log.DeleteIfThreshold();

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
                    IpAddress = PageUtils.GetIpAddress(),
                    AddDate = DateTime.Now,
                    Action = action,
                    Summary = summary
                };

                DataProvider.Log.Insert(logInfo);
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
            if (!ConfigManager.Instance.IsLogUser) return;

            try
            {
                DataProvider.UserLog.DeleteIfThreshold();

                if (!string.IsNullOrEmpty(summary))
                {
                    summary = StringUtils.MaxLengthText(summary, 250);
                }

                var userLogInfo = new UserLogInfo
                {
                    UserName = userName,
                    IpAddress = PageUtils.GetIpAddress(),
                    AddDate = DateTime.Now,
                    Action = actionType,
                    Summary = summary
                };

                DataProvider.UserLog.Insert(userLogInfo);
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
            }
        }
    }
}
