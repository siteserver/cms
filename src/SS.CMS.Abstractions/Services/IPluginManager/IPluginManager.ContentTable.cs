using System.Threading.Tasks;

namespace SS.CMS.Services
{
    public partial interface IPluginManager
    {
        Task SyncContentTableAsync(IService service);

        Task<bool> IsContentTableUsedAsync(string tableName);
    }
}
