using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IUrlManager
    {
        string GetRootUrl(string relatedUrl);

        string GetApiUrl(string route);

        Task<string> GetSystemDefaultPageUrlAsync(int siteId);

        Task<string> GetHomeDefaultPageUrlAsync();

        string GetMenuUrl(string pluginId, string href, int siteId, int channelId, int contentId);

        string GetWebUrl(Site siteInfo, params string[] value);

        string GetAssetsUrl(Site siteInfo, params string[] value);

        string GetHomeUrl(Site siteInfo, params string[] value);

        string GetSiteUrl(Site siteInfo, bool isLocal);

        string GetSiteUrl(Site siteInfo, string requestPath, bool isLocal);

        string GetSiteUrlByPhysicalPath(Site siteInfo, string physicalPath, bool isLocal);

        Task<string> GetIndexPageUrlAsync(Site siteInfo, bool isLocal);

        Task<string> GetFileUrlAsync(Site siteInfo, int fileTemplateId, bool isLocal);

        Task<string> GetContentUrlAsync(Site siteInfo, Content contentInfo, bool isLocal);

        Task<string> GetContentUrlAsync(Site siteInfo, Channel channelInfo, int contentId, bool isLocal);

        //得到栏目经过计算后的连接地址
        Task<string> GetChannelUrlAsync(Site siteInfo, Channel channelInfo, bool isLocal);

        Task<string> GetInputChannelUrlAsync(Site siteInfo, Channel nodeInfo, bool isLocal);

        string AddVirtualToUrl(string url);

        //根据发布系统属性判断是否为相对路径并返回解析后路径
        string ParseNavigationUrl(Site siteInfo, string url, bool isLocal);

        string GetVirtualUrl(Site siteInfo, string url);

        bool IsVirtualUrl(string url);

        string GetSiteFilesUrl(string apiUrl, string relatedUrl);

        string GetHomeUploadUrl(params string[] paths);

        string DefaultAvatarUrl { get; }

        string GetUserUploadUrl(int userId, string relatedUrl);

        string GetUserAvatarUrl(User userInfo);
    }
}