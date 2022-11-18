using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class CloudManager
    {
        // [Obsolete]
        // public Task<bool> IsEnabledAsync()
        // {
        //     return Task.FromResult(false);
        // }

        [Obsolete]
        public Task<(bool success, string errorMessage)> SendAsync(string mail, string subject, string htmlBody)
        {
            return Task.FromResult((false, "未启用邮件功能"));
        }

        public async Task<MailSettings> GetMailSettingsAsync()
        {
            var config = await _configRepository.GetAsync();
            return new MailSettings
            {
                IsMail = config.IsCloudMail,
                IsMailContentAdd = config.IsCloudMailContentAdd,
                IsMailContentEdit = config.IsCloudMailContentEdit,
                MailAddress = config.CloudMailAddress,
            };
        }

        public class SendMailRequest
        {
            public string Mail { get; set; }
            public string Subject { get; set; }
            public string HtmlBody { get; set; }
        }

        public async Task<(bool success, string errorMessage)> SendMailAsync(string mail, string subject, string htmlBody)
        {
            var config = await _configRepository.GetAsync();
            if (string.IsNullOrEmpty(config.CloudUserName) || string.IsNullOrEmpty(config.CloudToken))
            {
                throw new Exception("云助手未登录");
            }

            if (string.IsNullOrEmpty(mail))
            {
                throw new Exception("邮箱地址不能为空");
            }

            var url = GetCloudUrl(RouteMail);
            var (success, result, errorMessage) = await RestUtils.PostAsync<SendMailRequest, CloudResult>(url, new SendMailRequest
            {
                Mail = mail,
                Subject = subject,
                HtmlBody = htmlBody
            }, config.CloudToken);

            if (!success)
            {
                throw new Exception(errorMessage);
            }

            return (result.Success, result.ErrorMessage);
        }

        public static async Task<string> GetMailTemplateHtmlAsync(IPathManager pathManager, ICacheManager cacheManager)
        {
            var htmlPath = pathManager.GetSiteFilesPath("assets/mail/template.html");
            if (cacheManager.Exists(htmlPath)) return cacheManager.Get<string>(htmlPath);

            var html = await FileUtils.ReadTextAsync(htmlPath);

            cacheManager.AddOrUpdate(htmlPath, html);
            return html;
        }

        public static async Task<string> GetMailListHtmlAsync(IPathManager pathManager, ICacheManager cacheManager)
        {
            var htmlPath = pathManager.GetSiteFilesPath("assets/mail/list.html");
            if (cacheManager.Exists(htmlPath)) return cacheManager.Get<string>(htmlPath);

            var html = await FileUtils.ReadTextAsync(htmlPath);

            cacheManager.AddOrUpdate(htmlPath, html);
            return html;
        }

        public static async Task SendContentChangedMail(IPathManager pathManager, ICacheManager cacheManager, IMailManager mailManager, IErrorLogRepository errorLogRepository, Site site, Content content, string channelNames, string userName, bool isEdit)
        {
            try
            {
                var mailSettings = await mailManager.GetMailSettingsAsync();
                if (mailSettings == null || !mailSettings.IsMail)
                {
                    return;
                }

                if (isEdit)
                {
                    if (!mailSettings.IsMailContentEdit)
                    {
                        return;
                    }
                }
                else
                {
                    if (!mailSettings.IsMailContentAdd)
                    {
                        return;
                    }
                }

                var action = isEdit ? "修改" : "添加";
                var templateHtml = await GetMailTemplateHtmlAsync(pathManager, cacheManager);
                var listHtml = await GetMailListHtmlAsync(pathManager, cacheManager);

                var url = await pathManager.GetContentUrlByIdAsync(site, content, false);

                var list = new StringBuilder();
                var keyValueList = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("内容标题", content.Title),
                    new KeyValuePair<string, string>("栏目", channelNames),
                    new KeyValuePair<string, string>("内容Id", content.Id.ToString()),
                    new KeyValuePair<string, string>($"{action}时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm")),
                    new KeyValuePair<string, string>("执行人", userName)
                };
                foreach (var kv in keyValueList)
                {
                    list.Append(listHtml.Replace("{{key}}", kv.Key).Replace("{{value}}", kv.Value));
                }

                var subject = $"{action}内容";
                var htmlBody = templateHtml
                    .Replace("{{title}}", subject)
                    .Replace("{{url}}", url)
                    .Replace("{{list}}", list.ToString());

                await mailManager.SendMailAsync(mailSettings.MailAddress, subject, htmlBody);
            }
            catch (Exception ex)
            {
                await errorLogRepository.AddErrorLogAsync(ex);
            }
        }
    }
}
