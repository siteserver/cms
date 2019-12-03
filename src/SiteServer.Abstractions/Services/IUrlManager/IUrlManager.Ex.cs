
namespace SiteServer.Abstractions
{
    public partial interface IUrlManager
    {
        string GetTemporaryFilesUrl(string relatedUrl);

        string GetSiteTemplatesUrl(string relatedUrl);

        string GetSiteTemplateMetadataUrl(string siteTemplateUrl, string relatedUrl);

        string ParsePluginUrl(string pluginId, string url);

        string GetRootUrlByPhysicalPath(string physicalPath);

        string ParseNavigationUrl(string url);

        string AddProtocolToUrl(string url);

        string AddProtocolToUrl(string url, string host);

        string GetUrlWithReturnUrl(string pageUrl, string returnUrl);

        string GetUrlByBaseUrl(string rawUrl, string baseUrl);

        string ParseConfigRootUrl(string url);

        string GetAdministratorUploadUrl(params string[] paths);

        string GetAdministratorUploadUrl(int userId, string relatedUrl);
    }
}
