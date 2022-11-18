using System;
using System.Threading.Tasks;
using SSCMS.Dto;

namespace SSCMS.Services
{
    public interface IMailManager
    {
        [Obsolete]
        Task<bool> IsEnabledAsync();

        [Obsolete]
        Task<(bool success, string errorMessage)> SendAsync(string mail, string subject, string htmlBody);

        Task<MailSettings> GetMailSettingsAsync();

        Task<(bool success, string errorMessage)> SendMailAsync(string mail, string subject, string htmlBody);
    }
}
