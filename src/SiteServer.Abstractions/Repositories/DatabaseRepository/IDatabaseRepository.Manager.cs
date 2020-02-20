using System.Threading.Tasks;

namespace SiteServer.Abstractions
{
    public partial interface IDatabaseRepository
    {
        Task InstallDatabaseAsync(string adminName, string adminPassword);

        Task CreateSiteServerTablesAsync();

        Task SyncDatabaseAsync();

        Task SyncContentTablesAsync();
    }
}
