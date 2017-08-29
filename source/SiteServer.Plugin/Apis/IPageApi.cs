namespace SiteServer.Plugin.Apis
{
    public interface IPageApi
    {
        string GetPluginPageUrl(int publishmentSystemId, string relatedUrl = "");

        string GetPluginJsonApiUrl(int publishmentSystemId, string action = "", int id = 0);

        string GetPluginHttpApiUrl(int publishmentSystemId, string action = "", int id = 0);

        string FilterXss(string html);
    }
}
