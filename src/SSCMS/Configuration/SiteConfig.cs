using System.Collections.Generic;

namespace SSCMS.Configuration
{
    public class SiteConfig
    {
        public int SiteId { get; set; }
        public bool AllChannels { get; set; }
        public List<int> ChannelIds { get; set; }
    }
}
