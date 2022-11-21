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

        public async Task<SmsSettings> GetSmsSettingsAsync()
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            return new SmsSettings
            {
                IsSms = isAuthentication && config.IsCloudSms,
                IsSmsAdministrator = config.IsCloudSmsAdministrator,
                IsSmsUser = config.IsCloudSmsUser,
            };
        }

        public Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, string templateCode, Dictionary<string, string> parameters)
        {
            throw new System.NotImplementedException();
        }

        public class SendSmsRequest
        {
            public SmsCodeType Type { get; set; }
            public string Mobile { get; set; }
            public int Code { get; set; }
        }

        public async Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType, int code)
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
                Type = codeType,
                Mobile = phoneNumbers,
                Code = code
            }, config.CloudToken);

            if (!success)
            {
                throw new Exception(errorMessage);
            }

            return (result.Success, result.ErrorMessage);
        }
    }
}
