using System.Threading.Tasks;

namespace SS.CMS.Repositories
{
    public partial interface ILogRepository
    {
        Task AddAdminLogAsync(string ipAddress, string adminName, string action, string summary = "");
    }
}
