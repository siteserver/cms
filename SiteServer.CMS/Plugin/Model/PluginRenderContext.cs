using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Model
{
    public class PluginRenderContext: IRenderContext
    {
        public PluginRenderContext(string html, int siteId, int channelId, int contentId)
        {
            Html = html;
            SiteId = siteId;
            ChannelId = channelId;
            ContentId = contentId;
        }

        public string Html  { get; set; }
        public int SiteId { get; set; }
        public int ChannelId  { get; set; }
        public int ContentId { get; set; }
    }
}
