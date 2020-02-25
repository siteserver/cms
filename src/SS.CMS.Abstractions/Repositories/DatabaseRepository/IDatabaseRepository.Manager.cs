using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IDatabaseRepository
    {
        Task InstallDatabaseAsync(string adminName, string adminPassword);

        Task CreateSiteServerTablesAsync();

        Task SyncDatabaseAsync();

        Task SyncContentTablesAsync();
    }
}
