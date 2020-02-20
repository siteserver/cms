using System.Threading.Tasks;

namespace SiteServer.Abstractions
{
    public partial interface ILogRepository
    {
        Task AddAdminLogAsync(Administrator adminInfo, string action, string summary = "");
    }
}
