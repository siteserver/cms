using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IPluginManager : IService
    {
        Task<IPluginInstance> GetInstanceAsync(IPackageMetadata metadata, IPluginService pluginService,
            PluginBase plugin, long initTime);

        void ClearCache();

        Task LoadPluginsAsync(string applicationPhysicalPath);

        Task<IPackageMetadata> GetMetadataAsync(string pluginId);

        Task<bool> IsExistsAsync(string pluginId);

        Task<List<IPluginInstance>> GetPluginInfoListRunnableAsync();

        Task<List<IPluginInstance>> GetAllPluginInfoListAsync();

        Task<List<IPluginInstance>> GetEnabledPluginInfoListAsync<T>() where T : PluginBase;

        Task<List<IPluginService>> GetServicesAsync();

        Task<IPluginInstance> GetPluginInfoAsync(string pluginId);

        Task<IPluginInstance> GetPluginInfoAsync<T>() where T : PluginBase;

        Task<Dictionary<string, string>> GetPluginIdAndVersionDictAsync();

        List<string> PackagesIdAndVersionList { get; }

        Task<PluginBase> GetPluginAsync(string pluginId);

        Task<IPluginInstance> GetEnabledPluginInfoAsync<T>(string pluginId) where T : PluginBase;

        Task<List<IPluginInstance>> GetEnabledPluginInfoListAsync<T1, T2>();

        Task<List<PluginBase>> GetEnabledPluginMetadatasAsync<T>() where T : PluginBase;

        Task<IPackageMetadata> GetEnabledPluginMetadataAsync<T>(string pluginId) where T : PluginBase;

        Task<T> GetEnabledFeatureAsync<T>(string pluginId) where T : PluginBase;

        Task<List<T>> GetEnabledFeaturesAsync<T>() where T : PluginBase;

        Task<IPluginService> GetServiceAsync(string pluginId);

        void Delete(string pluginId);

        Task UpdateDisabledAsync(string pluginId, bool isDisabled);

        Task UpdateTaxisAsync(string pluginId, int taxis);

        Task<string> GetPluginIconUrlAsync(string pluginId);

        string GetPluginIconUrl(IPluginService pluginService);
    }
}
