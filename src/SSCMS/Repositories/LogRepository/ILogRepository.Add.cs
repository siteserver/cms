using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface ILogRepository
    {
        Task AddAdminLogAsync(Administrator adminInfo, string action, string summary = "");
    }
}
