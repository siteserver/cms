namespace SiteServer.Plugin.Models
{
    public class PluginRenderContext
    {
        public PluginRenderContext(string html, int publishmentSystemId, int channelId, int contentId)
        {
            Html = html;
            PublishmentSystemId = publishmentSystemId;
            ChannelId = channelId;
            ContentId = contentId;
        }

        public string Html  { get; set; }
        public int PublishmentSystemId { get; set; }
        public int ChannelId  { get; set; }
        public int ContentId { get; set; }
    }
}
