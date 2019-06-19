using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Table("siteserver_ContentCheck")]
    public class ContentCheckInfo : Entity
    {
        [TableColumn]
        public string TableName { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public int ChannelId { get; set; }

        [TableColumn]
        public int ContentId { get; set; }

        [TableColumn]
        public string UserName { get; set; }

        [TableColumn]
        public bool IsChecked { get; set; }

        [TableColumn]
        public int CheckedLevel { get; set; }

        [TableColumn]
        public DateTimeOffset? CheckDate { get; set; }

        [TableColumn]
        public string Reasons { get; set; }
    }
}
