using System;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    public class RecordInfo
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string Summary { get; set; }

        public string Source { get; set; }

        public DateTime AddDate { get; set; }
    }
}
