using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Services
{
    public interface IMailManager
    {
        Task<bool> IsEnabledAsync();

        Task<(bool success, string errorMessage)> SendAsync(string mail, string subject, string htmlBody);
    }
}
