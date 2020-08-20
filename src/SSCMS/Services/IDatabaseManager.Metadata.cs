using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IDatabaseManager
    {
        Task<(bool success, string errorMessage)> InstallAsync(string userName, string password, string email,
            string mobile);

        Task CreateSiteServerTablesAsync();

        Task SyncDatabaseAsync();

        Task SyncContentTablesAsync();
    }
}
