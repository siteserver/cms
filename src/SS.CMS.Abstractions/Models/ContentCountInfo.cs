namespace SS.CMS.Models
{
    public class ContentCountInfo
    {
        public int SiteId { get; set; }
        public int ChannelId { get; set; }
        public bool IsChecked { get; set; }
        public int CheckedLevel { get; set; }
        public int UserId { get; set; }
        public int Count { get; set; }
    }
}
