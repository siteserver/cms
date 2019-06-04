using SS.CMS.Plugin;

namespace SS.CMS.Core.Plugin.Impl
{
    public class ContentContextImpl: IContentContext
    {
        public int SiteId  { get; set; }

        public int ChannelId  { get; set; }

        public int ContentId { get; set; }
    }
}
