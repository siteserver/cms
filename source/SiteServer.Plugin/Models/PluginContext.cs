using SiteServer.Plugin.Apis;

namespace SiteServer.Plugin.Models
{
    public class PluginContext
    {
        public PluginEnvironment Environment { get; internal set; }

        public PluginMetadata Metadata { get; set; }

        public IAdminApi AdminApi { get; internal set; }

        public IConfigApi ConfigApi { get; internal set; }

        public IContentApi ContentApi { get; internal set; }

        public IDataApi DataApi { get; internal set; }

        public IFilesApi FilesApi { get; internal set; }

        public IMenuApi MenuApi { get; internal set; }

        public INodeApi NodeApi { get; internal set; }

        public IParseApi ParseApi { get; internal set; }

        public IPaymentApi PaymentApi { get; internal set; }

        public IPublishmentSystemApi PublishmentSystemApi { get; internal set; }

        public ISmsApi SmsApi { get; internal set; }

        public IUserApi UserApi { get; internal set; }
    }
}
