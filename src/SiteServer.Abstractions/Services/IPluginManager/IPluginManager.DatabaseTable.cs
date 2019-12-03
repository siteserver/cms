using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IPluginManager
    {
        Task SyncTableAsync(IService service);
    }
}
