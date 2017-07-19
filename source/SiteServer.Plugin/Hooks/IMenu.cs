namespace SiteServer.Plugin.Hooks
{
    public interface IMenu : IHooks
    {
        PluginMenu GetTopMenu();
        PluginMenu GetSiteMenu(int siteId);
    }
}
