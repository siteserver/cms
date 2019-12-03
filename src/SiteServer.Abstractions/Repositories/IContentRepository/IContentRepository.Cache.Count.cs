using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IContentRepository
    {
        Task<int> GetCountAsync(Site siteInfo, bool isChecked, IPluginManager pluginManager);

        Task<int> GetCountAsync(Site siteInfo, Channel channelInfo, int? onlyAdminId);

        Task<int> GetCountAsync(Site siteInfo, Channel channelInfo, bool isChecked);
    }
}