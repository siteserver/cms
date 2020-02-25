using System.Threading.Tasks;


namespace SS.CMS.Abstractions
{
    public partial interface IPluginManager
    {
        Task SyncContentTableAsync(IPluginService service);

        Task<bool> IsContentTableUsedAsync(string tableName);
    }
}
