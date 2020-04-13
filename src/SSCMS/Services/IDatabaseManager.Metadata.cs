using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IDatabaseManager
    {
        Task<(bool success, string errorMessage)> InstallAsync(IOldPluginManager pluginManager, string userName, string password, string email,
            string mobile);

        Task CreateSiteServerTablesAsync();

        Task SyncDatabaseAsync(IOldPluginManager pluginManager);

        Task SyncContentTablesAsync(IOldPluginManager pluginManager);
    }
}
