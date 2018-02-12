using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Model
{
    public class PluginInfo
    {
        public PluginInfo(string directoryName, PackageMetadata metadata, string errorMessage)
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
            ErrorMessage = errorMessage;
        }

        public PluginInfo(PackageMetadata metadata, PluginService service, PluginBase plugin, long initTime)
        {
            Id = plugin.Id;
            Metadata = metadata;
            Plugin = plugin;
            Service = service;
            InitTime = initTime;

            bool isDisabled;
            int taxis;
            DataProvider.PluginDao.SetIsDisabledAndTaxis(Id, out isDisabled, out taxis);

            IsDisabled = isDisabled;
            Taxis = taxis;
        }

        public string Id { get; }

        public PackageMetadata Metadata { get; }

        public PluginBase Plugin { get; }

        public PluginService Service { get; }

        public long InitTime { get; }

        public string ErrorMessage { get; }

        public bool IsDisabled { get; set; }

        public int Taxis { get; set; }
    }
}
