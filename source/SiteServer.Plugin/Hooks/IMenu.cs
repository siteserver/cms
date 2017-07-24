namespace SiteServer.Plugin.Hooks
{
    public interface IMenu : IPlugin
    {
        PluginMenu GetTopMenu();
        PluginMenu GetSiteMenu(int siteId);
    }
}
