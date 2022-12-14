using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class CloudManager
    {
        private class SendSmsRequest
        {
            public string PhoneNumbers { get; set; }
            public SmsCodeType CodeType { get; set; }
            public string TemplateCode { get; set; }
            public Dictionary<string, string> Parameters { get; set; }
        }

        [Obsolete]
        public Task<bool> IsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        [Obsolete]
        public Task<(bool success, string errorMessage)> SendAsync(string phoneNumbers, string templateCode, Dictionary<string, string> parameters)
        {
            return Task.FromResult((false, "未启用短信功能"));
        }

        [Obsolete]
        public Task<(bool success, string errorMessage)> SendAsync(string phoneNumbers, SmsCodeType codeType, int code)
        {
            return Task.FromResult((false, "未启用短信功能"));
        }

        public async Task<bool> IsSmsEnabledAsync()
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            return isAuthentication && config.IsCloudSms;
        }

        public async Task<SmsSettings> GetSmsSettingsAsync()
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            return new SmsSettings
            {
                IsSms = isAuthentication && config.IsCloudSms,
                IsSmsAdmin = config.IsCloudSmsAdmin,
                IsSmsAdminAndDisableAccount = config.IsCloudSmsAdminAndDisableAccount,
                IsSmsUser = config.IsCloudSmsUser,
            };
        }

        public async Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, string templateCode, Dictionary<string, string> parameters)
        {
            return await SendSmsAsync(phoneNumbers, SmsCodeType.None, templateCode, parameters);
        }

        public async Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType,
            Dictionary<string, string> parameters = null)
        {
            return await SendSmsAsync(phoneNumbers, codeType, string.Empty, parameters);
        }

        public async Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType, int code)
        {
            var parameters = new Dictionary<string, string> { { "code", code.ToString() } };
            return await SendSmsAsync(phoneNumbers, codeType, string.Empty, parameters);
        }

        public async Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType, string templateCode, Dictionary<string, string> parameters)
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            if (!isAuthentication)
            {
                throw new Exception("云助手未登录");
            }

            if (string.IsNullOrEmpty(phoneNumbers))
            {
                throw new Exception("手机号码不能为空");
            }

            var url = GetCloudUrl(RouteSms);
            var (success, result, errorMessage) = await RestUtils.PostAsync<SendSmsRequest, CloudResult>(url, new SendSmsRequest
            {
                PhoneNumbers = phoneNumbers,
                CodeType = codeType,
                TemplateCode = templateCode,
                Parameters = parameters
            }, config.CloudToken);

            if (!success)
            {
                throw new Exception(errorMessage);
            }

            return (result.Success, result.ErrorMessage);
        }
    }
}
