using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IDatabaseManager
    {
        Task<(bool success, string errorMessage)> InstallAsync(IPluginManager pluginManager, string userName, string password, string email,
            string mobile);

        Task CreateSiteServerTablesAsync();

        Task SyncDatabaseAsync(IPluginManager pluginManager);

        Task SyncContentTablesAsync(IPluginManager pluginManager);
    }
}
