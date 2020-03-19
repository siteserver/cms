using System;


namespace SSCMS
{
    [Serializable]
    public class ContentCount
    {
        public int SiteId { get; set; }
        public int ChannelId { get; set; }
        public string IsChecked { get; set; }
        public int CheckedLevel { get; set; }
        public int AdminId { get; set; }
        public int Count { get; set; }
    }
}
