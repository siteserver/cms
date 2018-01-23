using SiteServer.Plugin;
using SiteServer.Plugin.Apis;

namespace SiteServer.CMS.Plugin.Model
{
    public class PluginContext: IContext
    {
        public IEnvironment Environment { get; set; }

        public IMetadata Metadata { get; set; }

        public IAdminApi AdminApi { get; set; }

        public IConfigApi ConfigApi { get; set; }

        public IContentApi ContentApi { get; set; }

        public IDataApi DataApi { get; set; }

        public IFilesApi FilesApi { get; set; }

        public IChannelApi ChannelApi { get; set; }

        public IParseApi ParseApi { get; set; }

        public ISiteApi SiteApi { get; set; }

        public IUserApi UserApi { get; set; }
    }
}
