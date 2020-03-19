using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SSCMS
{
    public partial interface IPluginManager
    {
        Task<bool> SetConfigAsync(string pluginId, int siteId, object config);

        Task<bool> SetConfigAsync(string pluginId, int siteId, string name, object config);

        Task<T> GetConfigAsync<T>(string pluginId, int siteId, string name = "");

        Task<bool> RemoveConfigAsync(string pluginId, int siteId, string name = "");
    }
}
