using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IUrlManager
    {
        string GetSystemDefaultPageUrl(int siteId);

        string GetHomeDefaultPageUrl();

        string GetMenuUrl(string pluginId, string href, int siteId, int channelId, int contentId);

        string ApiUrl { get; }

        string InnerApiUrl { get; }

        string GetApiUrl(string route);

        string GetInnerApiUrl(string route);

        string GetWebUrl(SiteInfo siteInfo);

        string GetAssetsUrl(SiteInfo siteInfo);

        string GetRootUrl(string relatedUrl);

        string GetHomeUrl(string relatedUrl);

        string GetSiteUrl(SiteInfo siteInfo, bool isLocal);

        string GetSiteUrl(SiteInfo siteInfo, string requestPath, bool isLocal);

        string GetSiteUrlByPhysicalPath(SiteInfo siteInfo, string physicalPath, bool isLocal);

        string GetIndexPageUrl(SiteInfo siteInfo, bool isLocal);

        string GetFileUrl(SiteInfo siteInfo, int fileTemplateId, bool isLocal);

        string GetContentUrl(SiteInfo siteInfo, ContentInfo contentInfo, bool isLocal);

        string GetContentUrl(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId, bool isLocal);

        //得到栏目经过计算后的连接地址
        string GetChannelUrl(SiteInfo siteInfo, ChannelInfo channelInfo, bool isLocal);

        string GetInputChannelUrl(SiteInfo siteInfo, ChannelInfo nodeInfo, bool isLocal);

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