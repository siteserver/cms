using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface ILogRepository
    {
        Task AddAdminLogAsync(Administrator adminInfo, string action, string summary = "");
    }
}
