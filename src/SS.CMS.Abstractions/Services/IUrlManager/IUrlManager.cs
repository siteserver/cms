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

        string GetWebUrl(SiteInfo siteInfo, params string[] value);

        string GetAssetsUrl(SiteInfo siteInfo, params string[] value);

        string GetHomeUrl(SiteInfo siteInfo, params string[] value);

        string GetSiteUrl(SiteInfo siteInfo, bool isLocal);

        string GetSiteUrl(SiteInfo siteInfo, string requestPath, bool isLocal);

        string GetSiteUrlByPhysicalPath(SiteInfo siteInfo, string physicalPath, bool isLocal);

        string GetIndexPageUrl(SiteInfo siteInfo, bool isLocal);

        string GetFileUrl(SiteInfo siteInfo, int fileTemplateId, bool isLocal);

        Task<string> GetContentUrlAsync(SiteInfo siteInfo, ContentInfo contentInfo, bool isLocal);

        Task<string> GetContentUrlAsync(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId, bool isLocal);

        //得到栏目经过计算后的连接地址
        Task<string> GetChannelUrlAsync(SiteInfo siteInfo, ChannelInfo channelInfo, bool isLocal);

        Task<string> GetInputChannelUrlAsync(SiteInfo siteInfo, ChannelInfo nodeInfo, bool isLocal);

        string AddVirtualToUrl(string url);

        //根据发布系统属性判断是否为相对路径并返回解析后路径
        string ParseNavigationUrl(SiteInfo siteInfo, string url, bool isLocal);

        string GetVirtualUrl(SiteInfo siteInfo, string url);

        bool IsVirtualUrl(string url);

        string GetSiteFilesUrl(string apiUrl, string relatedUrl);

        string GetHomeUploadUrl(params string[] paths);

        string DefaultAvatarUrl { get; }

        string GetUserUploadUrl(int userId, string relatedUrl);

        string GetUserAvatarUrl(UserInfo userInfo);
    }
}