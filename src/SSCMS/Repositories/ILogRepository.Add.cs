using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface ILogRepository
    {
        Task AddAdminLogAsync(Administrator adminInfo, string action, string summary = "");
    }
}
