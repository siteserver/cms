using System;
using System.Threading.Tasks;
using Datory;

namespace SSCMS.Repositories
{
    public interface IPluginConfigRepository : IRepository
    {
        Task<T> GetAsync<T>(string pluginId, int siteId, string name);

        Task<T> GetAsync<T>(string pluginId, string name);

        [Obsolete("GetConfigAsync is deprecated, please use GetAsync instead.")]
        Task<T> GetConfigAsync<T>(string pluginId, int siteId, string name);

        [Obsolete("GetConfigAsync is deprecated, please use GetAsync instead.")]
        Task<T> GetConfigAsync<T>(string pluginId, string name);

        Task<bool> ExistsAsync(string pluginId, int siteId, string name);

        Task<bool> ExistsAsync(string pluginId, string name);

        Task<bool> SetAsync<T>(string pluginId, int siteId, string name, T value);

        Task<bool> SetAsync<T>(string pluginId, string name, T value);

        [Obsolete("SetConfigAsync is deprecated, please use SetAsync instead.")]
        Task<bool> SetConfigAsync<T>(string pluginId, int siteId, string name, T value);

        [Obsolete("SetConfigAsync is deprecated, please use SetAsync instead.")]
        Task<bool> SetConfigAsync<T>(string pluginId, string name, T value);

        Task<bool> RemoveAsync(string pluginId, int siteId, string name);

        Task<bool> RemoveAsync(string pluginId, string name);
    }
}