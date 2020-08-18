namespace SSCMS.Services
{
    public partial interface IPluginManager
    {
        void Install(string pluginId, string version);

        void UnInstall(string pluginId);
    }
}
