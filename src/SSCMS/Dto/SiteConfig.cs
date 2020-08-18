using System.Collections.Generic;

namespace SSCMS.Dto
{
    public class SiteConfig
    {
        public int SiteId { get; set; }
        public bool IsAllChannels { get; set; }
        public List<int> ChannelIds { get; set; }
    }
}
