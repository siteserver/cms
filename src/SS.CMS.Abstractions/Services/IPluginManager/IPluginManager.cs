using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Services
{
    /// <summary>
    /// The entry for managing SiteServer plugins
    /// </summary>
    public partial interface IPluginManager
    {
        Task<IPackageMetadata> GetMetadataAsync(string pluginId);

        Task<bool> IsExistsAsync(string pluginId);

        Task<List<IPluginInstance>> GetPluginInfoListRunnableAsync();

        Task<List<IPluginInstance>> GetAllPluginInfoListAsync();

        Task<List<IPluginInstance>> GetEnabledPluginInfoListAsync<T>() where T : PluginBase;

        Task<List<IService>> GetServicesAsync();

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

        Task<IService> GetServiceAsync(string pluginId);

        Task DeleteAsync(string pluginId);

        Task UpdateDisabledAsync(string pluginId, bool isDisabled);

        Task UpdateTaxisAsync(string pluginId, int taxis);

        Task<string> GetPluginIconUrlAsync(string pluginId);

        string GetPluginIconUrl(IService service);

        Task<SortedList<string, IPluginInstance>> GetPluginSortedListAsync();
    }
}
