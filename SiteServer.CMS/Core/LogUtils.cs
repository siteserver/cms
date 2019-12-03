using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Context;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Model;

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

        private static async Task<int> AddErrorLogAsync(ErrorLog log)
        {
            try
            {
                var config = await DataProvider.ConfigRepository.GetAsync();
                if (!config.IsLogError) return 0;

                await DataProvider.ErrorLogRepository.DeleteIfThresholdAsync();

                return await DataProvider.ErrorLogRepository.InsertAsync(log);
            }
            catch
            {
                // ignored
            }

            return 0;
        }

        public static async Task AddErrorLogAndRedirectAsync(Exception ex, string summary = "")
        {
            if (ex == null || ex.StackTrace.Contains("System.Web.HttpResponse.set_StatusCode(Int32 value)")) return;

            var logId = await AddErrorLogAsync(ex, summary);
            if (logId > 0)
            {
                PageUtils.RedirectToErrorPage(logId);
            }
            else
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        public static async Task<int> AddErrorLogAsync(Exception ex, string summary = "")
        {
            return await AddErrorLogAsync(new ErrorLog
            {
                Id = 0,
                Category = CategoryAdmin,
                PluginId = string.Empty,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary,
                AddDate = DateTime.Now
            });
        }
        public static async Task<int> AddErrorLogAsync(string pluginId, Exception ex, string summary = "")
        {
            return await AddErrorLogAsync(new ErrorLog
            {
                Id = 0,
                Category = CategoryAdmin,
                PluginId = pluginId,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary,
                AddDate = DateTime.Now
            });
        }

        public static async Task<string> AddStlErrorLogAsync(PageInfo pageInfo, string elementName, string stlContent, Exception ex)
        {
            var summary = string.Empty;
            if (pageInfo != null)
            {
                summary = $@"站点名称：{pageInfo.Site.SiteName}，
模板类型：{TemplateTypeUtils.GetText(pageInfo.Template.Type)}，
模板名称：{pageInfo.Template.TemplateName}
<br />";
            }

            summary += $@"STL标签：{WebUtils.HtmlEncode(stlContent)}";
            await AddErrorLogAsync(new ErrorLog
            {
                Id = 0,
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

        public static async Task AddSiteLogAsync(int siteId, int channelId, int contentId, Administrator adminInfo, string action, string summary)
        {
            var config = await DataProvider.ConfigRepository.GetAsync();
            if (!config.IsLogSite) return;

            if (siteId <= 0)
            {
                await AddAdminLogAsync(adminInfo, action, summary);
            }
            else
            {
                try
                {
                    await DataProvider.SiteLogRepository.DeleteIfThresholdAsync();

                    if (!string.IsNullOrEmpty(action))
                    {
                        action = WebUtils.MaxLengthText(action, 250);
                    }
                    if (!string.IsNullOrEmpty(summary))
                    {
                        summary = WebUtils.MaxLengthText(summary, 250);
                    }
                    if (channelId < 0)
                    {
                        channelId = -channelId;
                    }

                    var siteLogInfo = new SiteLog
                    {
                        Id = 0,
                        SiteId = siteId,
                        ChannelId = channelId,
                        ContentId = contentId,
                        UserName = adminInfo.UserName,
                        IpAddress = PageUtils.GetIpAddress(),
                        AddDate = DateTime.Now,
                        Action = action,
                        Summary = summary
                    };

                    await DataProvider.SiteLogRepository.InsertAsync(siteLogInfo);

                    await DataProvider.AdministratorRepository.UpdateLastActivityDateAsync(adminInfo);
                }
                catch (Exception ex)
                {
                    await AddErrorLogAsync(ex);
                }
            }
        }

        public static async Task AddAdminLogAsync(Administrator adminInfo, string action, string summary = "")
        {
            var config = await DataProvider.ConfigRepository.GetAsync();
            if (!config.IsLogAdmin) return;

            try
            {
                await DataProvider.LogRepository.DeleteIfThresholdAsync();

                if (!string.IsNullOrEmpty(action))
                {
                    action = WebUtils.MaxLengthText(action, 250);
                }
                if (!string.IsNullOrEmpty(summary))
                {
                    summary = WebUtils.MaxLengthText(summary, 250);
                }

                var logInfo = new Log
                {
                    Id = 0,
                    UserName = adminInfo.UserName,
                    IpAddress = PageUtils.GetIpAddress(),
                    AddDate = DateTime.Now,
                    Action = action,
                    Summary = summary
                };

                await DataProvider.LogRepository.InsertAsync(logInfo);

                await DataProvider.AdministratorRepository.UpdateLastActivityDateAsync(adminInfo);
            }
            catch (Exception ex)
            {
                await AddErrorLogAsync(ex);
            }
        }

        public static async Task AddUserLoginLogAsync(string userName)
        {
            await AddUserLogAsync(userName, "用户登录", string.Empty);
        }

        public static async Task AddUserLogAsync(string userName, string actionType, string summary)
        {
            var config = await DataProvider.ConfigRepository.GetAsync();
            if (!config.IsLogUser) return;

            try
            {
                await DataProvider.UserLogRepository.DeleteIfThresholdAsync();

                if (!string.IsNullOrEmpty(summary))
                {
                    summary = WebUtils.MaxLengthText(summary, 250);
                }

                var userLogInfo = new UserLog
                {
                    Id = 0,
                    UserName = userName,
                    IpAddress = PageUtils.GetIpAddress(),
                    AddDate = DateTime.Now,
                    Action = actionType,
                    Summary = summary
                };

                await DataProvider.UserLogRepository.InsertAsync(userLogInfo);
            }
            catch (Exception ex)
            {
                await AddErrorLogAsync(ex);
            }
        }
    }
}
