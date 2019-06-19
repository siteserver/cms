using System.Collections.Specialized;

namespace SS.CMS.Services.IUrlManager
{
    public partial interface IUrlManager
    {
        // 系统根目录访问地址
        string GetMainUrl(int siteId);

        string GetSiteFilesUrl(string relatedUrl);

        string GetTemporaryFilesUrl(string relatedUrl);

        string GetSiteTemplatesUrl(string relatedUrl);

        string GetSiteTemplateMetadataUrl(string siteTemplateUrl, string relatedUrl);

        string ParsePluginUrl(string pluginId, string url);

        string GetSiteServerUrl(string className);

        string GetSiteServerUrl(string className, NameValueCollection queryString);

        string GetPluginsUrl(string className);

        string GetPluginsUrl(string className, NameValueCollection queryString);

        string GetSettingsUrl(string className);

        string GetSettingsUrl(string className, NameValueCollection queryString);

        string GetCmsUrl(string pageName, int siteId, object param = null);

        string GetCmsUrl(int siteId, string className, NameValueCollection queryString);

        string GetCmsWebHandlerUrl(int siteId, string className, NameValueCollection queryString);

        string GetAjaxUrl(string className, NameValueCollection queryString);

        string GetRootUrlByPhysicalPath(string physicalPath);

        string GetLoadingUrl(string url);

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
