using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
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
        public int UserId { get; set; }

        [TableColumn]
        public bool IsChecked { get; set; }

        [TableColumn]
        public int CheckedLevel { get; set; }

        [TableColumn]
        public DateTime? CheckDate { get; set; }

        [TableColumn]
        public string Reasons { get; set; }
    }
}
