using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Impl
{
    public class ApiCollectionImpl: IApiCollection
    {
        public IAdminApi AdminApi { get; set; }

        public IConfigApi ConfigApi { get; set; }

        public IContentApi ContentApi { get; set; }

        public IDatabaseApi DatabaseApi { get; set; }

        public IChannelApi ChannelApi { get; set; }

        public IParseApi ParseApi { get; set; }

        public IPluginApi PluginApi { get; set; }

        public ISiteApi SiteApi { get; set; }

        public IUserApi UserApi { get; set; }

        public IUtilsApi UtilsApi { get; set; }
    }
}
