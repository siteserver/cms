using System;
using System.Threading.Tasks;
using SSCMS.Dto;
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

        public async Task<bool> IsMailAsync()
        {
            var config = await _configRepository.GetAsync();
            return config.IsCloudMail;
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

            var url = GetCloudUrl(RouteGetDownloadUrl);
            var (success, result, errorMessage) = await RestUtils.PostAsync<SendMailRequest, BoolResult>(url, new SendMailRequest
            {
                Mail = mail,
                Subject = subject,
                HtmlBody = htmlBody
            }, config.CloudToken);

            return (success, errorMessage);
        }
    }
}
