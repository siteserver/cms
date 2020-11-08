using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Services;

namespace SSCMS.Core.Services
{
    public class SmsManager : ISmsManager
    {
        public Task<bool> IsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        public Task<(bool success, string errorMessage)> SendAsync(string phoneNumbers, string templateCode, Dictionary<string, string> parameters)
        {
            return Task.FromResult((false, "未启用短信功能"));
        }

        public Task<(bool success, string errorMessage)> SendAsync(string phoneNumbers, SmsCodeType codeType, int code)
        {
            return Task.FromResult((false, "未启用短信功能"));
        }
    }
}
