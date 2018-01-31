using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Model
{
    public class PluginInfo
    {
        public PluginInfo(string id, string errorMessage)
        {
            Id = id;
            ErrorMessage = errorMessage;
        }

        public PluginInfo(PluginService service, PluginBase plugin, long initTime)
        {
            Id = plugin.Id;
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

        public PluginBase Plugin { get; }

        public PluginService Service { get; }

        public long InitTime { get; }

        public string ErrorMessage { get; }

        public bool IsDisabled { get; set; }

        public int Taxis { get; set; }
    }
}
