using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Enums;

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

        public Task<bool> IsSmsAsync()
        {
            return Task.FromResult(false);
        }

        public Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, string templateCode, Dictionary<string, string> parameters)
        {
            return Task.FromResult((false, "未启用短信功能"));
        }

        public Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType, int code)
        {
            return Task.FromResult((false, "未启用短信功能"));
        }
    }
}
