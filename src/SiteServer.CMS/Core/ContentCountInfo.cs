using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteServer.CMS.Core
{
    public class ContentCountInfo
    {
        public int SiteId { get; set; }
        public int ChannelId { get; set; }
        public string IsChecked { get; set; }
        public int CheckedLevel { get; set; }
        public int AdminId { get; set; }
        public int Count { get; set; }
    }
}
