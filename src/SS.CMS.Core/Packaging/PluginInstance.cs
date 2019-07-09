using Newtonsoft.Json;
using SS.CMS.Repositories;

namespace SS.CMS.Core.Plugin
{
    public class PluginInstance : IPluginInstance
    {
        public PluginInstance(string directoryName, IPackageMetadata metadata, string errorMessage)
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

        public PluginInstance(IPackageMetadata metadata, ServiceImpl service, PluginBase plugin, long initTime, IPluginRepository pluginRepository)
        {
            Id = plugin.Id;
            Metadata = metadata;
            Plugin = plugin;
            Service = service;
            InitTime = initTime;

            IsRunnable = plugin != null;

            var result = pluginRepository.SetIsDisabledAndTaxisAsync(Id).GetAwaiter().GetResult();
            IsDisabled = result.IsDisabled;
            Taxis = result.Taxis;
        }

        public string Id { get; }

        public IPackageMetadata Metadata { get; }

        [JsonIgnore] public PluginBase Plugin { get; }

        [JsonIgnore] public IService Service { get; }

        public long InitTime { get; }

        public string ErrorMessage { get; }

        public bool IsRunnable { get; set; }

        public bool IsDisabled { get; set; }

        public int Taxis { get; set; }
    }
}
