using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IPluginManager
    {
        Task SyncContentTableAsync(IService service);

        Task<bool> IsContentTableUsedAsync(string tableName);
    }
}
