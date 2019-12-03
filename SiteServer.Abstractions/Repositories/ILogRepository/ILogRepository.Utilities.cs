using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface ILogRepository
    {
        Task AddAdminLogAsync(string ipAddress, int userId, string action, string summary = "");
    }
}
