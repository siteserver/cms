

using SS.CMS.Abstractions;

namespace SS.CMS.Plugins.Impl
{
    public class ContentContextImpl : IContentContext
    {
        public int SiteId  { get; set; }

        public int ChannelId  { get; set; }

        public int ContentId { get; set; }
    }
}
