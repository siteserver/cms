

using SS.CMS.Abstractions;

namespace SS.CMS.Core.PluginImpls
{
    public class ContentContextImpl : IContentContext
    {
        public int SiteId  { get; set; }

        public int ChannelId  { get; set; }

        public int ContentId { get; set; }
    }
}
