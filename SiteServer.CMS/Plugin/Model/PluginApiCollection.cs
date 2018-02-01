using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Model
{
    public class PluginApiCollection: IApiCollection
    {
        public IAdminApi AdminApi { get; set; }

        public IConfigApi ConfigApi { get; set; }

        public IContentApi ContentApi { get; set; }

        public IDataApi DataApi { get; set; }

        public IFilesApi FilesApi { get; set; }

        public IChannelApi ChannelApi { get; set; }

        public IParseApi ParseApi { get; set; }

        public IPluginApi PluginApi { get; set; }

        public ISiteApi SiteApi { get; set; }

        public IUserApi UserApi { get; set; }
    }
}
