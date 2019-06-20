using System.Collections.Generic;

namespace SS.CMS.Services
{
    /// <summary>
    /// The entry for managing SiteServer plugins
    /// </summary>
    public partial interface IPluginManager
    {
        IPackageMetadata GetMetadata(string pluginId);

        bool IsExists(string pluginId);

        List<IPluginInstance> PluginInfoListRunnable { get; }

        List<IPluginInstance> AllPluginInfoList { get; }

        List<IPluginInstance> GetEnabledPluginInfoList<T>() where T : PluginBase;

        List<IService> Services { get; }

        IPluginInstance GetPluginInfo(string pluginId);

        IPluginInstance GetPluginInfo<T>() where T : PluginBase;

        Dictionary<string, string> GetPluginIdAndVersionDict();

        List<string> PackagesIdAndVersionList { get; }

        PluginBase GetPlugin(string pluginId);

        IPluginInstance GetEnabledPluginInfo<T>(string pluginId) where T : PluginBase;

        List<IPluginInstance> GetEnabledPluginInfoList<T1, T2>();

        List<PluginBase> GetEnabledPluginMetadatas<T>() where T : PluginBase;

        IPackageMetadata GetEnabledPluginMetadata<T>(string pluginId) where T : PluginBase;

        T GetEnabledFeature<T>(string pluginId) where T : PluginBase;

        List<T> GetEnabledFeatures<T>() where T : PluginBase;

        IService GetService(string pluginId);

        void Delete(string pluginId);

        void UpdateDisabled(string pluginId, bool isDisabled);

        void UpdateTaxis(string pluginId, int taxis);

        string GetPluginIconUrl(string pluginId);

        string GetPluginIconUrl(IService service);

        void ClearCache();

        SortedList<string, IPluginInstance> GetPluginSortedList();
    }
}
