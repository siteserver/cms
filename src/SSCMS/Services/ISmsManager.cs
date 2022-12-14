using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Services
{
    public interface ISmsManager
    {
        [Obsolete]
        Task<bool> IsEnabledAsync();

        [Obsolete]
        Task<(bool success, string errorMessage)> SendAsync(string phoneNumbers, string templateCode,
            Dictionary<string, string> parameters = null);

        [Obsolete]
        Task<(bool success, string errorMessage)> SendAsync(string phoneNumbers, SmsCodeType codeType,
            int code);

        Task<bool> IsSmsEnabledAsync();

        Task<SmsSettings> GetSmsSettingsAsync();

        Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, string templateCode,
            Dictionary<string, string> parameters = null);

        Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType,
            Dictionary<string, string> parameters = null);

        Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType,
            int code);
    }
}
