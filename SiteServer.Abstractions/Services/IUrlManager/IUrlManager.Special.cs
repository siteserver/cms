using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IUrlManager
    {
        string GetSpecialUrl(Site siteInfo, string url);

        Task<string> GetSpecialUrlAsync(Site siteInfo, int specialId);

        Task<string> GetSpecialUrlAsync(Site siteInfo, int specialId, bool isLocal);
    }
}