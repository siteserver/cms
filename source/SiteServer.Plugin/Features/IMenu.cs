namespace SiteServer.Plugin.Features
{
    public interface IMenu : IPlugin
    {
        PluginMenu GetTopMenu();
        PluginMenu GetSiteMenu(int siteId);
    }
}
