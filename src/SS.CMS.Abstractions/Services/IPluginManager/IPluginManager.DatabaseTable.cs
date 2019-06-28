using System.Threading.Tasks;

namespace SS.CMS.Services
{
    public partial interface IPluginManager
    {
        Task SyncTableAsync(IService service);
    }
}
