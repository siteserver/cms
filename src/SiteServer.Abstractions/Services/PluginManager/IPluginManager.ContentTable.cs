using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IPluginManager
    {
        Task SyncContentTableAsync(IPluginService service);

        Task<bool> IsContentTableUsedAsync(string tableName);
    }
}
