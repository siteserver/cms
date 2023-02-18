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

        public async Task<bool> IsMailEnabledAsync()
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            return isAuthentication && config.IsCloudMail;
        }

        public async Task<MailSettings> GetMailSettingsAsync()
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            return new MailSettings
            {
                IsMail = isAuthentication && config.IsCloudMail,
                IsMailContentAdd = config.IsCloudMailContentAdd,
                IsMailContentEdit = config.IsCloudMailContentEdit,
                MailAddress = config.CloudMailAddress,
            };
        }

        public class SendMailRequest
        {
            public string FromAlias { get; set; }
            public string Mail { get; set; }
            public string Subject { get; set; }
            public string HtmlBody { get; set; }
        }

        public async Task<(bool success, string errorMessage)> SendMailAsync(string mail, string subject, string htmlBody)
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            if (!isAuthentication)
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
                FromAlias = config.CloudMailFromAlias,
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

        public async Task<(bool success, string errorMessage)> SendMailAsync(string mail, string subject, string url, List<KeyValuePair<string, string>> items)
        {
            var templateHtml = await _pathManager.GetMailTemplateHtmlAsync();
            var listHtml = await _pathManager.GetMailListHtmlAsync();

            var list = new StringBuilder();
            foreach (var kv in items)
            {
                list.Append(listHtml.Replace("{{key}}", kv.Key).Replace("{{value}}", kv.Value));
            }

            var htmlBody = templateHtml
                .Replace("{{title}}", subject)
                .Replace("{{url}}", url)
                .Replace("{{list}}", list.ToString());

            if (string.IsNullOrEmpty(url))
            {
                htmlBody = htmlBody.Replace(@"<a class=""btn-link"" href=""""
            style=""margin: 0;padding: 0;font-family: Helvetica Neue, Microsoft Yahei, Hiragino Sans GB, WenQuanYi Micro Hei, sans-serif;word-break: break-word;display: inline-block;background: #1b9aee;box-shadow: 0 1px 3px 0 rgba(27,154,238,0.12);border-radius: 6px;width: 220px;height: 64px;line-height: 64px;border-radius: 6px;text-align: center;font-size: 24px;color: #fff;text-decoration: none;font-weight: 400;"">查看详情</a>", string.Empty);
            }

            return await SendMailAsync(mail, subject, htmlBody);
        }

        public static async Task SendContentChangedMail(IPathManager pathManager, IMailManager mailManager, IErrorLogRepository errorLogRepository, Site site, Content content, string channelNames, string userName, bool isEdit)
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
                var subject = $"{action}内容";
                var url = await pathManager.GetContentUrlByIdAsync(site, content, false);
                var items = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("内容标题", content.Title),
                    new KeyValuePair<string, string>("栏目", channelNames),
                    new KeyValuePair<string, string>("内容Id", content.Id.ToString()),
                    new KeyValuePair<string, string>($"{action}时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm")),
                    new KeyValuePair<string, string>("执行人", userName)
                };

                await mailManager.SendMailAsync(mailSettings.MailAddress, subject, url, items);
            }
            catch (Exception ex)
            {
                await errorLogRepository.AddErrorLogAsync(ex);
            }
        }
    }
}
