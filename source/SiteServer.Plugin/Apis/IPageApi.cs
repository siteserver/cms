using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Apis
{
    public interface IPageApi
    {
        string GetPluginPageUrl(int publishmentSystemId, string relatedUrl = "");

        string GetPluginJsonApiUrl(int publishmentSystemId, string action = "", int id = 0);

        string GetPluginHttpApiUrl(int publishmentSystemId, string action = "", int id = 0);

        string FilterXss(string html);

        string GetHomeUrl(int publishmentSystemId, string relatedUrl = "");

        string GetHomeLoginUrl(int publishmentSystemId, string returnUrl);

        string GetHomeLogoutUrl(int publishmentSystemId, string returnUrl);

        string GetHomeRegisterUrl(int publishmentSystemId, string returnUrl);

        string GetCurrentUrl(PluginParseContext context);

        string GetPublishmentSystemUrl(int publishmentSystemId);

        string GetPublishmentSystemUrlByFilePath(string filePath);

        string GetChannelUrl(int publishmentSystemId, int channelId);

        string GetContentUrl(int publishmentSystemId, int channelId, int contentId);

        string GetRootUrl(string relatedUrl);
    }
}
