namespace SSCMS.Services
{
    public partial interface IPluginManager
    {
        void Install(string userName, string name, string version);

        void UnInstall(string pluginId);
    }
}
