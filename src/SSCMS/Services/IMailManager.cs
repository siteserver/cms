using System;
using System.Threading.Tasks;

namespace SSCMS.Services
{
    public interface IMailManager
    {
        [Obsolete]
        Task<bool> IsEnabledAsync();

        [Obsolete]
        Task<(bool success, string errorMessage)> SendAsync(string mail, string subject, string htmlBody);

        Task<bool> IsMailAsync();

        Task<(bool success, string errorMessage)> SendMailAsync(string mail, string subject, string htmlBody);
    }
}
