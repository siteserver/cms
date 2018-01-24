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

        public PluginInfo(PluginContext context, PluginService service, IPlugin plugin, long initTime)
        {
            Context = context;
            Service = service;
            Plugin = plugin;
            Metadata = Context.Metadata;
            Id = Metadata.Id;
            InitTime = initTime;

            DataProvider.PluginDao.SetIsDisabledAndTaxis(this);
        }

        public string Id { get; }

        public string ErrorMessage { get; }

        public PluginContext Context { get; }

        public IPlugin Plugin { get; }

        public PluginService Service { get; }

        public IMetadata Metadata { get; }

        public long InitTime { get; }

        public bool IsDisabled { get; set; }

        public int Taxis { get; set; }
    }
}
