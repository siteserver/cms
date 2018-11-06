using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin
{
    public class PluginInstance
    {
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

        public PluginInstance(PackageMetadata metadata, ServiceImpl service, PluginBase plugin, long initTime)
        {
            Id = plugin.Id;
            Metadata = metadata;
            Plugin = plugin;
            Service = service;
            InitTime = initTime;

            bool isDisabled;
            int taxis;
            DataProvider.PluginDao.SetIsDisabledAndTaxis(Id, out isDisabled, out taxis);

            IsRunnable = plugin != null;
            IsDisabled = isDisabled;
            Taxis = taxis;
        }

        public string Id { get; }

        public PackageMetadata Metadata { get; }

        [JsonIgnore]
        public PluginBase Plugin { get; }

        [JsonIgnore]
        public ServiceImpl Service { get; }

        public long InitTime { get; }

        public string ErrorMessage { get; }

        public bool IsRunnable { get; set; }

        public bool IsDisabled { get; set; }

        public int Taxis { get; set; }
    }
}
