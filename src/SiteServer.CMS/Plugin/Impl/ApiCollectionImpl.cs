

namespace SiteServer.CMS.Plugin.Impl
{
    public class ApiCollectionImpl
    {
        public AdminApi AdminApi { get; set; }

        public ConfigApi ConfigApi { get; set; }

        public ContentApi ContentApi { get; set; }

        public ChannelApi ChannelApi { get; set; }

        public ParseApi ParseApi { get; set; }

        public PluginApi PluginApi { get; set; }

        public SiteApi SiteApi { get; set; }

        public UserApi UserApi { get; set; }

        public UtilsApi UtilsApi { get; set; }
    }
}
