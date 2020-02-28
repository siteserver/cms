using System.Threading.Tasks;
using Newtonsoft.Json;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Packaging;
using SS.CMS.Plugins.Impl;

namespace SS.CMS.Plugins
{
    public class PluginInstance
    {
        private PluginInstance()
        {

        }

        public PluginInstance(string directoryName, PackageMetadata metadata, string errorMessage)
        {
            if (metadata != null)
            {
                Id = metadata.Id;
                Metadata = metadata;
            }
            else
            {
                Id = directoryName;
            }
            IsRunnable = false;
            ErrorMessage = errorMessage;
        }

        public static async Task<PluginInstance> GetAsync(PackageMetadata metadata, ServiceImpl service, PluginBase plugin, long initTime)
        {
            var instance = new PluginInstance
            {
                Id = plugin.Id,
                Metadata = metadata,
                Plugin = plugin,
                Service = service,
                InitTime = initTime
            };

            var (isDisabled, taxis) = await GlobalSettings.PluginRepository.SetIsDisabledAndTaxisAsync(instance.Id);

            instance.IsRunnable = plugin != null;
            instance.IsDisabled = isDisabled;
            instance.Taxis = taxis;

            return instance;
        }

        public string Id { get; private set; }

        public PackageMetadata Metadata { get; private set; }

        [JsonIgnore]
        public PluginBase Plugin { get; private set; }

        [JsonIgnore]
        public ServiceImpl Service { get; private set; }

        public long InitTime { get; private set; }

        public string ErrorMessage { get; private set; }

        public bool IsRunnable { get; private set; }

        public bool IsDisabled { get; set; }

        public int Taxis { get; set; }
    }
}
