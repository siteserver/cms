using System.Threading.Tasks;
using SSCMS.Services;

namespace SSCMS.Core.Services
{
    public class MailManager : IMailManager
    {
        public Task<bool> IsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        public Task<(bool success, string errorMessage)> SendAsync(string mail, string subject, string htmlBody)
        {
            return Task.FromResult((false, "未启用邮件功能"));
        }
    }
}
