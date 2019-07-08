using System.Threading.Tasks;

namespace SS.CMS.Repositories
{
    public partial interface ILogRepository
    {
        Task AddAdminLogAsync(string ipAddress, int userId, string action, string summary = "");
    }
}
