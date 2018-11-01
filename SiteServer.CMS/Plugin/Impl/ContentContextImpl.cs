using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Impl
{
    public class ContentContextImpl: IContentContext
    {
        public int SiteId  { get; set; }

        public int ChannelId  { get; set; }

        public int ContentId { get; set; }
    }
}
