using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Enums;

namespace SSCMS.Services
{
    public interface ISmsManager
    {
        Task<bool> IsEnabledAsync();

        Task<(bool success, string errorMessage)> SendAsync(string phoneNumbers, string templateCode,
            Dictionary<string, string> parameters = null);

        Task<(bool success, string errorMessage)> SendAsync(string phoneNumbers, SmsCodeType codeType,
            int code);
    }
}
